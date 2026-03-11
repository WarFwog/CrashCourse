using UnityEngine;
using TMPro;

public class RaceUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI timerText;

    private float _raceTime;

    private void Update()
    {
        UpdateTimer();
        UpdateSpeed();
    }

    private void UpdateTimer()
    {
        _raceTime += Time.deltaTime;

        var minutes = Mathf.FloorToInt(_raceTime / 60f);
        var seconds = Mathf.FloorToInt(_raceTime % 60f);
        var milliseconds = Mathf.FloorToInt((_raceTime * 1000f) % 1000f);

        timerText.text = $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }

    private void UpdateSpeed()
    {
        var speed = player.CurrentSpeed();
        speedText.text = $"Speed: {speed:0.0}";
    }
}