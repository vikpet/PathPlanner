using UnityEngine;

namespace UnitySampleAssets.Vehicles.Car
{
    public class CarController : MonoBehaviour
    {

        // This car component is designed to be used on a gameobject which has wheels attached.
        // The wheels must be child objects, and each have a Wheel script attached, and a WheelCollider component.

        // Even though wheelcolliders have their own settings for grip loss, this car script (and its accompanying
        // wheel scripts) modify the settings on the wheelcolliders at runtime, to give a more exaggerated and fun
        // experience, allowing burnouts and drifting behavior in a way that is not readily achievable using
        // constant values on wheelcolliders alone.

        // The code priorities fun over realism, and although a gears system is included, it is not used to 
        // 'drive' the engine. Instead, the current revs and gear are calculated retrospectively based
        // on the car's current speed. These gear and rev values can then be read and used by a GUI or Sound component.


        [SerializeField] private float maxSteerAngle = 28; // The maximum angle the car can steer
        [SerializeField] private float steeringResponseSpeed = 200; // how fast the steering responds
        [SerializeField] [Range(0, 1)] private float maxSpeedSteerAngle = 0.23f;// the reduction in steering angle at max speed
        [SerializeField] [Range(0, .5f)] private float maxSpeedSteerResponse = 0.5f;// the reduction in steer response at max speed
        [SerializeField] private float maxSpeed = 60; // the maximum speed (in meters per second!)
        [SerializeField] private float maxTorque = 35; // the maximum torque of the engine
        [SerializeField] private float minTorque = 10; // the minimum torque of the engine
        [SerializeField] private float brakePower = 40; // how powerful the brakes are at stopping the car
        [SerializeField] private float adjustCentreOfMass = 0.25f; // vertical offset for the centre of mass
        [SerializeField] private Advanced advanced;// container for the advanced setting which will expose as a foldout in the inspector

        [System.Serializable]
        public class Advanced // the advanced settings for the car controller
        {

            public float downForce = 30; // the amount of downforce applied (speed is factored in)
            public float reversingSpeedFactor = 0.3f;// The car's maximum reverse speed, as a proportion of its max forward speed.
            //public float skidGearLockFactor = 0.1f;// The car will not automatically change gear if the current skid factor is higher than this value.
            public float accelChangeSmoothing = 2f; // Used to smooth out changes in acceleration input.
        }

        private Wheel[] wheels; // Stores a reference to each wheel attached to this car.
        private float accelBrake; // The acceleration or braking input (1 to -1 range)
        private float smallSpeed;// A small proportion of max speed, used to decide when to start accelerating/braking when transitioning between fwd and reverse motion
        private float maxReversingSpeed; // The maximum reversing speed
        private bool immobilized; // Whether the car is accepting inputs.

        public float CurrentSpeed { get; private set; } // the current speed of the car
        public float CurrentSteerAngle { get; private set; } // The current steering angle for steerable wheels.
        public float AccelInput { get; private set; } // the current acceleration input
        public float BrakeInput { get; private set; } // the current brake input
        public float SpeedFactor { get; private set; }// value between 0-1 of the car's current speed relative to max speed



        // the following values are provided as read-only properties,
        // and are required by the Wheel script to compute grip, burnout, skidding, etc
        public float MaxSpeed
        {
            get { return maxSpeed; }
        }


        public float MaxTorque
        {
            get { return maxTorque; }
        }


        public float MaxSteerAngle
        {
            get { return maxSteerAngle; }
        }


        // variables added due to separating out things into functions!
        private bool anyOnGround;
        private float curvedSpeedFactor;
        private bool reversing;
        private float targetAccelInput;// target accel input is our desired acceleration input. We smooth towards it later

        private void Awake()
        {
            // get a reference to all wheel attached to the car.
            wheels = GetComponentsInChildren<Wheel>();


            // deactivate and reactivate the gameobject - this is a workaround
            // to a bug where changes to wheelcolliders at runtime are not 'taken'
            // by the rigidbody unless this step is performed :(
            gameObject.SetActive(false);
            gameObject.SetActive(true);

            // a few useful speeds are calculated for use later:
            smallSpeed = maxSpeed*0.05f;
            maxReversingSpeed = maxSpeed*advanced.reversingSpeedFactor;
        }


        private void OnEnable()
        {
            // set adjusted centre of mass.
            rigidbody.centerOfMass = Vector3.up*adjustCentreOfMass;
        }


        public void Move(float steerInput, float accelBrakeInput)
        {

            // lose control of engine if immobilized
            if (immobilized) accelBrakeInput = 0;


            ConvertInputToAccelerationAndBraking(accelBrakeInput);
            CalculateSpeedValues();
            ProcessWheels(steerInput);
            ApplyDownforce();

        }

