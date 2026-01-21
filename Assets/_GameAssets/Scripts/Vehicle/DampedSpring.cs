using UnityEngine;
using System;

[System.Serializable]
[CreateAssetMenu(fileName = "DampedSpring", menuName = "Scriptable Objects/PlanetDelivery/DampedSpring")]
public class DampedSpring : ScriptableObject
{
    public float springConstant = 1f;
    public float dampingConstant = 1f;

    private float lastPosition;
    private float position;

    public void SetPosition(float pos) {
        lastPosition = position;
        position = pos;
    }

    public float CalculateForce(float displacement)
    {
        float velocity = (position - lastPosition) / Time.fixedDeltaTime;
        return -springConstant * displacement - dampingConstant * velocity;
    }
}
