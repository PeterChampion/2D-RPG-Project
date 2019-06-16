using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles all camera parallazing effects, moves the elements stored within an array based on their z axis, the closer (larger) the z axis the faster the element moves across the scene.
public class CameraParallaxing : MonoBehaviour
{
    [SerializeField] private Transform[] elements; // Array of objects that will be parallaxed
    private float[] parallaxScales; // The proportion of the game's movement to move the elements by
    [SerializeField] private float parallaxSmoothing = 1f; // How smooth the parallax is going to be, keep this above 0

    private Transform cameraTransform;
    private Vector3 previousCameraPosition; // The position of the camera in the previous frame

    private void Awake()
    {
        cameraTransform = FindObjectOfType<Camera>().transform; // Set reference to camera transform
    }

    void Start()
    {
        previousCameraPosition = cameraTransform.position; // Record position of camera

        parallaxScales = new float[elements.Length]; // The number of parallaxScales depends on the amount of elements that need parallaxing
        for (int i = 0; i < elements.Length; i++)
        {
            parallaxScales[i] = elements[i].position.z * -1; // Determines the scale of the parallax by inverting the value (5 becomes -5, -5 becomes 5)
        }
    }

    void Update()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            // The parallax is the difference of the camera position of the current frame from the previous, multiplied by the scale
            float parallax = (previousCameraPosition.x - cameraTransform.position.x) * parallaxScales[i];

            // Set a target x position which is the current position + the parallax
            float elementTargetXPosition = elements[i].position.x + parallax;

            // Create a target position which is the elements current position with its target x position
            Vector3 elementTargetPosition = new Vector3(elementTargetXPosition, elements[i].position.y, elements[i].position.z);

            // Move between current position and the target position by lerping
            elements[i].position = Vector3.Lerp(elements[i].position, elementTargetPosition, parallaxSmoothing * Time.deltaTime);
        }
        // Set the preavious camera position to the cameras current position at the end of the frame
        previousCameraPosition = cameraTransform.position;
    }
}
