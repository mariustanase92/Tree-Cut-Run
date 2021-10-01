using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Vector3 firstWaveTarget;

    public void InitEndLevel(RectTransform whereTo)
    {
        firstWaveTarget = new Vector3(Random.Range(-200, 200) + transform.position.x, Random.Range(-200, 200) + transform.position.y);

        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(transform.DOMove(firstWaveTarget,1f).SetEase(Ease.OutSine));
        mySequence.Append(transform.DOMove(whereTo.position, 1f).SetEase(Ease.InCubic));
        mySequence.PrependInterval(Random.Range(0, 0.5f));
        mySequence.OnComplete(() => {
            //ShopManager.Instance.AddCoins(10);
            //BusSystem.CallUpdateCoinUI();
            Destroy(gameObject);
            
        });
    }


}
