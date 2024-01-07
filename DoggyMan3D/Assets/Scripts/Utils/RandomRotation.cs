using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    public float rotationSpeed = 1f;

    private Vector3 dir;

    private void Start()
    {
        dir = Random.insideUnitSphere; ;
    }

    private void Update()
    {
        transform.Rotate(dir * rotationSpeed * Time.deltaTime);
    }
}
