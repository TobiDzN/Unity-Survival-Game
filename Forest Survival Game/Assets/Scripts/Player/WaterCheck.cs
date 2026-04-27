using UnityEngine;

public class WaterCheck : MonoBehaviour
{
    [Header("Setup")]
    public LayerMask waterLayer;
    public float radius = 0.7f;

    [Header("Result")]
    public bool isInWater;

    void Update()
    {
        isInWater = Physics.CheckSphere(transform.position, radius, waterLayer, QueryTriggerInteraction.Collide);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
