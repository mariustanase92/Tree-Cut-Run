using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] int _recoverHP = 1;
    [SerializeField] GameObject _glowFX;

    private void OnEnable()
    {
       BusSystem.OnNewLevelLoad += EnableMesh;
    }

    private void OnDisable()
    {
       BusSystem.OnNewLevelLoad -= EnableMesh;
    }

    private void Start()
    {
        EnableMesh();
    }

    void EnableMesh()
    {
       GetComponentInChildren<MeshRenderer>().enabled = true;
       GetComponent<BoxCollider>().enabled = true;

        if (_glowFX != null)
            _glowFX.SetActive(true);

        foreach (Transform child in transform)
        {
            if (child != null)
                child.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void DisableMesh()
    {
        if (GameManager.Instance.canVibrate)
            Vibration.VibratePop();

        GetComponentInChildren<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;

        foreach (Transform child in transform)
        {
            if (child != null)
                child.GetComponent<MeshRenderer>().enabled = false;
        }

        if(_glowFX != null)
         _glowFX.SetActive(false);
    }

    public int GetRecoverHP()
    {
        return _recoverHP;
    }
}
