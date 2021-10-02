using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BusSystem 
{
    //Game main actions
    public static Action OnNewLevelStart;
    public static void CallNewLevelStart() { OnNewLevelStart?.Invoke(); }
    public static Action<bool> OnLevelDone;
    public static void CallLevelDone(bool status) { OnLevelDone?.Invoke(status); }
    public static Action OnNewLevelLoad;
    public static void CallNewLevelLoad() { OnNewLevelLoad?.Invoke(); }
    public static Action<CollectibleData> OnItemCollect;
    public static void CallItemCollect(CollectibleData data) { OnItemCollect?.Invoke(data); }

    //Gameplay actions
    public static Action OnPhaseOneEnd;
    public static void CallPhaseOneEnd() { OnPhaseOneEnd?.Invoke(); }
    public static Action<bool> OnCanCut;
    public static void CallCanCut(bool canCut) { OnCanCut?.Invoke(canCut); }
    public static Action OnTreeHit;
    public static void CallTreeHit() { OnTreeHit?.Invoke(); }
    public static Action OnTreeChopped;
    public static void CallTreeChopped() { OnTreeChopped?.Invoke(); }
    public static Action OnPerfectRound;
    public static void CallPerfectRound() { OnPerfectRound?.Invoke(); }

    //Polish


    //UI
    public static Action<int> OnUpdateCoins;
    public static void CallUpdateCoins(int value) { OnUpdateCoins?.Invoke(value); }
    public static Action<int> OnAddCash;
    public static void CallAddCash(int value) { OnAddCash?.Invoke(value); }

    //Audio
    public static Action<SoundEffects> OnSoundPlay;
    public static void CallSoundPlay(SoundEffects eff) { OnSoundPlay?.Invoke(eff); }
}
