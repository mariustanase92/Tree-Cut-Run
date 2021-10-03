﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deadzone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Const.TAG_PLAYER))
            BusSystem.CallLevelDone(false);
        else if (other.CompareTag(Const.TAG_TREEBODY))
        {
            Debug.Log(other.gameObject);
            Destroy(other.gameObject);
        }
    }
}
