using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPositioner : MonoBehaviour
{
    public float groundOffset = 1.0f; 
    public LayerMask groundLayer;    

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, groundLayer))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + groundOffset, transform.position.z);
        }
    }
}
