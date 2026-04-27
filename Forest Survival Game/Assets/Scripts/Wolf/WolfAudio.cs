using UnityEngine;

public class WolfAudio : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip howlClip;
    [SerializeField] private float howlCooldown = 6f;
    private float nextHowlTime = 0f;
    private void Reset()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayHowl()
    {
        if (Time.time < nextHowlTime) return;
        nextHowlTime = Time.time + howlCooldown;

        if (!source || howlClip == null) return;
        source.PlayOneShot(howlClip, 1f);
    }
}
