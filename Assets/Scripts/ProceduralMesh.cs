using UnityEngine;

//-----------------------------------------------------------------------------
// name: ProceduralMesh.cs
// desc: generate canyon walls via Perlin noise; move inwards with progress
//-----------------------------------------------------------------------------

public class ProceduralMesh : MonoBehaviour
{
    Mesh mesh;
    static int xSize = 1000;
    static int zSize = 55;
    Vector3[] vertices = new Vector3[(xSize + 1) * (zSize + 1)];
    float[,] history = new float[xSize + 1, zSize + 1];
    int[] triangles;

    public int octaves = 4;
    public float scale = 0.3f;
    public float factor = 4f;
    public float persistance = 0.5f;

    public GameObject cam;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float amplitude = 1;
                float noiseHeight = 0;

                for (int j = 0; j < octaves; j++)
                {
                    float perlinValue = Mathf.PerlinNoise(x * scale, z * scale) * factor;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                }

                float y = Mathf.Round(noiseHeight);
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }

            vert++;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    // move walls inwards with player progress down canyon
    void Update()
    {
        float invLerp = Mathf.InverseLerp(18, 885, cam.transform.position.x);
        float lerp = 0;

        if (transform.position.z > 0)
        {
            // move left wall to the right
            lerp = Mathf.Lerp(10, 6.7f, invLerp);
        }
        else
        {
            // move right wall to the left
            lerp = Mathf.Lerp(-10, -5.7f, invLerp);
        }

        transform.position = new Vector3(transform.position.x,
                                         transform.position.y,
                                         lerp);
    }
}
