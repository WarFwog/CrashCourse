using UnityEngine;

public class LogSpawner : MonoBehaviour
{
    [SerializeField] private GameObject logPrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnHeight = 1.5f;

    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (!(_timer >= spawnInterval)) return;
        SpawnLog();
        _timer = 0f;
    }

    private void SpawnLog()
    {
        var spawnPos = transform.position + Vector3.up * spawnHeight;

        Instantiate(logPrefab, spawnPos, transform.rotation);
    }
}