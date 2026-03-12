using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Transform posA, posB;
    public float speed = 2f;

    Vector3 target;

    void Start()
    {
        target = posB.position;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, posA.position) < 0.1f)
            target = posB.position;

        if (Vector3.Distance(transform.position, posB.position) < 0.1f)
            target = posA.position;

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}