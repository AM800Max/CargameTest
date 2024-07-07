using UnityEngine;

public class Car_Controller : MonoBehaviour
{
    [Header("Car Settings")]
    public WheelCollider FrontLeftWheelCollider;
    public WheelCollider FrontRightWheelCollider;
    public WheelCollider RearLeftWheelCollider;
    public WheelCollider RearRightWheelCollider;

    public Transform FrontLeftWheelTransform;
    public Transform FrontRightWheelTransform;
    public Transform RearLeftWheelTransform;
    public Transform RearRightWheelTransform;

    public float MotorTorque = 1500f;
    public float MaxSteerAngle = 30f;
    public float BrakeForce = 3000f;
    public float MaximumSpeed = 200f;
    public bool enable_boost = false;
    public float boost_force = 5000f;
    public KeyCode Boost_KeyCode = KeyCode.LeftShift;

    public bool Enable_Engine_Sound = false;
    public AudioSource Engine_Sound;
    public bool Enable_Horn_Sound = false;
    public AudioSource Horn_Source;
    public KeyCode Horn_Key = KeyCode.H;
    public bool Enable_Crash_Sound = false;
    public AudioSource Crash_Sound;

    public bool Enable_Headlights_Lights = false;
    public Light[] Headlights;
    public KeyCode Headlights_Key = KeyCode.L;
    public bool Enable_Brakelights_Lights = false;
    public MeshRenderer[] Brakelights;
    public KeyCode Handbrake_Key = KeyCode.Space;

    public GameObject Center_of_Mass;
    public float stability = 0.3f; // Einstellbarer Wert für Stabilität
    public float stabilizerForce = 5000f; // Einstellbare Kraft für die Stabilisierung

    [Header("Jump Settings")]
    public float JumpForce = 5000f;
    public float ForwardForce = 2000f;
    public KeyCode Jump_Key = KeyCode.J;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Center_of_Mass.transform.localPosition;

        if (Enable_Engine_Sound && Engine_Sound != null)
        {
            Engine_Sound.loop = true;
            Engine_Sound.Play();
        }
    }

    private void Update()
    {
        HandleMotor();
        HandleSteering();
        UpdateWheels();

        if (Enable_Horn_Sound && Horn_Source != null && Input.GetKeyDown(Horn_Key))
        {
            Horn_Source.Play();
        }

        if (Enable_Headlights_Lights && Input.GetKeyDown(Headlights_Key))
        {
            foreach (var light in Headlights)
            {
                light.enabled = !light.enabled;
            }
        }
    }

    private void HandleMotor()
    {
        float motor = MotorTorque * Input.GetAxis("Vertical");
        float steering = MaxSteerAngle * Input.GetAxis("Horizontal");

        if (enable_boost && Input.GetKey(Boost_KeyCode))
        {
            motor += boost_force;
        }

        FrontLeftWheelCollider.motorTorque = motor;
        FrontRightWheelCollider.motorTorque = motor;

        FrontLeftWheelCollider.steerAngle = steering;
        FrontRightWheelCollider.steerAngle = steering;

        // Automatisches Bremsen wenn keine Taste gedrückt wird
        if (Input.GetAxis("Vertical") == 0)
        {
            FrontLeftWheelCollider.brakeTorque = BrakeForce;
            FrontRightWheelCollider.brakeTorque = BrakeForce;
            RearLeftWheelCollider.brakeTorque = BrakeForce;
            RearRightWheelCollider.brakeTorque = BrakeForce;
        }
        else
        {
            FrontLeftWheelCollider.brakeTorque = 0;
            FrontRightWheelCollider.brakeTorque = 0;
            RearLeftWheelCollider.brakeTorque = 0;
            RearRightWheelCollider.brakeTorque = 0;
        }

        if (Input.GetKey(Handbrake_Key))
        {
            RearLeftWheelCollider.brakeTorque = BrakeForce;
            RearRightWheelCollider.brakeTorque = BrakeForce;

            if (Enable_Brakelights_Lights)
            {
                foreach (var light in Brakelights)
                {
                    light.enabled = true;
                }
            }
        }
        else
        {
            if (Input.GetAxis("Vertical") != 0)
            {
                RearLeftWheelCollider.brakeTorque = 0;
                RearRightWheelCollider.brakeTorque = 0;
            }

            if (Enable_Brakelights_Lights)
            {
                foreach (var light in Brakelights)
                {
                    light.enabled = false;
                }
            }
        }

        if (Enable_Engine_Sound && Engine_Sound != null)
        {
            Engine_Sound.pitch = Mathf.Clamp(rb.velocity.magnitude / 50f, 0.5f, 2f);
        }
    }

    private void HandleSteering()
    {
        float steering = MaxSteerAngle * Input.GetAxis("Horizontal");
        FrontLeftWheelCollider.steerAngle = steering;
        FrontRightWheelCollider.steerAngle = steering;
    }

    private void UpdateWheels()
    {
        UpdateWheelTransform(FrontLeftWheelCollider, FrontLeftWheelTransform);
        UpdateWheelTransform(FrontRightWheelCollider, FrontRightWheelTransform);
        UpdateWheelTransform(RearLeftWheelCollider, RearLeftWheelTransform);
        UpdateWheelTransform(RearRightWheelCollider, RearRightWheelTransform);
    }

    private void UpdateWheelTransform(WheelCollider wheelCollider, Transform trans)
    {
        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);
        trans.position = position;
        trans.rotation = rotation;
    }

    private void FixedUpdate()
    {
        StabilizeVehicle();

        // Sprung-Funktion
        if (Input.GetKeyDown(Jump_Key))
        {
            Vector3 jumpVector = transform.up * JumpForce + transform.forward * ForwardForce;
            rb.AddForce(jumpVector, ForceMode.Impulse);
        }
    }

    private void StabilizeVehicle()
    {
        WheelHit hit;
        foreach (WheelCollider wheel in new WheelCollider[] { FrontLeftWheelCollider, FrontRightWheelCollider, RearLeftWheelCollider, RearRightWheelCollider })
        {
            bool grounded = wheel.GetGroundHit(out hit);
            if (grounded)
            {
                rb.AddForceAtPosition((transform.up - hit.normal) * stabilizerForce, wheel.transform.position);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Enable_Crash_Sound && Crash_Sound != null)
        {
            Crash_Sound.Play();
        }
    }
}
