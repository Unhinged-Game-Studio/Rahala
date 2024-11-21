using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLookatPLayer : MonoBehaviour
{
    public Transform playerCamera;  // Assign the player's camera in the Inspector
    public float minVisibilityRadius = 2f;  // Minimum distance to hide the text
    public float maxVisibilityRadius = 10f;  // Maximum distance to hide the text

    private Renderer textRenderer;  // Renderer component of the text mesh

    void Start()
    {
        // Get the Renderer component
        textRenderer = GetComponent<Renderer>();

        // Ensure the text starts hidden if the player is outside the range
        if (textRenderer != null)
            textRenderer.enabled = false;
    }

    void LateUpdate()
    {
        if (playerCamera != null)
        {
            // Calculate the distance between the player and the text
            float distanceToPlayer = Vector3.Distance(playerCamera.position, transform.position);

            // Toggle visibility based on the distance range
            if (textRenderer != null)
            {
                textRenderer.enabled = distanceToPlayer <= maxVisibilityRadius && distanceToPlayer >= minVisibilityRadius;
            }

            // Rotate to face the player if visible
            if (textRenderer.enabled)
            {
                Vector3 direction = transform.position - playerCamera.position;
                transform.rotation = Quaternion.LookRotation(direction);
                transform.Rotate(0, 180, 0);  // Adjust for any 180-degree flip
            }
        }
    }
}
