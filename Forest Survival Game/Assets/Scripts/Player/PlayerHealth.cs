using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health;


    void Start()
    {
        health = 100;
    }


    void Update()
    {

    }

    public float getHealth()
    {
        return health;
    }

    public void SetHealth(float num)
    {
        health = num;
    }
}
