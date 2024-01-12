using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteLater : MonoBehaviour
{
    public float DeleteAfterTime = 2.0f;

    void Start()
    {
        StartCoroutine(DeleteLaterAsync());
    }

    IEnumerator DeleteLaterAsync()
    {
        yield return new WaitForSeconds(DeleteAfterTime);
        Destroy(gameObject);
    }
}
