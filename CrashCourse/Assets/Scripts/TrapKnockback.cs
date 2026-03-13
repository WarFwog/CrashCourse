using System.Runtime.CompilerServices;
using UnityEngine;

public class TrapKnockback : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 15f;
    [SerializeField] private float knockbackDuration = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<PlayerController>();

        if (player != null)
            player.ApplyKnockback(transform.forward, knockbackForce, knockbackDuration);
    }
}