using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    public float rotationSpeed = 1f;
    public bool GenerateRandomRotationDir = true;
    public Vector3 RotationDir;

    private void Start()
    {
        if (GenerateRandomRotationDir)
            RotationDir = Random.insideUnitSphere;
    }

    private void Update()
    {
        transform.Rotate(RotationDir * rotationSpeed * Time.deltaTime);
    }
}
