using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionParticleEffect : MonoBehaviour
{
    public ParticleSystem particleEffect;
    public GameObject particlePrefab;

    private void Start()
    {
        if (particleEffect == null)
        {
            particleEffect = GetComponentInChildren<ParticleSystem>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("RespawnTrigger"))
        {
            PlayParticleEffect(collision.contacts[0].point);
        }
    }

    private void PlayParticleEffect(Vector3 position)
    {
        if (particleEffect != null)
        {
            particleEffect.transform.position = position;
            particleEffect.Play();
        }
        else
        {
            Debug.LogError("Geen Particle System gevonden op " + gameObject.name);
        }

        if (particlePrefab != null)
        {
            GameObject effect = Instantiate(particlePrefab, position, Quaternion.identity);
            
        }
    }



} 