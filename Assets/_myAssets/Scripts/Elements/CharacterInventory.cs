using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInventory : MonoBehaviour
{
    [SerializeField]
    private List<Transform> boneList;

    private BoingKit.BoingBones bonesManager;
    private int currentHairIndex = 0;

    private void Awake()
    {
        bonesManager = GetComponent<BoingKit.BoingBones>();
    }

    private void OnEnable()
    {
        ResetHair();
        BusSystem.OnNewLevelLoad += ResetHair;
        BusSystem.OnItemCollect += HandleNewItem;
    }

    private void OnDisable()
    {
        BusSystem.OnNewLevelLoad -= ResetHair;
        BusSystem.OnItemCollect -= HandleNewItem;
    }

    private void HandleNewItem(CollectibleData itemData)
    {
        switch (itemData.Type)
        {
            case CollectibleType.Good:
                IncreaseHair();
                break;
            case CollectibleType.Bad:
                DecreaseHair();
                break;
            default:
                break;
        }
    }

    private void IncreaseHair()
    {
        if (currentHairIndex >= boneList.Count) return;
        currentHairIndex++;

        bonesManager.BoneChains[0].Exclusion = new Transform[1] { boneList[currentHairIndex - 1] };

        boneList[currentHairIndex - 1].localScale = Vector3.one;
        bonesManager.RescanBoneChains();
    }

    private void DecreaseHair()
    {
        if (currentHairIndex == 0)
        {
            return;
        }
        currentHairIndex--;

        bonesManager.BoneChains[0].Exclusion = new Transform[1] { boneList[currentHairIndex] };

        boneList[currentHairIndex].localScale = Vector3.zero;
        bonesManager.RescanBoneChains();
    }

    private void ResetHair()
    {
        currentHairIndex = 0;

        bonesManager.BoneChains[0].Exclusion = new Transform[1] { boneList[0].parent };
        foreach (var item in boneList)
        {
            item.localScale = Vector3.zero;
        }
        bonesManager.RescanBoneChains();
    }

}
