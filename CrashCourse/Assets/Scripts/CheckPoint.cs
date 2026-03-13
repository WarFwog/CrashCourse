using UnityEngine;

public class CheckPoint : MonoBehaviour
{

    public BoxCollider trigger;
    public GameObject redFlag;
    public GameObject greenFlag;

    private bool activated = false;

    private void OnTriggerEnter(Collider collision)
    {

        if (activated) return;

        if (collision.CompareTag("Player"))
        {
            RespawnController.instance.respawnPoint = transform;
            trigger.enabled = false;
            redFlag.SetActive(false);
            greenFlag.SetActive(true);
            Debug.Log("Check");

        }

       

        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        greenFlag.SetActive(false);
        redFlag.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
