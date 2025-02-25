using UnityEngine;

public class WallCollisionSound : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip collisionSound;
    public float minCollisionVelocity = 0.5f;
    public float volumeMultiplier = 1f; // to adjust later
    
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // if this wall collided with player
        if (collision.gameObject.CompareTag("Player"))
        {   

            // check if player has won
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            
            // to make sure wall collisions can't play during victory screen
            if (playerController != null && !playerController.won())
            {
                float collisionVelocity = collision.relativeVelocity.magnitude;

                if (collisionVelocity > minCollisionVelocity)
                {
                    float volume = Mathf.Clamp01(collisionVelocity / 10f * volumeMultiplier);
                    audioSource.PlayOneShot(collisionSound, volume);
                }

            }
        }
    }
}