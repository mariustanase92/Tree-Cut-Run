using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterDataSO", order = 1)]
public class CharacterDataSO : ScriptableObject
{
    [Range(0, 50)] public float moveSpeed = 6;
    [Range(0, 50)] public float steerSpeed = 3;
    [Range(0, 50)] public int startHP = 2;
}
