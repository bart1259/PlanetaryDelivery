using System.Collections.Generic;
using UnityEngine;

public class MeshUtils
{
    private struct Meshlet
    {
        public List<Vector3> vertices;
        public List<int> triangleIndices;
        public List<Vector2> uvs;
    
        public Mesh ToMesh()
        {
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangleIndices, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            return mesh;
        }

        public static Meshlet Combine(List<Meshlet> meshlets)
        {
            List<Vector3> combinedVertices = new List<Vector3>();
            List<int> combinedTriangleIndices = new List<int>();
            List<Vector2> combinedUVs = new List<Vector2>();

            int vertexOffset = 0;
            foreach (var meshlet in meshlets)
            {
                combinedVertices.AddRange(meshlet.vertices);
                combinedUVs.AddRange(meshlet.uvs);

                foreach (var index in meshlet.triangleIndices)
                {
                    combinedTriangleIndices.Add(index + vertexOffset);
                }

                vertexOffset += meshlet.vertices.Count;
            }

            return new Meshlet
            {
                vertices = combinedVertices,
                triangleIndices = combinedTriangleIndices,
                uvs = combinedUVs
            };
        }
    }

    private static Vector2 GetUVForVertex(Vector3 normal, Vector2 localUV)
    {
        localUV = new Vector2(1.0f - localUV.x, localUV.y);
        localUV /= 4.0f;
        if (normal == Vector3.forward)
            localUV += new Vector2(0.25f, 0.25f);
        else if (normal == Vector3.right)
            localUV += new Vector2(0.0f, 0.25f);
        else if (normal == Vector3.left)
            localUV += new Vector2(0.5f, 0.25f);
        else if (normal == Vector3.up)
            localUV += new Vector2(0.25f, 0.5f);
        else if (normal == Vector3.down)
            localUV += new Vector2(0.25f, 0.0f);
        else if (normal == Vector3.back)
            localUV += new Vector2(0.75f, 0.25f);

        return localUV;
    }


    // Resolution is the number of verticies
    private static Meshlet GeneratePlaneMeshlet(int resolution, float size, Vector3 normal)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangleIndices = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        Quaternion rotation;
        if (normal == Vector3.forward)
            rotation = Quaternion.identity;
        else if (normal == Vector3.right)
            rotation = Quaternion.Euler(0, 90, 0);
        else if (normal == Vector3.left)
            rotation = Quaternion.Euler(0, -90, 0);
        else if (normal == Vector3.up)
            rotation = Quaternion.Euler(-90, 0, 0);
        else if (normal == Vector3.down)
            rotation = Quaternion.Euler(90, 0, 0);
        else if (normal == Vector3.back)
            rotation = Quaternion.Euler(0, 180, 0);
        else
            rotation = Quaternion.FromToRotation(Vector3.forward, normal);

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float xPos = ((float)x / (resolution - 1) - 0.5f) * size;
                float yPos = ((float)y / (resolution - 1) - 0.5f) * size;

                Vector3 vertex = new Vector3(xPos, yPos, 0);
                vertex = rotation * vertex;
                vertices.Add(vertex);

                Vector2 uv = new Vector2((float)x / (resolution - 1), (float)y / (resolution - 1));
                uv = GetUVForVertex(normal, uv);
                uvs.Add(uv);

                if (x < resolution - 1 && y < resolution - 1)
                {
                    int topLeft = y * resolution + x;
                    int bottomLeft = (y + 1) * resolution + x;
                    int topRight = y * resolution + (x + 1);
                    int bottomRight = (y + 1) * resolution + (x + 1);

                    triangleIndices.Add(bottomLeft);
                    triangleIndices.Add(topLeft);
                    triangleIndices.Add(topRight);

                    triangleIndices.Add(bottomLeft);
                    triangleIndices.Add(topRight);
                    triangleIndices.Add(bottomRight);
                }
            }
        }

        return new Meshlet
        {
            vertices = vertices,
            triangleIndices = triangleIndices,
            uvs = uvs
        };
    }

    private static Meshlet ProjectOntoSphere(Meshlet meshlet, Vector3 sphereCenter, float sphereRadius, Texture2D offsetTexture = null, HeightMapEvaluatorSO heightMapEvaluatorSO = null)
    {
        List<Vector3> projectedVertices = new List<Vector3>();


        for (int i = 0; i < meshlet.vertices.Count; i++)
        {
            Vector3 vertex = meshlet.vertices[i];
            Vector2 uv = meshlet.uvs[i];

            Vector3 direction = (vertex - sphereCenter).normalized;

            float offset = 0.0f;
            if (offsetTexture != null)
            {
                Color offsetColor = offsetTexture.GetPixelBilinear(uv.x, uv.y);
                offset = heightMapEvaluatorSO.Evaluate(direction);
            }
            Vector3 projectedVertex = sphereCenter + direction * (sphereRadius + offset);
            projectedVertices.Add(projectedVertex);
        }

        return new Meshlet
        {
            vertices = projectedVertices,
            triangleIndices = meshlet.triangleIndices,
            uvs = meshlet.uvs
        };
    }

    public static Mesh GeneratePlaneMesh(int resolution, float size, Vector3 normal)
    {
        Meshlet meshlet = GeneratePlaneMeshlet(resolution, size, normal);
        return meshlet.ToMesh();
    }

    public static Mesh GenerateProjectedSphereMesh(int resolution, float sphereRadius, Vector3 normal, Texture2D offsetTexture = null, HeightMapEvaluatorSO heightMapEvaluatorSO = null)
    {
        Meshlet planeMeshlet = GeneratePlaneMeshlet(resolution, sphereRadius * 2, normal);
        Meshlet projectedMeshlet = ProjectOntoSphere(planeMeshlet, -normal * sphereRadius, sphereRadius, offsetTexture, heightMapEvaluatorSO);
        return projectedMeshlet.ToMesh();
    }

    public static GameObject CreateMeshObject(Mesh mesh, Material material, bool addCollider=false)
    {
        GameObject go = new GameObject();
        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        if (addCollider)
        {
            MeshCollider meshCollider = go.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }
        return go;
    }


}