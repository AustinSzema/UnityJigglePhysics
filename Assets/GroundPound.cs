using System;
using UnityEngine;

public class GroundPound : MonoBehaviour
{
    [SerializeField] private ParticleSystem groundPoundParticles;
    [SerializeField] private Transform playerFootPosition;

    private void Start()
    {
        transform.parent = null;
    }

    public void PlayParticles()
    {
        transform.position = playerFootPosition.position;
        groundPoundParticles.Play();
    }
}
