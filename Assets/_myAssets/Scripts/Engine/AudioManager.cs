using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip _chainSaw;
    public AudioClip[] effects;
    public AudioSource source;


    private void OnEnable()
    {
        BusSystem.OnSoundPlay += PlaySound;
    }

    private void OnDisable()
    {
        BusSystem.OnSoundPlay -= PlaySound;
    }

    private void PlaySound(SoundEffects obj)
    {
        source.pitch = 1f;
        
        switch (obj)
        {
            case SoundEffects.Chainsaw:
                source.Stop();
                source.clip = _chainSaw;
                source.pitch = 1f;
                source.Play();
                break;
            case SoundEffects.Checkpoint:
                source.pitch = 2;
                source.PlayOneShot(effects[1]);
                break;
            case SoundEffects.Hit1:
                source.Stop();
                source.PlayOneShot(effects[2]);
                break;
            case SoundEffects.Hit2:
                source.Stop();
                source.PlayOneShot(effects[3]);
                break;
            case SoundEffects.Hit3:
                source.Stop();
                source.PlayOneShot(effects[4]);
                break;
            case SoundEffects.Lose:
                source.PlayOneShot(effects[5]);
                break;
            case SoundEffects.PickChainsaw:
                source.PlayOneShot(effects[6]);
                break;
            case SoundEffects.PickIngot:
                source.pitch = UnityEngine.Random.Range(.5f, 1.5f);
                source.PlayOneShot(effects[7]);
                break;
            case SoundEffects.TreeFall1:
                source.PlayOneShot(effects[8]);
                break;
            case SoundEffects.TreeFall2:
                source.PlayOneShot(effects[9]);
                break;
            case SoundEffects.TreeFall3:
                source.PlayOneShot(effects[10]);
                break;
            case SoundEffects.Win:
                source.Stop();
                source.PlayOneShot(effects[11]);
                break;
        }
    }
}

public enum SoundEffects
{
    Chainsaw,
    Checkpoint,
    Hit1,
    Hit2,
    Hit3,
    Lose,
    PickChainsaw,
    PickIngot,
    TreeFall1,
    TreeFall2,
    TreeFall3,
    Win   
}