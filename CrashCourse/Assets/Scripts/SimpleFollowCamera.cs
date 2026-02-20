using UnityEngine;

public class SimpleFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -7f);
    [SerializeField] private float followSpeed = 10f;

    private void LateUpdate()
    {
        if (target == null) return;

        var desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );
    }
}