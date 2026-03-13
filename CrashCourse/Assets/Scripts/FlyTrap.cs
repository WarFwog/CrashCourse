using UnityEngine;
using System.Collections;

public class FlyTrap : MonoBehaviour
{

    public Animator animator;
    public float respawnDelay = 1.5f;

    private bool triggered = false;

    private void OnTriggerEnter(Collider collision)
    {
        if (triggered) return;
        if (collision.CompareTag("Player"))
        {
            triggered = true;
            PlayerController movement = collision.GetComponent<PlayerController>();
            animator.SetTrigger("Close");
            StartCoroutine(RespawnAfterDelay(collision.gameObject));
        }
    }

    IEnumerator RespawnAfterDelay(GameObject Player)
    {
        yield return new WaitForSeconds(respawnDelay);
        RespawnController.instance.RespawnPlayer(Player);
        triggered = false;

    }
 
}
