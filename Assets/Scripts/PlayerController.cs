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

    public float speed = 0; // set in the editor
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    
    private AttitudeSensor attitudeSensor;
    // private Quaternion initialRotation;

    public float maxTiltAngle = 30f;

    
    // jump elements
    private bool onGround; // to prevent double jump in air
    public float jumpForce = 5f; // can set in editor
    public LayerMask groundLayer; // set this in editor to detect ground
    

    // for the player tapping on collectibles part
    private Camera mainCamera;
    public float collectDistance = 5f; // Distance within which items can be collected
    public ParticleSystem collectEffect; // Assign in inspector
    

    // Start is called before the first frame update
    void Start()
    {   
        mainCamera = Camera.main;

        rb = GetComponent<Rigidbody>();
        count = 0;
        onGround = true;

        // Get the AttitudeSensor
        attitudeSensor = AttitudeSensor.current;
        if (attitudeSensor != null)
        {
            InputSystem.EnableDevice(attitudeSensor);
            // I confirmed this part works, so it is enabled
            Debug.Log("Attitude Sensor enabled: " + attitudeSensor.enabled);
        }
        else
        {
            Debug.LogWarning("AttitudeSensor not found");
        }

        SetCountText();
        winTextObject.SetActive(false);
    }

    /*
    void OnMove(InputValue movementValue) 
    {
        Vector2 movementVector = movementValue.Get<Vector2>(); 
        movementX = movementVector.x;
        movementY = movementVector.y;

    }
    */

    // New: Method to check if we're grounded
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
        if (onGround)
        {
            // this is upward force
            // use impulse to make it sudden
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

    // Use Update() when frame rate doesn't matter
    // So no physics stuff here
    void Update()
    {
        // Check for mouse click in editor or touch on mobile
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            CheckForCollectible(Input.mousePosition);
        }
        #else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckForCollectible(Input.GetTouch(0).position);
        }
        #endif
    }

    // checks if there is a collectible where the person clicked
    void CheckForCollectible(Vector3 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if we hit a collectible
            if (hit.collider.CompareTag("Pickup"))
            {
                // Check if player is close enough
                float distanceToCollectible = Vector3.Distance(transform.position, hit.collider.transform.position);
                
                if (distanceToCollectible <= collectDistance)
                {
                    // Collect the item
                    CollectItem(hit.collider.gameObject);
                }
                else
                {
                    Debug.Log("Too far to collect!");
                }
            }
        }
    }

    void CollectItem(GameObject collectible)
    {
        // Spawn particle effect if assigned
        if (collectEffect != null)
        {
            ParticleSystem effect = Instantiate(collectEffect, collectible.transform.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration); // Clean up after effect finishes
        }

        // Play audio if the collectible has an AudioSource
        AudioSource audio = collectible.GetComponent<AudioSource>();
        if (audio != null)
        {
            // Detach audio source before destroying collectible
            audio.transform.SetParent(null);
            audio.Play();
            Destroy(audio.gameObject, audio.clip.length);
        }

        // Increment counter and update UI
        count += 1;
        SetCountText();

        // Disable the collectible
        collectible.SetActive(false);
    }

    void FixedUpdate()
    {   
        CheckGrounded(); // do this first for jumping


        Vector3 force = Vector3.zero; // 0,0,0
        // so that we don't redefine it every time

        // use WASD in editor, tilt on device
        // including drag to make it more realistic
        #if UNITY_EDITOR
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            force = new Vector3(horizontalInput, 0, verticalInput);
            
            // Debug.Log("Editor Controls - WASD/Arrows: " + force);
        #else

        // Debug.LogWarning("ENTERED THE CORRECT PLACE HEHE!");
        // if (AttitudeSensor.current != null)

        if (attitudeSensor != null)
        {
            // quaternion is internal repr for rotation
            Quaternion attitude = attitudeSensor.attitude.ReadValue();
            
            Vector3 deviceUp = attitude * Vector3.up; // uses local vector for phone

            // directions in the Unity Editor
            // I tried all possible combinations here
            float forceX = -deviceUp.z;
            float forceY = deviceUp.x;
            float forceZ = deviceUp.y;

            // normalize it (so we can scale it later)
            force = new Vector3(forceX, 0, forceZ).normalized;
            
            Debug.Log($"Force Direction: X={forceX:F2}, Y={forceY:F2}, Z={forceZ:F2}");
        }

        #endif

        // force on rigidbody
        rb.AddForce(force * speed);
    }

    // private void OnCollisionEnter(Collision collision) {
    //     if (collision.gameObject.CompareTag("Enemy")) {
    //         Destroy(gameObject); // gameObject is current object

    //         winTextObject.gameObject.SetActive(true);
    //         winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
    //     }
    // }

    // detect collisions with the goal area, to move to next level
    private void OnTriggerEnter(Collider other) {

        Debug.Log($"Triggered with: {other.gameObject.name}, Tag: {other.gameObject.tag}");

        if (other.gameObject.CompareTag("Goal")) {
            Debug.Log("Goal trigger detected!");
            LoadNext();
        }
    }

    private void LoadNext()
    {
        int currentSceneIdx = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIdx = currentSceneIdx + 1;

        // int totalScenes = SceneManager.sceneCountInBuildSettings;
        // Debug.Log($"Current Scene Index: {currentSceneIdx}");
        // Debug.Log($"Next Scene Index: {nextSceneIdx}");
        // Debug.Log($"Total Scenes in Build: {totalScenes}");

        // make sure we don't exceed the total number of scenes
        if (nextSceneIdx < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadSceneAsync(nextSceneIdx);
            // use Async for better performance
            // StartCoroutine(LoadSceneAsync(nextSceneIdx));
        }
        else
        {
            Debug.Log("Game completed!");
            // Can load a game completion scene here later
        }
    }

    private IEnumerator LoadSceneAsync(int sceneIndex) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        
        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            // You could add a loading progress bar here using asyncLoad.progress
            Debug.Log($"Loading progress: {asyncLoad.progress * 100}%");
            yield return null;
        }
    }


}
