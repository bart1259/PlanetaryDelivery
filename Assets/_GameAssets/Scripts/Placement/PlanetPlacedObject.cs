using UnityEngine;

public class PlanetPlacedObject : MonoBehaviour
{
    public bool allignToNormal = true;
    public bool randomizeRotation = false;

    public bool randomScale = false;
    public float minScale = 1.0f;
    public float maxScale = 1.0f;
    
    public void Reallign(LayerMask planetMask)
    {
        // Shoot a raycast towards the center of the world to find the normal
        Vector3 rayOrigin = transform.position.normalized * 250.0f;
        Ray ray = new Ray(rayOrigin, -rayOrigin.normalized);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 400.0f, planetMask))
        {
            // Reallign the object
            Vector3 normal = hit.normal;
            transform.position = hit.point;
            if (allignToNormal)
                transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);

            if (randomizeRotation)
            {
                float randomYRotation = Random.Range(0.0f, 360.0f);
                transform.Rotate(Vector3.up, randomYRotation, Space.Self);
            }

            if (randomScale)
            {
                float scale = Random.Range(minScale, maxScale);
                transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }
}

