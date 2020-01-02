using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cam"))
        {
            collision.gameObject.GetComponent<Snapshot>().ResetPos();
        }
    }
}
