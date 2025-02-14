using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

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



    // Start is called before the first frame update
    void Start()
    {
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

        // SetCountText();
        // winTextObject.SetActive(false);
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

    /*
    void SetCountText() {
        countText.text = "Count: " + count.ToString();

        if (count >= 8) {
            winTextObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Win!";

            Destroy(GameObject.FindGameObjectWithTag("Enemy"));

        }
    }
    */

    void FixedUpdate()
    {   
        CheckGrounded(); // do this first


        Vector3 force = Vector3.zero; // 0,0,0
        // so that we don't redefine it every time

        // use WASD in editor, tilt on device
        // including drag to make it more realistic
        #if UNITY_EDITOR
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            force = new Vector3(horizontalInput, 0, verticalInput);
            
            Debug.Log("Editor Controls - WASD/Arrows: " + force);
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

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            Destroy(gameObject); // gameObject is current object

            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
        }
    }

    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag("Pickup")) {
            other.gameObject.SetActive(false);

            count += 1;
            // SetCountText();
        }


        


    }
}
