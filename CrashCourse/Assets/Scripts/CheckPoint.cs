using UnityEngine;

public class CheckPoint : MonoBehaviour
{

    public BoxCollider trigger;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            RespawnController.instance.respawnPoint = transform;
            trigger.enabled = false;

        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
