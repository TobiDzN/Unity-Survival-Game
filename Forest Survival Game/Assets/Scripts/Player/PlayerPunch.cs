using UnityEngine;

public class PlayerPunch : MonoBehaviour
{
    public Camera cam;
    public float range = 2.2f;
    public int damage = 20;
    public float cooldown = 0.5f;

    float lastTime;

    void Start()
    {
        if (!cam) cam = Camera.main;
        Debug.Log("PunchAttack started. Cam=" + (cam ? cam.name : "NULL"));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastTime >= cooldown)
        {
            lastTime = Time.time;
            TryPunch();
        }
    }

    void TryPunch()
    {
        if (!cam)
        {
            Debug.LogError("No camera assigned and no MainCamera found!");
            return;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 0.5f);

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log("Punched hit: " + hit.collider.name);

            var hp = hit.collider.GetComponentInParent<WolfHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
            else
            {
                Debug.Log("Hit something without Health.");
            }
        }
        else
        {
            Debug.Log("Punch hit nothing (too far or no collider).");
        }
    }
}
