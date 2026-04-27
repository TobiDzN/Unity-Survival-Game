using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WolfAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Speeds")]
    public float walkSpeed = 1.8f;
    public float runSpeed = 8f;

    [Header("Wander Area")]
    public float wanderRadius = 30f;

    [Header("Timers")]
    public Vector2 moveTimeRange = new Vector2(6f, 12f);
    public Vector2 waitTimeRange = new Vector2(2f, 5f);

    [Header("Attack Lock")]
    public float attackLockTime = 0.7f;
    float attackLockTimer = 0f;

    [Header("Detection")]
    public float detectionRange = 25f;
    public float loseRange = 35f;

    [Header("Howl Before Chase")]
    public float howlDuration = 3.958f;

    [Header("Attack")]
    public float attackRange = 2.2f;
    public float attackStartBuffer = 0.4f;
    public float attackCooldown = 1.2f;

    [Header("Animator Triggers")]
    public string howlTrigger = "Howl";
    public string attackLTrigger = "AttackL";
    public string attackRTrigger = "AttackR";

    public Animator anim;
    public PlayerHealth playerHealth;

    enum State { Wander, Alert, Chase, Attack }
    State state = State.Wander;

    NavMeshAgent agent;

    // Wander
    float timer;
    bool waiting;
    float moveTime;
    float waitTime;

    // Alert lock
    bool hasHowled;
    float alertTimer;

    // Attack
    float attackTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!anim) anim = GetComponentInChildren<Animator>(true);
        FindPlayerIfNeeded();
    }

    void Start()
    {
        moveTime = Random.Range(moveTimeRange.x, moveTimeRange.y);
        PickNewDestination();
    }

    void Update()
    {
        FindPlayerIfNeeded();

        float dist = player ? Vector3.Distance(transform.position, player.position) : Mathf.Infinity;

        // If we are in an attack lock, stay attacking
        if (attackLockTimer > 0f)
        {
            state = State.Attack;
            DoAttack();
            return;
        }

        // If in the middle of howling, stay alert until finished
        if (state == State.Alert)
        {
            DoAlert();
            return;
        }

        // Reset if player got far away
        if (dist >= loseRange)
        {
            state = State.Wander;
            hasHowled = false;
        }
        else
        {
            // Prefer remainingDistance if chasing and path exists
            float closeDist = GetCloseDistance(dist);

            if (closeDist <= attackRange + attackStartBuffer)
            {
                attackLockTimer = attackLockTime;
                attackTimer = 0f;
                state = State.Attack;
            }
            else if (dist <= detectionRange)
            {
                state = hasHowled ? State.Chase : State.Alert;
            }
            else
            {
                state = State.Wander;
            }
        }

        switch (state)
        {
            case State.Wander: DoWander(); break;
            case State.Alert: DoAlert(); break;
            case State.Chase: DoChase(); break;
            case State.Attack: DoAttack(); break;
        }
    }

    float GetCloseDistance(float fallbackWorldDist)
    {
        // If agent has a valid path, remainingDistance is the best path
        if (agent != null && agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            if (!float.IsInfinity(agent.remainingDistance))
                return agent.remainingDistance;
        }
        return fallbackWorldDist;
    }

    void DoWander()
    {
        agent.isStopped = false;
        agent.speed = walkSpeed;
        agent.stoppingDistance = 0f;
        attackTimer = 0f;

        if (!waiting)
        {
            timer += Time.deltaTime;

            if (timer >= moveTime || (agent.hasPath && agent.remainingDistance <= 0.5f))
            {
                waiting = true;
                timer = 0f;
                agent.ResetPath();
                waitTime = Random.Range(waitTimeRange.x, waitTimeRange.y);
            }
            else if (!agent.hasPath)
            {
                PickNewDestination();
            }
        }
        else
        {
            timer += Time.deltaTime;

            if (timer >= waitTime)
            {
                waiting = false;
                timer = 0f;
                moveTime = Random.Range(moveTimeRange.x, moveTimeRange.y);
                PickNewDestination();
            }
        }
    }

    void DoAlert()
    {
        if (!player) { state = State.Wander; return; }

        // Enter alert: stop + howl once, lock for howlDuration
        if (alertTimer <= 0f)
        {
            agent.isStopped = true;
            agent.ResetPath();

            FacePlayer();

            if (!hasHowled)
            {
                anim.SetTrigger(howlTrigger);
                hasHowled = true;
            }

            alertTimer = howlDuration;
        }

        // Stay still while howling
        agent.isStopped = true;
        agent.ResetPath();
        FacePlayer();

        alertTimer -= Time.deltaTime;
        if (alertTimer <= 0f)
        {
            state = State.Chase;
        }
    }

    void DoChase()
    {
        if (!player) { state = State.Wander; return; }

        waiting = false;
        timer = 0f;

        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.stoppingDistance = 0f;
        agent.SetDestination(player.position);

        // Use remainingDistance when possible
        float dist = GetCloseDistance(Vector3.Distance(transform.position, player.position));
        if (dist <= attackRange + attackStartBuffer)
        {
            attackLockTimer = attackLockTime;
            attackTimer = 0f;
            state = State.Attack;
        }
    }

    void DoAttack()
    {
        if (!player) { state = State.Wander; return; }

        attackLockTimer -= Time.deltaTime;

        // Hard stop for attack
        agent.isStopped = true;
        agent.ResetPath();

        FacePlayer();

        // Trigger attack
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            if (Random.value < 0.5f) anim.SetTrigger(attackLTrigger);
            else anim.SetTrigger(attackRTrigger);

            float playerhp = playerHealth.getHealth();
            playerHealth.SetHealth(playerhp - 10);
            attackTimer = attackCooldown;
        }

        // After lock ends, if player is far, go back to chase
        if (attackLockTimer <= 0f)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist > attackRange + attackStartBuffer)
            {
                agent.isStopped = false;
                state = State.Chase;
            }
        }
    }

    void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
        }
    }

    void PickNewDestination()
    {
        agent.isStopped = false;
        Vector3 dest = RandomPointOnNavMesh(transform.position, wanderRadius);
        agent.SetDestination(dest);
    }

    static Vector3 RandomPointOnNavMesh(Vector3 origin, float radius)
    {
        Vector3 random = origin + Random.insideUnitSphere * radius;
        if (NavMesh.SamplePosition(random, out NavMeshHit hit, radius, NavMesh.AllAreas))
            return hit.position;

        return origin;
    }

    void FindPlayerIfNeeded()
    {
        if (player) return;
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }
}
