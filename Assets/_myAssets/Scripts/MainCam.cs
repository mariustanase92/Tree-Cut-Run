using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainCam : MonoBehaviour
{
    [SerializeField] DOTweenAnimation _tween;
    bool _canShake = false;
    private void OnEnable()
    {
        BusSystem.OnNewLevelStart += EnableShaking;
        BusSystem.OnLevelDone += StopShaking;
        BusSystem.OnTreeChopped += CameraShake;
    }

    private void OnDisable()
    {
        BusSystem.OnNewLevelStart -= EnableShaking;
        BusSystem.OnLevelDone -= StopShaking;
        BusSystem.OnTreeChopped -= CameraShake;
    }

    void CameraShake()
    {
        if (_canShake)
            _tween.DORestart();
        else
            _tween.DOPause();
    }

    void EnableShaking()
    {
        _canShake = true;
    }

    void StopShaking(bool shake)
    {
        _canShake = false;
        _tween.DOPause();
       
    }
}
