using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{   
    // for pickup object behavior
    public float detectionDistance = 5f; // can change this as needed in inspector
    private AudioSource audioSource;
    private GameObject player;

    // the objects will turn a darker shade when the player is near it
    // to signal

    // just for the color stuff
    private Color originalColor;
    private Color normalColor;
    private Color highlightColor;
    private Renderer objectRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        objectRenderer = GetComponent<Renderer>();

        // set up audio
        audioSource = GetComponent<AudioSource>();

        // make new material
        // NOTE: this asset is using a shader so a bit more complicated
        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.mainTexture = objectRenderer.material.mainTexture; // keep texture

        // we have to edit all these fields as part of the shader script
        // for transparency mode
        newMaterial.SetInt("_Mode", 3); // Transparent mode
        newMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        newMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        newMaterial.SetInt("_ZWrite", 0);
        newMaterial.DisableKeyword("_ALPHATEST_ON");
        newMaterial.EnableKeyword("_ALPHABLEND_ON");
        newMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        newMaterial.renderQueue = 3000;

        objectRenderer.material = newMaterial;

        // Store the original color
        originalColor = objectRenderer.material.color;
        // Debug.Log($"Original color: R:{originalColor.r}, G:{originalColor.g}, B:{originalColor.b}, A:{originalColor.a}");
        
        // semi-transparent version
        normalColor = new Color(
            originalColor.r * 0.7f,
            originalColor.g * 0.7f,
            originalColor.b * 0.7f,
            0.7f // more transparent
        );
        
        highlightColor = originalColor;

        // set the starting color as transparent
        objectRenderer.material.color = normalColor;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
        // make sure it happens smoothly, based on frame update rate

        // if player is certain distance away, need to play a sound
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= detectionDistance)
        {
            // visualEffect.SetActive(true);
            if (!audioSource.isPlaying)
                audioSource.Play();

            objectRenderer.material.color = highlightColor; // Change to highlight color
            Debug.Log($"Player in range (distance: {distance}). Setting highlight color.");
        }
        else
        {
            // visualEffect.SetActive(false);
            audioSource.Stop();

            objectRenderer.material.color = normalColor; // Change back to normal color
        }
    }
}

