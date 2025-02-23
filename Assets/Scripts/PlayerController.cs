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
    // private float movementX;
    // private float movementY;
    private int count;

    // public float speed = 0; // set in the editor
    public TextMeshProUGUI countText;

    // need the game object so we can set active / hide it
    // public GameObject winTextObject;
    // public TextMeshProUGUI winTextRaw;

    public float maxTiltAngle = 30f;

    
    // jump elements
    private bool onGround; // to prevent double jump in air
    public float jumpForce = 5f; // can set in editor
    public LayerMask groundLayer; // set this in editor to detect ground
    

    // for the player tapping on collectibles part
    private Camera mainCamera;
    private float collectDistance;
    public ParticleSystem collectEffect; // Assign in inspector


    // apply gravity to make sure player stays on board
    // otherwise will bounce up

    // public float additionalGravity; // set in inspector
    // private Vector3 gravityDirection = Vector3.down;

    // Start is called before the first frame update

    // for managing the canvas stuff
    // public Canvas gameplayCanvas; // Main canvas with gameplay UI
    // public Canvas victoryCanvas;  // Victory screen canvas
    // public TextMeshProUGUI completionTimeText;
    // public TextMeshProUGUI collectiblesText;
    // public GameObject nextLevelButton;
    // public GameObject restartLevelButton;
    // public GameObject startScreenButton;

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
    // public AudioClip inRangeSound;
    // private bool isInRange = false;
    
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
        // winTextObject.SetActive(false);

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

        // if (count >= 8) {
        //     winTextObject.SetActive(true);
        //     winTextObject.GetComponent<TextMeshProUGUI>().text = "You Win!";

        //     Destroy(GameObject.FindGameObjectWithTag("Enemy"));

        // }
    }

    // Use Update() just to track clicks
    // Here, frame rate doesn't matter (no physics)
    void Update()
    {
        /*** on unity editor!!! so using mouse ****/
        #if UNITY_EDITOR
        // if (Input.GetMouseButtonDown(0)) // Left mouse click
        // {
        //     CheckForCollectible(Input.mousePosition);
        // }
        // #else
        // if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        // {
        //     CheckForCollectible(Input.GetTouch(0).position);
        // }

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
                    // the in-range sound is handled by the pickup controller script
                    // because we want to play it whenever, not just when the player clicks

                    // if (!isInRange && inRangeSound != null)
                    // {   
                    //     // this method uses AudioSource class instead of making an instance
                    //     AudioSource.PlayClipAtPoint(inRangeSound, hit.collider.transform.position, 0.5f);
                    // }
                    // isInRange = true;

                    HandleCollect(hit.collider.gameObject);
                }
                else
                {
                    // isInRange = false;
                    Debug.Log("Too far to collect!");
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
            Destroy(effect.gameObject, effect.main.duration); // Clean up after effect finishes
        }

        // play collect audio
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, collectible.transform.position);
        }

        // AudioSource audio = collectible.GetComponent<AudioSource>();
        // if (audio != null)
        // {
        //     audio.transform.SetParent(null);
        //     audio.Play();
        //     Destroy(audio.gameObject, audio.clip.length);
        // }

        count += 1;
        SetCountText(); // updates the UI

        // disable the collectible now
        collectible.SetActive(false);
    }

    void FixedUpdate()
    {   
        CheckGrounded(); // do this first for jumping

        // Vector3 boardNormal = transform.parent.up;
        
        // // Calculate gravity direction (opposite of board normal)
        // gravityDirection = -boardNormal;
        
        // // Apply additional downward force to keep the ball on the board
        // rb.AddForce(gravityDirection * additionalGravity, ForceMode.Acceleration);

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

    // private void OnCollisionEnter(Collision collision) {
    //     if (collision.gameObject.CompareTag("Enemy")) {
    //         Destroy(gameObject); // gameObject is current object

    //         winTextObject.gameObject.SetActive(true);
    //         winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
    //     }
    // }

    /** GOALLL!!!! **///
    // detect collisions with the goal area, to move to next level
    private void OnTriggerEnter(Collider other) {

        Debug.Log($"Triggered with: {other.gameObject.name}, Tag: {other.gameObject.tag}");

        if (other.gameObject.CompareTag("Goal")) {

            hasWon = true;

            Debug.Log("Goal trigger detected!");

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
    
    // load victory screen
    // private void LoadVictory()
    // {
    //     // int curSceneIdx = SceneManager.GetActiveScene().buildIndex;
    //     // int nextSceneIdx = curSceneIdx + 1;

    //     VictoryScreen victoryScreen = FindObjectOfType<VictoryScreen>();
    //     if (victoryScreen != null)
    //     {
    //         victoryScreen.ShowVictoryScreen(count); // have to pass in the collectibles count
    //     }

    //     // int totalScenes = SceneManager.sceneCountInBuildSettings;
    //     // Debug.Log($"Current Scene Index: {currentSceneIdx}");
    //     // Debug.Log($"Next Scene Index: {nextSceneIdx}");
    //     // Debug.Log($"Total Scenes in Build: {totalScenes}");

    //     // make sure we don't exceed the total number of scenes
    //     // so if on the last level, just go back to home page?

    //     // we need to show a victory screen as well
    //     // when it hits the goal, just immediately show the victory screen

    //     // 
    //     // if (nextSceneIdx < SceneManager.sceneCountInBuildSettings)
    //     // {
    //     //     StartCoroutine(LoadSceneWithProgress(nextSceneIdx));
    //     //     // use Async for better performance
    //     //     // StartCoroutine(LoadSceneAsync(nextSceneIdx));
    //     // }
    //     // else
    //     // {
    //     //     Debug.Log("Game completed!");
    //     //     // Can load a game completion scene here later
    //     // }
    // }

    /*
    private IEnumerator LoadSceneWithProgress(int sceneIndex) {
        winTextObject.SetActive(true); // show win screen
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        
        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // easy convert for 0-1 range
            
            if (winTextRaw != null)
            {
                winTextRaw.text = $"Complete! Loading next... {progress * 100:F0}%";
            }
            
            yield return null;
        }
    }
    */


}
