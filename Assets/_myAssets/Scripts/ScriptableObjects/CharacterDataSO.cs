using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterDataSO", order = 1)]
public class CharacterDataSO : ScriptableObject
{
    public float laneLimit = 5.5f;

    //Speed
    public float moveSpeed = 6;

    public float steerSpeed = 3;
}
