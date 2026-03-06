using Unity.VisualScripting;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
    public static RespawnController instance;
    public Transform respawnPoint;


    private void Awake()
    {
        instance = this; 
    }
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.transform.position = respawnPoint.position;
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
