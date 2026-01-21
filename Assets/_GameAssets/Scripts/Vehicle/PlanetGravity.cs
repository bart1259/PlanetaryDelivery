using UnityEngine;

public class PlanetGravity : MonoBehaviour
{
    public float gravity = 9.8f;

    public Vector3 getGravityVector(Vector3 position)
    {
        Vector3 center = transform.position;
        Vector3 direction = center - position;
        return direction.normalized * gravity;
    }
}
