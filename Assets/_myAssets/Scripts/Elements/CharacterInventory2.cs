using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterInventory2 : MonoBehaviour
{
    [SerializeField]
    private Transform boneT;

    private int currentHairIndex = 0;
    private float defaultHairScale = 2f;
    private float deltaHairScale = .45f;

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
        boneT.DOKill();
        boneT.transform.localScale = Vector3.one * (deltaHairScale * currentHairIndex + defaultHairScale);

        currentHairIndex++;
        boneT.DOScale(boneT.transform.localScale + Vector3.one * deltaHairScale, .5f);
    }

    private void DecreaseHair()
    {
        if (currentHairIndex == 0)
        {
            return;
        }
        boneT.DOKill();
        boneT.transform.localScale = Vector3.one * (deltaHairScale * currentHairIndex + defaultHairScale);
        currentHairIndex--;

        boneT.DOScale(boneT.transform.localScale - Vector3.one * deltaHairScale, .5f);
    }

    private void ResetHair()
    {
        currentHairIndex = 0;
        boneT.DOKill();
        boneT.transform.localScale = Vector3.one * defaultHairScale;
    }
}
