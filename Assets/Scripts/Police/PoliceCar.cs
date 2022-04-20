// Police controller

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Police;

public class PoliceCar : MonoBehaviour
{   
    public bool policeEnter;                    // Flag to start moving to warehouse
    public bool policeWaiting;                  // Flag to wait
    public bool policeExit;                     // Flag to start moving to exit
    
    public List<GameObject> enterPoints;        // The way from respawn to warehouse
    public List<GameObject> exitPoints;         // The exit way

    private bool getTarget;                     // Flag shows that truck has a current target
    private bool waitForBox;                    // Flag shows that police car wait the box
    public bool getBox;                         // Flag shows that police car get box

    private int i = 0;                          // Waypoints counter

    public List<Light> lights;                  // All police lights

    void Awake()
    {
        // FIXME: for testing
        policeEnter = true;
    }

    void FixedUpdate()
    {
        // Listening for flag moving to the warehouse
        if (policeEnter && !getTarget)
            MoveToWarehouse();
        
        // Listening when police car is waiting
        if (policeWaiting)
        {
            // At first need to arrest all trucks
            if (!waitForBox)
            {
                // Turn on lights
                transform.GetComponent<Animator>().enabled = true;
                transform.GetComponent<AudioSource>().enabled = true;

                GameObject.FindGameObjectWithTag("loading zone manager").GetComponent<LoadingZoneManager>().arrested = true;
                waitForBox = true;
            }
            else
            {
                // At second wait for box
                if (getBox)
                {
                    // Move to exit waiter
                    policeWaiting = false;
                    StartCoroutine(WaitingForExit());
                }
            }
        }

        // Listening to start moving to exit
        if (policeExit && !getTarget)
            MoveToExit();            
        
        // Check for get target
        if (GetTarget(policeEnter) || GetTarget(policeExit))
        {
            // Flag to start search next waypoint
            getTarget = false;
            i++;
        }
    }

    // Move to WAREHOUSE
    void MoveToWarehouse()
    {
        // Move until the last waypoint
        if (i < enterPoints.Count)
        {
            // Select waypoint as car target
            GetComponent<PoliceAIControl>().m_Target = enterPoints[i].transform;
            getTarget = true;

            // Car start moving
            CarStartMoving(true);
        } 
        else
        {
            // Stop in last waypoint
            GetComponent<PoliceAIControl>().m_Driving = false;

            // Reset counter and change flag
            i = 0;
            policeEnter = false;
            policeWaiting = true;
            getTarget = false;
        }
    }

    // Move to EXIT
    void MoveToExit()
    {
        // Move until the last waypoint
        if (i < exitPoints.Count)
        {
            // Select waypoint as car target
            GetComponent<PoliceAIControl>().m_Target = exitPoints[i].transform;
            getTarget = true;

            // Car start moving
            CarStartMoving(true);
        } 
    }

    // Check for get target
    bool GetTarget(bool checker)
    {
        // Calculate the local-relative position of the target
        var localTarget = transform.InverseTransformPoint(GetComponent<PoliceAIControl>().m_Target.position);
        
        // Listening for waypoint target
        if  (checker && getTarget && localTarget.magnitude < 2f)
            return true;
        return false;
    }

    // Car start moving function
    void CarStartMoving(bool cancelMoveBack)
    {
        // Start moving (controls by two variables, check PoliceAIControl for more info)
        GetComponent<PoliceAIControl>().startMoving = true;
        GetComponent<PoliceAIControl>().m_Driving = true;

        // Fix: truck dont move forward after switch off m_Driving parametr. Need to revert accel value for a 0.1 sec.
        GetComponent<PoliceAIControl>().moveBack = true;
        if (cancelMoveBack)
            StartCoroutine(CancelMoveBack());
    }

    // Fix: car dont move forward after switch off m_Driving parametr. Need to revert accel value for a 0.1 sec.
    IEnumerator CancelMoveBack()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<PoliceAIControl>().moveBack = false;
    }

    // Waiter for exit
    IEnumerator WaitingForExit()
    {
        yield return new WaitForSeconds(1f);

        // Turn off lights
        transform.GetComponent<Animator>().enabled = false;
        transform.GetComponent<AudioSource>().enabled = false;
        foreach (var light in lights)
        {
            light.enabled = false;
        }

        yield return new WaitForSeconds(2f);

        // Leave warehouse zone
        policeExit = true;

        // Cancel warehouse arrest
        GameObject.FindGameObjectWithTag("loading zone manager").GetComponent<LoadingZoneManager>().arrested = false;
    }

    // Waiting for box
    void OnTriggerEnter(Collider collider)
    {
        // Check for police arrested warehouse and player give police car a box
        if (GameObject.FindGameObjectWithTag("loading zone manager").GetComponent<LoadingZoneManager>().arrested
        && collider.transform.parent.tag == "box")
        {
            // Take a box
            Destroy(collider.transform.parent.gameObject);
            getBox = true;
        }
    }
}