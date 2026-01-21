using UnityEngine;

// Attached to independent GO of the car
public class CarCamera : MonoBehaviour
{
    public Transform carTransform;

    // Camera that orbits the car
    public Transform cameraTransform;

    public float translationSmoothness = 2.0f;
    public float rotationSmoothness = 2.0f;


    void Start()
    {
        
    }

    void Update()
    {
        Vector3 desiredPosition = carTransform.position;
        Vector3 currentPosition = transform.position;
        transform.position = Vector3.Lerp(currentPosition, desiredPosition, Time.deltaTime * translationSmoothness);

        Vector3 desiredUp = carTransform.position;
        Vector3 desiredForward = carTransform.forward;
        Vector3 currentUp = transform.up;
        Vector3 currentForward = transform.forward;
        // Smooth look
        Vector3 newUp = Vector3.Slerp(currentUp, desiredUp, Time.deltaTime * rotationSmoothness);
        Vector3 newForward = Vector3.Slerp(currentForward, desiredForward, Time.deltaTime * rotationSmoothness);
        transform.rotation = Quaternion.LookRotation(newForward, newUp);
    }
}