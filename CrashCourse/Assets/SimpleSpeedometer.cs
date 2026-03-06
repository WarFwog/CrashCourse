using UnityEngine;
using TMPro;

public class SimpleSpeedometer : MonoBehaviour
{
    [Header("Setup")]
    public Transform player;     // Sleep je player/voertuig hierin (met Rigidbody)
    public bool showKmh = true;  // true = km/h, false = m/s

    private Rigidbody rb;
    private TextMeshProUGUI speedText;

    void Start()
    {
        rb = player.GetComponent<Rigidbody>();
        speedText = GetComponent<TextMeshProUGUI>();
        if (rb == null) Debug.LogError("Player heeft geen Rigidbody!");
        if (speedText == null) Debug.LogError("Script op TextMeshProUGUI zetten!");
    }

    void Update()
    {
        if (rb == null) return;

        // Snelheid berekenen (total velocity)
        float speed = rb.linearVelocity.magnitude;
        if (showKmh) speed *= 3.6f;  // m/s -> km/h

        // Update tekst (afgerond)
        speedText.text = Mathf.RoundToInt(speed).ToString() + (showKmh ? " km/h" : " m/s");
    }
}
