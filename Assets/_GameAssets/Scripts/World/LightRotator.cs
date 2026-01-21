using UnityEngine;

public class LightRotator : MonoBehaviour
{
    public Transform directionalLight;
    public Transform playerTransform;

    public void FixedUpdate()
    {
        Vector3 playerNormalizedPosition = playerTransform.position.normalized;
        directionalLight.transform.position = playerNormalizedPosition * 1000.0f;
        directionalLight.transform.LookAt(playerTransform.position);
        
    }
}
