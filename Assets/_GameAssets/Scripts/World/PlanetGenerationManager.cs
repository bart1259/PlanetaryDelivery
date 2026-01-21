
using UnityEngine;

// Monobehaviour attached to planet
public class PlanetGenerationManager : MonoBehaviour
{
    public int resolution;
    public float radius;
    public Texture2D planetTexture;

    public Material planetMaterial;
    public HeightMapEvaluatorSO heightMapEvaluatorSO;
    public LayerMask planetMask;
    public bool generateCollider = true;

    public void Clear()
    {
        // Delete all children
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
    public void GeneratePlanetMesh()
    {
        Clear();

        Mesh topMesh = MeshUtils.GenerateProjectedSphereMesh(resolution, radius, Vector3.up, planetTexture, heightMapEvaluatorSO);
        GameObject topFace = MeshUtils.CreateMeshObject(topMesh, planetMaterial, generateCollider);
        topFace.layer = (int) Mathf.Log(planetMask.value, 2);
        topFace.transform.SetParent(transform);
        topFace.name = "Face - Top";
        topFace.transform.localPosition = Vector3.up * radius;
    
        Mesh bottomMesh = MeshUtils.GenerateProjectedSphereMesh(resolution, radius, Vector3.down, planetTexture, heightMapEvaluatorSO);
        GameObject bottomFace = MeshUtils.CreateMeshObject(bottomMesh, planetMaterial, generateCollider);
        bottomFace.layer = (int) Mathf.Log(planetMask.value, 2);
        bottomFace.transform.SetParent(transform);
        bottomFace.name = "Face - Bottom";
        bottomFace.transform.localPosition = Vector3.down * radius;

        Mesh leftMesh = MeshUtils.GenerateProjectedSphereMesh(resolution, radius, Vector3.left, planetTexture, heightMapEvaluatorSO);
        GameObject leftFace = MeshUtils.CreateMeshObject(leftMesh, planetMaterial, generateCollider);
        leftFace.layer = (int) Mathf.Log(planetMask.value, 2);
        leftFace.transform.SetParent(transform);
        leftFace.name = "Face - Left";
        leftFace.transform.localPosition = Vector3.left * radius;

        Mesh rightMesh = MeshUtils.GenerateProjectedSphereMesh(resolution, radius, Vector3.right, planetTexture, heightMapEvaluatorSO);
        GameObject rightFace = MeshUtils.CreateMeshObject(rightMesh, planetMaterial, generateCollider);
        rightFace.layer = (int) Mathf.Log(planetMask.value, 2);
        rightFace.transform.SetParent(transform);
        rightFace.name = "Face - Right";
        rightFace.transform.localPosition = Vector3.right * radius;

        Mesh frontMesh = MeshUtils.GenerateProjectedSphereMesh(resolution, radius, Vector3.forward, planetTexture, heightMapEvaluatorSO);
        GameObject frontFace = MeshUtils.CreateMeshObject(frontMesh, planetMaterial, generateCollider);
        frontFace.layer = (int) Mathf.Log(planetMask.value, 2);
        frontFace.transform.SetParent(transform);
        frontFace.name = "Face - Front";
        frontFace.transform.localPosition = Vector3.forward * radius;

        Mesh backMesh = MeshUtils.GenerateProjectedSphereMesh(resolution, radius, Vector3.back, planetTexture, heightMapEvaluatorSO);
        GameObject backFace = MeshUtils.CreateMeshObject(backMesh, planetMaterial, generateCollider);
        backFace.layer = (int) Mathf.Log(planetMask.value, 2);
        backFace.transform.SetParent(transform);
        backFace.name = "Face - Back";
        backFace.transform.localPosition = Vector3.back * radius;
    }

    public Vector3 GetClosestRoadToPoint(Vector3 point)
    {
        // In an expanding circle find the closest road point (red on texture)
        float searchRadius = 0.5f;
        int searchSteps = 16;
        while (searchRadius < 100.0f)
        {
            for (int i = 0; i < searchSteps; i++)
            {
                float angle = (float)i / (float)searchSteps * Mathf.PI * 2.0f;
                Vector3 offset = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle)) * searchRadius;
                offset = Quaternion.FromToRotation(Vector3.up, point.normalized) * offset;
                Vector3 samplePoint = (point + offset).normalized;
                float height = heightMapEvaluatorSO.Evaluate(samplePoint);
                if (heightMapEvaluatorSO.IsRoadAtPoint(samplePoint))
                {
                    return samplePoint * (radius + height);
                }
            }
            searchRadius += 0.2f;
        }

        return Vector3.zero;
    }
}
