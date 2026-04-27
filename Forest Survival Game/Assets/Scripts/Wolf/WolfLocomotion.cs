using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WolfLocomotion : MonoBehaviour
{
    public Animator anim;
    public float maxRunSpeed = 8f;

    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!anim) anim = GetComponentInChildren<Animator>(true);
    }

    void Update()
    {
        float v = agent.velocity.magnitude;

        float speed01 = Mathf.Clamp01(v / maxRunSpeed);

        anim.SetFloat("Speed", speed01);
    }
}
