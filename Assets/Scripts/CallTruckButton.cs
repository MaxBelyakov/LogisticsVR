// Push button controller

using UnityEngine;
using System;

public class CallTruckButton : MonoBehaviour
{
    public GameObject lamp;                 // Lamp indicates is the truck already called
    public Light light1;                    // Lamp light 1
    public Light light2;                    // Lamp light 2
    public Material red;                    // Red light materail
    public Material green;                  // Green light materail

    private float threshold = 0.1f;         // Button threshold
    private float deadZone = 0.025f;        // Button dead zone

    private bool isPressed;                 // Flag the button is pressed
    private Vector3 startPos;               // Start button local coordinates
    private ConfigurableJoint joint;        // Button joint

    public GameObject truckPrefab;          // Big truck prefab
    public Transform truckRespawn;          // Big truck respawn point
    
    void Start()
    {
        // Save start button position
        startPos = transform.localPosition;
        joint = GetComponent<ConfigurableJoint>();
    }

    void Update()
    {
        // Works just when big truck is not in a game
        if (GameObject.FindGameObjectWithTag("big truck") == null)
        {
            // Make lamp materail and lamp light Green
            lamp.GetComponent<Renderer>().materials = new Material[] {lamp.GetComponent<Renderer>().materials[0], green};
            light1.color = Color.green;
            light2.color = Color.green;

            // Listening for player push button trigger and check for police arrest
            if (!isPressed && GetValue() + threshold >= 1f && !FindObjectOfType<LoadingZoneManager>().arrested)
                Pressed();
        }
        else
        {
            // Big truck in the game, lamp is red
            lamp.GetComponent<Renderer>().materials = new Material[] {lamp.GetComponent<Renderer>().materials[0], red};
            light1.color = Color.red;
            light2.color = Color.red;
        }

        // Listening for button released trigger
        if (isPressed && GetValue() - threshold <= 0f)
            Released();
    }

    // Calculate current button pushed value
    private float GetValue()
    {
        // Button position related to limit
        var value = Vector3.Distance(startPos, transform.localPosition) / joint.linearLimit.limit;

        // Minimum threshold for button (deadzone)
        if (Math.Abs(value) < deadZone)
            value = 0f;
        
        // Return button pushed value
        return Mathf.Clamp(value, -1f, 1f);
    }

    // Button is pressed
    private void Pressed()
    {
        isPressed = true;

        // Make lamp material and lights Red
        lamp.GetComponent<Renderer>().materials = new Material[] {lamp.GetComponent<Renderer>().materials[0], red};
        light1.color = Color.red;
        light2.color = Color.red;

        // Create new big truck in respawn point
        GameObject newTruck = Instantiate(truckPrefab, truckRespawn.position, truckRespawn.rotation);
    }

    // Button is released
    private void Released()
    {
        isPressed = false;
    }
}