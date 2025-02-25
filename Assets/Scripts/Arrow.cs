using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    public Transform goal;
    public Transform player;

    private RectTransform rectTransform;
    private Camera mainCamera;
    private CanvasGroup canvasGroup;
    
    void Start()
    {   
        // this is just transform of the arrow in the canvas UI
        rectTransform = GetComponent<RectTransform>();

        mainCamera = Camera.main; // need this
        canvasGroup = GetComponent<CanvasGroup>(); // lets us change opacity later

        // canvasGroup.ignoreParentGroups = true;
        canvasGroup.alpha = 1;
    }
    
    void Update()
    {   
        // no point in doing anything
        if (goal == null || mainCamera == null)
            return;
        
        // goal position to viewport position
        Vector3 goalViewportPos = mainCamera.WorldToViewportPoint(goal.position);
        
        // if goal is visible on screen
        bool goalOnScreen = goalViewportPos.z > 0 && 
                           goalViewportPos.x > 0 && goalViewportPos.x < 1 && 
                           goalViewportPos.y > 0 && goalViewportPos.y < 1;
        
        // hide arrow if goal is on screen
        if (goalOnScreen)
        {
            canvasGroup.alpha = 0;
            return;
        }
        else
        {
            canvasGroup.alpha = 1;
        }

        // dir from camera to goal (but in world space)
        Vector3 directionToGoal = goal.position - mainCamera.transform.position;
        
        // raycasting method, then we find intersection on screen
        Ray ray = new Ray(mainCamera.transform.position, directionToGoal);
        
        // only 2 are needed for this map
        Plane rightPlane = new Plane(Vector3.left, mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, 1)));
        Plane bottomPlane = new Plane(Vector3.up, mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0, 1)));
        
        float distance = Mathf.Infinity;
        bool foundIntersection = false;
        
        // check planes
        if (rightPlane.Raycast(ray, out float rightDist) && rightDist < distance) {
            distance = rightDist;
            foundIntersection = true;
            rectTransform.anchoredPosition = new Vector2(850, 0); // far right
        }
        
        // closer dist so overwrite
        if (bottomPlane.Raycast(ray, out float bottomDist) && bottomDist < distance) {
            foundIntersection = true;
            rectTransform.anchoredPosition = new Vector2(0, -375); // bottom
        }
        
        // found an intersection, position the arrow
        if (foundIntersection) {
            Vector2 directionFromCenter = new Vector2(
                goalViewportPos.x - 0.5f,
                goalViewportPos.y - 0.5f
            ).normalized;
            
            // rotate the arrow
            float angle = Mathf.Atan2(directionFromCenter.y, directionFromCenter.x) * Mathf.Rad2Deg - 90f;
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);
            
            // Debug.Log($"Intersection found at: {intersectionPoint}, Arrow position: {rectTransform.anchoredPosition}");
        }


    }
}