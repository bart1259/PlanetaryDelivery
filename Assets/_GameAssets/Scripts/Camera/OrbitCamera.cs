using UnityEngine;

// Attach to the pivot object (parent of the Camera).
public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform cam; // assign the Camera transform here

    [Header("Rotation")]
    public float rotationSpeed = 180f;
    public float maxVerticalAngle = 80f;

    [Header("Zoom")]
    public float zoomSpeed = 50f;
    public float minZoom = 220f;
    public float maxZoom = 400f;

    float yaw;
    float pitch;
    float distance;

    void Awake()
    {
        if (!cam) return;

        // Initialize from current transform so it doesn't jump on play.
        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = NormalizeAngle(e.x);


        distance = -cam.localPosition.z;
        distance = Mathf.Clamp(distance, minZoom, maxZoom);
        cam.localPosition = new Vector3(0f, 0f, -distance);
    }

    void LateUpdate()
    {
        if (!cam) return;

        // Rotate with left mouse drag
        if (Input.GetMouseButton(0))
        {
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");

            yaw += mx * rotationSpeed * Time.deltaTime;
            pitch -= my * rotationSpeed * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, -maxVerticalAngle, maxVerticalAngle);
        }

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Zoom with scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            distance -= scroll * zoomSpeed;              // no deltaTime here
            distance = Mathf.Clamp(distance, minZoom, maxZoom);
            cam.localPosition = new Vector3(0f, 0f, -distance);
        }
    }

    public void LookAt(Vector3 direction)
    {
        direction = direction.normalized;
        pitch = Mathf.Asin(direction.y) * Mathf.Rad2Deg;
        yaw = 180 + Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        pitch = Mathf.Clamp(pitch, -maxVerticalAngle, maxVerticalAngle);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    static float NormalizeAngle(float a)
    {
        a %= 360f;
        if (a > 180f) a -= 360f;
        return a;
    }
}