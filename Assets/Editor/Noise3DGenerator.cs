using UnityEngine;
using UnityEditor;

public class Noise3DGenerator
{
    [MenuItem("Tools/Generate 3D Perlin Worley Noise Texture")]
    static void Generate3DNoise()
    {
        int size = 64;

        Texture3D texture = new Texture3D(size, size, size, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.filterMode = FilterMode.Trilinear;

        Color[] colors = new Color[size * size * size];

        int index = 0;

        for (int z = 0; z < size; z++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector3 p = new Vector3(
                        (float)x / size,
                        (float)y / size,
                        (float)z / size
                    );

                    float perlin = PerlinFBM(p, 6f, 4);
                    float worley = WorleyNoise(p, 6);

                    // Invert Worley so cell interiors are denser than borders
                    float invertedWorley = 1f - worley;

                    float combined = Mathf.Clamp01(perlin * invertedWorley);

                    // Softer, more cloud-like mask
                    float density = Mathf.SmoothStep(0.15f, 0.85f, combined);

                    colors[index] = new Color(perlin, invertedWorley, combined, density);
                    index++;
                }
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        AssetDatabase.CreateAsset(texture, "Assets/PerlinWorleyNoise3D.asset");
        AssetDatabase.SaveAssets();

        Debug.Log("Perlin + Worley 3D Noise Texture Generated!");
    }

    static float PerlinFBM(Vector3 p, float scale, int octaves)
    {
        float value = 0f;
        float amplitude = 0.5f;
        float frequency = scale;

        for (int i = 0; i < octaves; i++)
        {
            float xy = Mathf.PerlinNoise(p.x * frequency, p.y * frequency);
            float yz = Mathf.PerlinNoise(p.y * frequency, p.z * frequency);
            float xz = Mathf.PerlinNoise(p.x * frequency, p.z * frequency);

            value += ((xy + yz + xz) / 3f) * amplitude;

            frequency *= 2f;
            amplitude *= 0.5f;
        }

        return Mathf.Clamp01(value);
    }

    static float WorleyNoise(Vector3 p, int cells)
    {
        Vector3 cellPos = p * cells;

        int ix = Mathf.FloorToInt(cellPos.x);
        int iy = Mathf.FloorToInt(cellPos.y);
        int iz = Mathf.FloorToInt(cellPos.z);

        float minDist = 999f;

        for (int z = -1; z <= 1; z++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    Vector3Int neighbor = new Vector3Int(ix + x, iy + y, iz + z);

                    Vector3 featurePoint = new Vector3(
                        neighbor.x + Random01(neighbor.x, neighbor.y, neighbor.z),
                        neighbor.y + Random01(neighbor.x + 19, neighbor.y, neighbor.z),
                        neighbor.z + Random01(neighbor.x, neighbor.y + 37, neighbor.z)
                    );

                    float dist = Vector3.Distance(cellPos, featurePoint);
                    minDist = Mathf.Min(minDist, dist);
                }
            }
        }

        return Mathf.Clamp01(minDist);
    }

    static float Random01(int x, int y, int z)
    {
        int n = x * 15731 + y * 789221 + z * 1376312589;
        n = (n << 13) ^ n;

        return 1.0f - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0f;
    }
}