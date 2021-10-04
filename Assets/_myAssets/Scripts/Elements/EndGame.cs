using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> _confetti = new List<ParticleSystem>(); 

    private void OnEnable()
    {
        BusSystem.OnLevelDone += PlayConfetti;
    }

    private void OnDisable()
    {
        BusSystem.OnLevelDone -= PlayConfetti;
    }

    void PlayConfetti(bool won)
    {
        if(won)
        {
            foreach (ParticleSystem particle in _confetti)
            {
                if(particle != null)
                    particle.Play();
            }
        }
    }
}
