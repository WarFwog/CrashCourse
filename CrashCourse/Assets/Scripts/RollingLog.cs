using UnityEngine;

public class RollingLog : MonoBehaviour
{
    [SerializeField] private float lifeTime = 10f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}