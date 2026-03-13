using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            RespawnController.instance.RespawnPlayer(collision.gameObject);
        }
    }

}
