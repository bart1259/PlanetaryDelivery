using UnityEngine;
using System;

// Heavily inspired by: https://www.youtube.com/watch?v=CdPYlj5uZeI
public class CarPhysics : MonoBehaviour
{
    public Transform wheelFL;
    public Transform wheelFR;
    public Transform wheelRL;
    public Transform wheelRR;

    private Transform wheelFLGraphic;
    private Transform wheelFRGraphic;
    private Transform wheelRLGraphic;
    private Transform wheelRRGraphic;

    private Rigidbody rb;

    public float restDistance = 1.0f;

    public float springK = 1.0f;

    public float damping = 1.0f;

    public float wheelRadius = 0.25f;

    public float maxSteerAngle = 30.0f;
    public float currentSteerAngle = 0.0f;
    public AnimationCurve steerSpeedCurve;

    // TODO: At some point, we should make steerspeed dependent on the speed of the car
    public float steerSpeed = 5.0f;

    // TODO: Make this dependent on the speed of the car
    public float tireGrip = 1.0f;
    public float tireMass = 0.0f;


    public AnimationCurve torqueCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    public float torqueMultiplier = 1.0f;
    public float maxSpeed = 20.0f;

    public float breakingForce = 10.0f;

    public float carSpeed = 0.0f;

    public PlanetGravity planetGravity;
    public PlanetGenerationManager planetGenerationManager;


    [Header("Debug")]
    public bool drawSuspension = false;

    public bool reverseGear = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wheelFLGraphic = wheelFL.GetChild(0);
        wheelFRGraphic = wheelFR.GetChild(0);
        wheelRLGraphic = wheelRL.GetChild(0);
        wheelRRGraphic = wheelRR.GetChild(0);

        rb = GetComponent<Rigidbody>();
    }

    void HandleWheel(Transform wheel, Transform wheelGraphic, float steerAngle = 0.0f, float torque = 0.0f) {

        // Update steering
        wheel.localEulerAngles = new Vector3(0, steerAngle, 0);

        RaycastHit hit;
        // Do raycast
        Physics.Raycast(wheel.position, -wheel.up, out hit, restDistance + wheelRadius);

        if (hit.collider != null) {
            if (hit.collider.gameObject.CompareTag("Water")) {
                ResetCar();
                return;
            }

            Vector3 springDir = wheel.up;

            Vector3 wheelVel = rb.GetPointVelocity(wheel.position);

            float offset = restDistance - hit.distance;

            float vel = Vector3.Dot(wheelVel, springDir);

            float force = (offset * springK) - (vel * damping);

            rb.AddForceAtPosition(force * springDir, wheel.position);

            if (drawSuspension) {
                Debug.DrawRay(wheel.position, -wheel.up * hit.distance, Color.red);
            }

            // Update graphics
            float wheelPosition = hit.distance;
            wheelPosition = Mathf.Clamp(wheelPosition, 0, restDistance);
            
            wheelGraphic.localPosition = new Vector3(0, -wheelPosition + wheelRadius, 0);

        }

        // Stearing
        if (hit.collider != null) {
            // Get velocity of the wheel
            Vector3 wheelVel = rb.GetPointVelocity(wheel.position);

            float steerVel = Vector3.Dot(wheel.right, wheelVel);

            float desiredVelChange = -steerVel * tireGrip;

            float desiredAccel = desiredVelChange / Time.fixedDeltaTime;

            rb.AddForceAtPosition(wheel.right * desiredAccel * tireMass, wheel.position);
        }

        // Acceleration / braking
        if (hit.collider != null) {
            // Get acceleration direction
            Vector3 accelDir = wheel.forward;
            
            rb.AddForceAtPosition(accelDir * torque, wheel.position);
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && Mathf.Abs(carSpeed) < 1.0f) {
            reverseGear = !reverseGear;
        } else if (Input.GetKeyDown(KeyCode.R)) {
            WarningUI.Instance.ShowWarning("Cannot switch gear while moving");
        }

        if (Input.GetKeyDown(KeyCode.K)) {
            ResetCar();
        }
    }

    public void ResetCar()
    {
        Vector3 closestRoadPoint = planetGenerationManager.GetClosestRoadToPoint(transform.position);
        transform.position = closestRoadPoint + closestRoadPoint.normalized * 8.0f;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = transform.position;

        // Rotate too
        transform.up = transform.position.normalized;
        rb.rotation = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Apply gravity
        Vector3 gravity = planetGravity.getGravityVector(transform.position);
        rb.AddForce(gravity * rb.mass);

        // Steering
        float horizontal = Input.GetAxis("Horizontal");
        float maxSteerAngleAtSpeed = steerSpeedCurve.Evaluate(Mathf.Abs(carSpeed) / maxSpeed) * maxSteerAngle;
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, maxSteerAngleAtSpeed * horizontal, Time.deltaTime * steerSpeed);

        // Get Speed of the car (signed)
        carSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        // Throttle / Brake input
        float vertical = Input.GetAxis("Vertical");
        float throttle = Mathf.Clamp01(vertical);     // W / Up
        float brake = Mathf.Clamp01(-vertical);       // S / Down

        float frontTorque = 0.0f;  // drive + brake on front wheels
        float brakeTorque = 0.0f;  // brake only (used on rear wheels)

        if (!reverseGear)
        {
            // Forward gear: accelerate forward with throttle
            if (throttle > 0f && carSpeed < maxSpeed)
                frontTorque += torqueCurve.Evaluate(Mathf.Abs(carSpeed) / maxSpeed) * torqueMultiplier * throttle;

            // Forward gear: braking only if moving forward
            if (brake > 0f && carSpeed > 0.1f)
                brakeTorque = -breakingForce * brake; // negative = opposite of forward
        }
        else
        {
            // Reverse gear: accelerate backward with throttle
            if (throttle > 0f && -carSpeed < maxSpeed)
                frontTorque += -torqueCurve.Evaluate(Mathf.Abs(carSpeed) / maxSpeed) * torqueMultiplier * throttle;

            // Reverse gear: braking only if moving backward
            if (brake > 0f && carSpeed < -0.1f)
                brakeTorque = breakingForce * brake; // positive = opposite of backward
        }

        // Apply braking to front too
        frontTorque += brakeTorque;

        HandleWheel(wheelFL, wheelFLGraphic, currentSteerAngle, frontTorque);
        HandleWheel(wheelFR, wheelFRGraphic, currentSteerAngle, frontTorque);

        // Rear wheels: braking only (keeps your original “no rear drive torque” behavior)
        HandleWheel(wheelRL, wheelRLGraphic, 0, brakeTorque);
        HandleWheel(wheelRR, wheelRRGraphic, 0, brakeTorque);
    }
}
