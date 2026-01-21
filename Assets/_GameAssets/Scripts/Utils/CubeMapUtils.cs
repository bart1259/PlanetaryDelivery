using UnityEngine;

public static class CubemapUtils
{
    public static void DirectionToCubemapFaceUV(
        Vector3 direction,
        out CubemapFace face,
        out Vector2 uv01)
    {
        // Safe normalize (assume user passes normalized, but this avoids NaNs).
        float mag = direction.magnitude;
        if (mag > 0f) direction /= mag;
        else
        {
            face = CubemapFace.PositiveZ;
            uv01 = new Vector2(0.5f, 0.5f);
            return;
        }

        float ax = Mathf.Abs(direction.x);
        float ay = Mathf.Abs(direction.y);
        float az = Mathf.Abs(direction.z);

        float u; // in [-1, 1]
        float v; // in [-1, 1]

        // Choose the major axis (the face the ray hits).
        if (ax >= ay && ax >= az)
        {
            // X
            if (direction.x > 0f)
            {
                face = CubemapFace.PositiveX;
                u = -direction.z / ax;
                v = -direction.y / ax;
            }
            else
            {
                face = CubemapFace.NegativeX;
                u =  direction.z / ax;
                v = -direction.y / ax;
            }
        }
        else if (ay >= ax && ay >= az)
        {
            // Y
            if (direction.y > 0f)
            {
                face = CubemapFace.PositiveY;
                u =  direction.x / ay;
                v = -direction.z / ay;
            }
            else
            {
                face = CubemapFace.NegativeY;
                u =  direction.x / ay;
                v =  direction.z / ay;
            }
        }
        else
        {
            // Z
            if (direction.z > 0f)
            {
                face = CubemapFace.PositiveZ;
                u =  direction.x / az;
                v = -direction.y / az;
            }
            else
            {
                face = CubemapFace.NegativeZ;
                u = -direction.x / az;
                v = -direction.y / az;
            }
        }

        // Map from [-1,1] to [0,1]
        uv01 = new Vector2((u + 1f) * 0.5f, (v + 1f) * 0.5f);
    }
}