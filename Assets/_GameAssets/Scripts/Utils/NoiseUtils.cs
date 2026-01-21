using System;

public sealed class NoiseUtils
{
    // Permutation table (size 512 to avoid overflow)
    private readonly int[] p = new int[512];

    public NoiseUtils(int seed = 0)
    {
        int[] perm = new int[256];
        for (int i = 0; i < 256; i++) perm[i] = i;

        // Shuffle deterministically by seed
        var rng = new Random(seed);
        for (int i = 255; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (perm[i], perm[j]) = (perm[j], perm[i]);
        }

        for (int i = 0; i < 512; i++)
            p[i] = perm[i & 255];
    }

    /// <summary>
    /// Single-octave 3D Perlin noise in [0,1].
    /// </summary>
    public float Noise(float x, float y, float z)
    {
        // Find unit cube that contains point
        int X = FastFloor(x) & 255;
        int Y = FastFloor(y) & 255;
        int Z = FastFloor(z) & 255;

        // Find relative x,y,z within cube
        x -= FastFloor(x);
        y -= FastFloor(y);
        z -= FastFloor(z);

        // Compute fade curves for each of x,y,z
        float u = Fade(x);
        float v = Fade(y);
        float w = Fade(z);

        // Hash coordinates of the cube corners
        int A  = p[X] + Y;
        int AA = p[A] + Z;
        int AB = p[A + 1] + Z;
        int B  = p[X + 1] + Y;
        int BA = p[B] + Z;
        int BB = p[B + 1] + Z;

        // Add blended results from 8 corners of cube
        float res =
            Lerp(w,
                Lerp(v,
                    Lerp(u, Grad(p[AA], x, y, z),     Grad(p[BA], x - 1, y, z)),
                    Lerp(u, Grad(p[AB], x, y - 1, z), Grad(p[BB], x - 1, y - 1, z))
                ),
                Lerp(v,
                    Lerp(u, Grad(p[AA + 1], x, y, z - 1),     Grad(p[BA + 1], x - 1, y, z - 1)),
                    Lerp(u, Grad(p[AB + 1], x, y - 1, z - 1), Grad(p[BB + 1], x - 1, y - 1, z - 1))
                )
            );

        // res is in [-1,1] -> map to [0,1]
        return (res + 1f) * 0.5f;
    }

    /// <summary>
    /// Fractal Brownian Motion (FBM) 3D noise in [0,1] using octaves/lacunarity/persistence.
    /// baseFrequency controls the starting scale (1 = "normal").
    /// </summary>
    public float Fbm(
        float x, float y, float z,
        int octaves,
        float lacunarity,
        float persistence,
        float baseFrequency = 1f,
        float offsetX = 0f, float offsetY = 0f, float offsetZ = 0f)
    {
        if (octaves <= 0) return 0f;

        float sum = 0f;
        float amp = 1f;
        float freq = baseFrequency;
        float norm = 0f;

        for (int i = 0; i < octaves; i++)
        {
            sum += amp * Noise((x + offsetX) * freq, (y + offsetY) * freq, (z + offsetZ) * freq);
            norm += amp;

            amp *= persistence;   // amplitude decreases
            freq *= lacunarity;   // frequency increases
        }

        return (norm > 0f) ? (sum / norm) : 0f; // keep in [0,1]
    }

    // --- Helpers (Improved Perlin) ---

    private static int FastFloor(float f) => (f >= 0) ? (int)f : (int)f - 1;

    private static float Fade(float t) => t * t * t * (t * (t * 6f - 15f) + 10f);

    private static float Lerp(float t, float a, float b) => a + t * (b - a);

    private static float Grad(int hash, float x, float y, float z)
    {
        // Convert low 4 bits of hash code into 12 gradient directions.
        int h = hash & 15;
        float u = (h < 8) ? x : y;
        float v = (h < 4) ? y : (h == 12 || h == 14) ? x : z;
        return (((h & 1) == 0) ? u : -u) + (((h & 2) == 0) ? v : -v);
    }
}