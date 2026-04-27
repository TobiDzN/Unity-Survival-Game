using UnityEngine;
using UnityEngine.AI;

public class WolfHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    [Header("Animation")]
    public string hurtTrigger = "Hurt";
    public string dieTrigger = "Die";
    public float hurtCooldown = 0.4f;

    bool dead;
    float lastHurtTime;

    Animator anim;
    public PlayerInventory playerInventory;
    public MaterialsData meat;

    void Awake()
    {
        currentHP = maxHP;
        anim = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int dmg)
    {
        if (dead) return;

        currentHP -= dmg;
        Debug.Log($"{name} took {dmg} damage. HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
            return;
        }

        if (anim && Time.time - lastHurtTime >= hurtCooldown)
        {
            lastHurtTime = Time.time;
            anim.SetTrigger(hurtTrigger);
        }
    }

    void Die()
    {
        dead = true;
        Debug.Log($"{name} died.");

        // Stop movement/AI
        var agent = GetComponent<NavMeshAgent>();
        if (agent) agent.enabled = false;

        var wolfAI = GetComponent<WolfAI>();
        if (wolfAI) wolfAI.enabled = false;

        // Disable colliders
        foreach (var col in GetComponentsInChildren<Collider>())
            col.enabled = false;

        // Play death animation
        if (anim) anim.SetTrigger(dieTrigger);

        playerInventory.AddItem(meat, 1);
        // Destroy after a short delay
        Destroy(gameObject, 4f);
    }
}
