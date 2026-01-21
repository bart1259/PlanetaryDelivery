using UnityEngine;

public class CarLight : MonoBehaviour
{
    public CarPhysics carPhysics;

    public Material offMaterial;
    public Material onMaterial;

    public float flashSpeed; // Wave period

    private MeshRenderer _meshRenderer;
    private bool _lightsOn = false;

    void TurnLightOn()
    {
        if (_lightsOn) return;

        _lightsOn = true;
        _meshRenderer.material = onMaterial;
    }

    void TurnLightOff()
    {
        if (!_lightsOn) return;

        _lightsOn = false;
        _meshRenderer.material = offMaterial;
    }

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _lightsOn = false;
        _meshRenderer.material = offMaterial;
    }

    void Update()
    {
        bool reversing = carPhysics.reverseGear;
        if (reversing) {
            bool shouldBeOn = (Mathf.Sin(Time.time * Mathf.PI * 2.0f / flashSpeed)) > 0.0f;
            if (shouldBeOn) {
                TurnLightOn();
            } else {
                TurnLightOff();
            }
        } else {
            bool braking = Input.GetAxis("Vertical") < -0.1f;
            if (braking) {
                TurnLightOn();
            } else {
                TurnLightOff();
            }
        }

    }
}
