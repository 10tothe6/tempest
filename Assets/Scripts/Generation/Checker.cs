using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    public Transform guide;

    public int offset;
    public int chunkSize;

    private float posX;

    void Update()
    {
        if (guide != null)
        {
            posX = Mathf.RoundToInt(guide.position.x / chunkSize) * chunkSize;

            transform.position = new Vector3(posX + (offset * chunkSize), 0, guide.position.z);
        }
    }
}
