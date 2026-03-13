using UnityEngine;

public class TrapLogKnockback : TrapKnockback
{
    [SerializeField] private Vector3 worldDirection = Vector3.back; 

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(worldDirection);
    }
}