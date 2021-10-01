using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public CollectibleData data;

    [SerializeField]
    private CollectibleType type;

    [SerializeField]
    private bool isMoving = false;
    [SerializeField]
    private float moveTo;

    private void Awake()
    {
        data = new CollectibleData(type);
        if (isMoving)
            transform.DOLocalMoveX(moveTo, 6f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    public void Collect()
    {
        BusSystem.CallItemCollect(data);
        PopDestroy();
    }

    private void PopDestroy()
    {
        GetComponent<Collider>().enabled = false;
        transform.DOScale(Vector3.zero, .5f).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
    }
}

public class CollectibleData
{
    public CollectibleData(CollectibleType _type)
    {
        Type = _type;
    }

    public CollectibleData(CollectibleData _clone)
    {
        Type = _clone.Type;
    }

    public CollectibleType Type { get; private set; }
}

public enum CollectibleType
{
    Good,
    Bad,
    Ingot,
    Chainsaw
}
