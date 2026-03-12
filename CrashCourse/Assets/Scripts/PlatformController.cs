using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Transform posA, posB;
    public float Speed = 2f;

    Vector3 targetPos;

    void Start()
    {
        targetPos = posB.position;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, posA.position) < 0.1f)
            targetPos = posB.position;

        if (Vector3.Distance(transform.position, posB.position) < 0.1f)
            targetPos = posA.position;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
            collision.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
            collision.transform.SetParent(null);
    }
}
