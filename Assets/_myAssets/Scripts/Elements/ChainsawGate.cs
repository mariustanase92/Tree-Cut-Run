using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainsawGate : MonoBehaviour
{
    List<GameObject> _disableGOs = new List<GameObject>();
    private void OnEnable()
    {
        GameObject ChainsawModel = transform.parent.Find("FuelForChainsaw").gameObject;
        GameObject TransparentCircle = transform.parent.Find("Sphere").gameObject;
        _disableGOs.Add(ChainsawModel);
        _disableGOs.Add(TransparentCircle);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Const.TAG_PLAYER))
        {
            foreach (GameObject go in _disableGOs)
            {
                if (go != null)
                    go.SetActive(false);
            }
        }
    }
}
