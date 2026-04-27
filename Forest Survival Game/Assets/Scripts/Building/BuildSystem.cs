using UnityEngine;

public class BuildSystem : MonoBehaviour
{
    public KeyCode buildKey = KeyCode.B;
    public Camera cam;
    public LayerMask groundMask;
    public GameObject campfirePrefab;

    [Header("Preview")]
    public Material canPlaceMat;   // green
    public Material cantPlaceMat;  // red
    public float checkRadius = 0.6f;

    private GameObject ghost;
    private bool buildMode;
    private bool canPlace;

    void Start()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(buildKey))
            ToggleBuildMode();

        if (!buildMode || !ghost) return;

        UpdateGhostPosition();

        if (Input.GetMouseButtonDown(0) && canPlace)
            Place();

        if (Input.GetMouseButtonDown(1))
            Cancel();
    }

    void ToggleBuildMode()
    {
        buildMode = !buildMode;

        if (buildMode) CreateGhost();
        else Cancel();
    }

    void CreateGhost()
    {
        ghost = Instantiate(campfirePrefab);
        SetGhostMode(true);
    }

    void Cancel()
    {
        if (ghost) Destroy(ghost);
        ghost = null;
        buildMode = false;
    }

    void UpdateGhostPosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 200f, groundMask))
        {
            canPlace = false;
            ApplyMat(cantPlaceMat);
            return;
        }

        ghost.transform.position = hit.point;

        canPlace = IsPlacementValid(hit.point);

        ApplyMat(canPlace ? canPlaceMat : cantPlaceMat);
    }

    bool IsPlacementValid(Vector3 pos)
    {
        Collider[] hits = Physics.OverlapSphere(pos, checkRadius);
        foreach (var h in hits)
        {
            if (!h) continue;

            // Ignore triggers
            if (h.isTrigger) continue;

            // Ignore ground layer
            if (((1 << h.gameObject.layer) & groundMask) != 0) continue;

            // Anything else blocks placement
            return false;
        }
        return true;
    }

    void Place()
    {
        Vector3 pos = ghost.transform.position;
        Quaternion rot = ghost.transform.rotation;

        Destroy(ghost);
        Instantiate(campfirePrefab, pos, rot);

        buildMode = false;
    }

    void SetGhostMode(bool isGhost)
    {
        // Disable colliders so ghost doesn’t block overlap/raycast weirdly
        foreach (var col in ghost.GetComponentsInChildren<Collider>())
            col.enabled = !isGhost;

        // Disable scripts on ghost (so it won't run campfire logic)
        foreach (var mb in ghost.GetComponentsInChildren<MonoBehaviour>())
        {
            if (mb == this) continue;
            mb.enabled = !isGhost;
        }

        ApplyMat(cantPlaceMat);
    }

    void ApplyMat(Material mat)
    {
        if (!mat || !ghost) return;

        foreach (var r in ghost.GetComponentsInChildren<Renderer>())
            r.material = mat;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!ghost) return;
        Gizmos.color = canPlace ? Color.green : Color.red;
        Gizmos.DrawWireSphere(ghost.transform.position, checkRadius);
    }
#endif
}
