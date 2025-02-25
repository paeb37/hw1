using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{   
    // for pickup object behavior
    private AudioSource audioSource;
    private GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("No GameObject with 'Player' tag found!");
            return;
        }

        // set up in inspector
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource component found on this GameObject!");
        }

        audioSource.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
        // make sure it happens smoothly, based on frame update rate

        // if player is certain distance away, need to play a sound
        float distance = Vector3.Distance(transform.position, player.transform.position);
        
        if (distance <= GameData.collectDistance)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }
}

