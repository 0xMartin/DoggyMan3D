using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ColliderGizmoDrawer : MonoBehaviour
{
    private BoxCollider boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void OnDrawGizmos()
    {
        if (boxCollider == null) return;

        // Nastavte barvu Gizma
        Gizmos.color = Color.green;

        Matrix4x4 oldMatrix = Gizmos.matrix;

        // Aplikujte transformace objektu (pozici, rotaci a měřítko) na Gizmos
        Gizmos.matrix = Matrix4x4.TRS(
            boxCollider.transform.TransformPoint(boxCollider.center), 
            boxCollider.transform.rotation, 
            boxCollider.transform.lossyScale
        );

        // Vykreslení boxu reprezentujícího collider
        Gizmos.DrawWireCube(Vector3.zero, boxCollider.size);

        // Resetujte Gizmos matrix zpět na původní
        Gizmos.matrix = oldMatrix;
    }
}