        private void ConvertInputToAccelerationAndBraking(float accelBrakeInput)
        {
            // move.Z is the user's fwd/back input. We need to convert it into acceleration and braking.
            // this differs based on if the car is currently moving forward or backward.
            // change is based slightly away from the zero value (by "smallspeed") so that for example when
            // the car transitions from reversing to moving forwards, the car does not need to come to a complete
            // rest before starting to accelerate.

            reversing = false;
            if (accelBrakeInput > 0)
            {
                if (CurrentSpeed > -smallSpeed)
                {
                    // pressing forward while moving forward : accelerate!
                    targetAccelInput = accelBrakeInput;
                    BrakeInput = 0;
                }
                else
                {
                    // pressing forward while movnig backward : brake!
                    BrakeInput = accelBrakeInput;
                    targetAccelInput = 0;
                }
            }
            else
            {
                if (CurrentSpeed > smallSpeed)
                {
                    // pressing backward while moving forward : brake!
                    BrakeInput = -accelBrakeInput;
                    targetAccelInput = 0;
                }
                else
                {
                    // pressing backward while moving backward : accelerate (in reverse direction)
                    BrakeInput = 0;
                    targetAccelInput = accelBrakeInput;
                    reversing = true;
                }
            }
            // smoothly move the current accel towards the target accel value.
            AccelInput = Mathf.MoveTowards(AccelInput, targetAccelInput, Time.deltaTime*advanced.accelChangeSmoothing);

        }

        private void CalculateSpeedValues()
        {
            // current speed is measured in the forward direction of the car (sliding sideways doesn't count!)
            CurrentSpeed = transform.InverseTransformDirection(rigidbody.velocity).z;
            // speedfactor is a normalized representation of speed in relation to max speed:
            SpeedFactor = Mathf.InverseLerp(0, reversing ? maxReversingSpeed : maxSpeed, Mathf.Abs(CurrentSpeed));
            curvedSpeedFactor = reversing ? 0 : CurveFactor(SpeedFactor);
        }


        private void ProcessWheels(float steerInput)
        {
            // Process each wheel:
            // we accumulate some averages of all wheels into these vars:

            var numPowerWheels = 0;
            anyOnGround = false;
            foreach (var wheel in wheels)
            {
                var wheelCollider = wheel.wheelCollider;
                if (wheel.steerable)
                {
                    // apply steering to this wheel. The actual steering change applied is based on the steering range, current speed, 
                    // and whether the wheel is currently pointing in the direction that steering is being applied
                    var currentSteerSpeed = Mathf.Lerp(steeringResponseSpeed,
                                                       steeringResponseSpeed*maxSpeedSteerResponse, curvedSpeedFactor);
                    var currentMaxAngle = Mathf.Lerp(maxSteerAngle, maxSteerAngle*maxSpeedSteerAngle, curvedSpeedFactor);

                    // increase steering speed if steering input is in opposite direction to current wheel direction (for faster response)
//                    if (Mathf.Sign(steerInput) != Mathf.Sign(CurrentSteerAngle))
//                    {
//                        currentSteerSpeed *= advanced.oppositeLockSteeringCorrection;
//                    }
                    // modify the actual steer angle of the wheel by these calculated values:
                    CurrentSteerAngle = Mathf.MoveTowards(CurrentSteerAngle, steerInput*currentMaxAngle,
                                                          Time.deltaTime*currentSteerSpeed);
                    wheelCollider.steerAngle = CurrentSteerAngle;
                }
                // acumulate skid amount from this wheel, for averaging later
//                AvgSkid += wheel.SkidFactor;
                if (wheel.powered)
                {
                    // apply power to wheels marked as powered:
                    // available torque drops off as we approach max speed
                    var currentMaxTorque = Mathf.Lerp(maxTorque, (SpeedFactor < 1) ? minTorque : 0,
                                                      reversing ? SpeedFactor : curvedSpeedFactor);
                    wheelCollider.motorTorque = AccelInput*currentMaxTorque;
                    // accumulate RPM from this wheel, for averaging later
 //                   AvgPowerWheelRpmFactor += wheel.Rpm/wheel.MaxRpm;
                    numPowerWheels++;
                }
                // apply curent brake torque to wheel
                wheelCollider.brakeTorque = BrakeInput*brakePower;
                // if any wheel is on the ground, the car is considered grounded

                if (wheel.OnGround)
                {
                    anyOnGround = true;
                }
            }

        }

        private void ApplyDownforce()
        {
            // apply downforce
            if (anyOnGround)
            {
                rigidbody.AddForce(-transform.up*curvedSpeedFactor*advanced.downForce);
            }
        }


        // simple function to add a curved bias towards 1 for a value in the 0-1 range
        private float CurveFactor(float factor)
        {
            return 1 - (1 - factor)*(1 - factor);
        }


        // unclamped version of Lerp, to allow value to exceed the from-to range
        private float ULerp(float from, float to, float value)
        {
            return (1.0f - value)*from + value*to;
        }


        private void OnDrawGizmosSelected()
        {
            // visualise the adjusted centre of mass in the editor
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(rigidbody.position + Vector3.up*adjustCentreOfMass, 0.2f);
        }

        // Immobilize can be called from other objects, if the car needs to be made uncontrollable
        // (eg, from asplosion!)
        public void Immobilize()
        {
            immobilized = true;
        }

        // Reset is called via the ObjectResetter script, if present.
        public void Reset()
        {
            immobilized = false;
        }
    }
}