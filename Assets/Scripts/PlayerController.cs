using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    private Rigidbody rb;
    private int count;
    public TextMeshProUGUI countText;
    public float maxTiltAngle = 30f;

    
    // jump elements
    private bool onGround; // to prevent double jump in air
    public float jumpForce = 5f; // can set in editor
    public LayerMask groundLayer; // set this in editor to detect ground
    

    // for the player tapping on collectibles part
    private Camera mainCamera;
    private float collectDistance;
    public ParticleSystem collectEffect;

    /** AUDIO !!!! ***/
    // ball roll
    private AudioSource rollingAudioSource; // this is a custom unity class for manipulating sound
    public AudioClip rollingSound; // in inspector
    private float minVelocityToPlaySound = 0.1f; // minimum velocity to trigger sound

    // ball jump
    private AudioSource jumpAudioSource;
    public AudioClip jumpSound;

    // collectible
    public AudioClip collectSound;
    
    // goal reached
    public AudioClip goalSound;

    // need this so that other sounds can't play after you win
    private bool hasWon = false;

    public bool won() {
        return hasWon;
    }

    void Start()
    {   
        mainCamera = Camera.main;
        collectDistance = GameData.collectDistance;

        rb = GetComponent<Rigidbody>();
        count = 0;
        onGround = true;

        SetCountText();

        // audio roll init
        rollingAudioSource = gameObject.AddComponent<AudioSource>();
        rollingAudioSource.clip = rollingSound;
        rollingAudioSource.loop = true; // so it loops while rolling
        rollingAudioSource.playOnAwake = false;
        rollingAudioSource.volume = 1.0f;  // init vol is max

        // jump init
        jumpAudioSource = gameObject.AddComponent<AudioSource>();
        jumpAudioSource.playOnAwake = false;
        jumpAudioSource.volume = 1.0f;
    }

    // check if we're grounded (to prevent double jump)
    private void CheckGrounded()
    {
        // cast sphere below the player
        float sphereRadius = 0.1f;

        onGround = Physics.SphereCast(
            transform.position,
            sphereRadius,
            Vector3.down,
            out RaycastHit hit,
            GetComponent<SphereCollider>().radius + 0.1f,
            groundLayer // this is set in editor
        );
    }

    // DO NOT double jump
    public void Jump()
    {      
        // do not do anything if you done
        if (hasWon) return;

        if (onGround)
        {   
            // sound first
            if (jumpSound != null)
            {
                jumpAudioSource.PlayOneShot(jumpSound);
            }

            // upward force, using impulse to make it sudden
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void SetCountText() {
        countText.text = "Count: " + count.ToString();
    }

    // Use Update() just to track clicks
    // Here, frame rate doesn't matter (no physics)
    void Update()
    {
        /*** on unity editor!!! so using mouse ****/
        #if UNITY_EDITOR

        // checks for mouse click in editor
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckForCollectible(Mouse.current.position.ReadValue());
        }

        /*** on phone!!!!! using touch ****/
        #else
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
        {
            CheckForCollectible(Touchscreen.current.primaryTouch.position.ReadValue());
        }

        #endif
    }

    // checks if there is a collectible where the person clicked
    // uses raycasting
    void CheckForCollectible(Vector3 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Pickup")) // hit collectible
            {
                float distanceToCollectible = Vector3.Distance(transform.position, hit.collider.transform.position);
                
                // close enough
                if (distanceToCollectible <= collectDistance)
                {
                    HandleCollect(hit.collider.gameObject);
                }
                else
                {
                    // Debug.Log("Too far to collect!");
                }
            }
        }
    }

    // this does particle system, plays audio
    // and updates the count + UI
    void HandleCollect(GameObject collectible)
    {   
        if (hasWon) return;

        // particle effect
        if (collectEffect != null)
        {
            ParticleSystem effect = Instantiate(collectEffect, collectible.transform.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration); // have to clean up after effect finishes
        }

        // play collect audio
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, collectible.transform.position);
        }

        count += 1;
        SetCountText(); // updates the UI

        // disable the collectible now
        collectible.SetActive(false);
    }

    void FixedUpdate()
    {   
        CheckGrounded(); // do this first for jumping

        // ball rolling sound
        // only trigger when it's on the ground and rolling
        if (!hasWon && rb.velocity.magnitude > minVelocityToPlaySound && onGround)
        {
            if (!rollingAudioSource.isPlaying)
            {
                rollingAudioSource.Play();
            }

            // higher velocity means louder sound
            rollingAudioSource.volume = Mathf.Clamp01(rb.velocity.magnitude / 5f);
        }
        else if (rollingAudioSource.isPlaying)
        {
            rollingAudioSource.Stop(); // do not play if conditions are not met
        }
    }

    /** GOALLL!!!! **///
    // detect collisions with the goal area, to move to next level
    private void OnTriggerEnter(Collider other) {

        // Debug.Log($"Triggered with: {other.gameObject.name}, Tag: {other.gameObject.tag}");

        if (other.gameObject.CompareTag("Goal")) {

            hasWon = true;

            // Debug.Log("Goal trigger detected!");

            // play goal reached sound
            if (goalSound != null)
            {
                AudioSource.PlayClipAtPoint(goalSound, transform.position, 1.0f);
            }

            // load victory screen
            VictoryScreen victoryScreen = FindObjectOfType<VictoryScreen>();
            if (victoryScreen != null)
            {
                victoryScreen.ShowVictoryScreen(count); // have to pass in the collectibles count
            }
        }
    }
}
