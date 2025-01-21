using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public float initialVelocity = 10f;
    public float height = 5f;
    public float curve = 0.5f;

    public void SpawnCube(Vector3 spawnPosition)
    {
        GameObject cube = Instantiate(cubePrefab, spawnPosition, Quaternion.identity);
        Rigidbody rb = cube.GetComponent<Rigidbody>();
        
    }
}