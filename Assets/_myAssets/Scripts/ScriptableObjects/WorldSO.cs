using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "ScriptableObjects/WorldDataSO", order = 2)]
public class WorldSO : ScriptableObject
{
    [Header("Default Level")]
    [Tooltip("Make sure to DISABLE prefs")]
    [Range(0, 9)] public int currentLevel = 0;

    [Header("Coins")]
    [Tooltip("How much cash you get by simply completing Phase 1")]
    [Range(10, 100)] public int phase1Reward = 50;
    [Tooltip("This is multiplied with HP left at the end of Phase 2")]
    [Range(10, 100)] public int phase2Multiplier = 30;
    [Range(0, 999)] public int cash = 0;

    [Header("Gravity")]
    public Vector3 worldGravity = new Vector3(0, -35.0F, 0);

    [Header("Vibration")]
    [Tooltip("Toggle Device RUMBLE")]
    public bool canVibrate;
}
