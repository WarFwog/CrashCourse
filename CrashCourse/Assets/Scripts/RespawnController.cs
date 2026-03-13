using UnityEngine;

public class RespawnController : MonoBehaviour
{
    public static RespawnController instance;
    public Transform respawnPoint;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
       else
        {
           
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            RespawnPlayer(collision.gameObject);
        }
    }

    public void RespawnPlayer(GameObject player)
    {
        CharacterController controller = player.GetComponent<CharacterController>();
        PlayerController movement = player.GetComponent<PlayerController>();

        if (controller != null)
            controller.enabled = false;

        player.transform.position = respawnPoint.position;

        if (movement != null)
            movement.ResetMovement();

        if (controller != null)
            controller.enabled = true;
    }
}
