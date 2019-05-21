using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraParallaxing : MonoBehaviour
{
    [SerializeField] private Transform[] elements; // Array of objects that will be parallaxed
    private float[] parallaxScales; // The proportion of the gamea's movement to move the backgrounds by
    [SerializeField] private float parallaxSmoothing = 1f; // How smooth the parallax is going to be, keep this above 0

    private Transform cameraTransform;
    private Vector3 previousCameraPosition; // The position of the camera in the previous frame

    private void Awake()
    {
        cameraTransform = FindObjectOfType<Camera>().transform;
    }

    void Start()
    {
        previousCameraPosition = cameraTransform.position; // Record position of camera

        parallaxScales = new float[elements.Length]; // Assigning corresponding parallax scales
        for (int i = 0; i < elements.Length; i++)
        {
            parallaxScales[i] = elements[i].position.z * -1;
        }
    }

    void Update()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            // The parallax is the opposite of the camera movement of the previous frame multiplied by the scale
            float parallax = (previousCameraPosition.x - cameraTransform.position.x) * parallaxScales[i];

            // Set a target x position which is the current position + the parallax
            float elementTargetXPosition = elements[i].position.x + parallax;

            // Create a target position which is the elements current position with its target x position
            Vector3 elementTargetPosition = new Vector3(elementTargetXPosition, elements[i].position.y, elements[i].position.z);

            // Move between current position and the target position using lerp
            elements[i].position = Vector3.Lerp(elements[i].position, elementTargetPosition, parallaxSmoothing * Time.deltaTime);

            // Set the preavious camera position to the cameras current position at the end of the frame
        }
        previousCameraPosition = cameraTransform.position;
    }
}
