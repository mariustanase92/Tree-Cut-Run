using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Const.TAG_PLAYER))
        {
            Debug.Log("Player");
        }
           // Destroy(this.gameObject);
    }
}
