// Truck controller

using System.Collections;
using UnityEngine;

public class Truck : MonoBehaviour
{   
    public bool loadingEnter;               // Flag to start moving to target waiting cell
    public bool loadingExit;                // Flag to start moving to loading zone
    public bool parking;                    // Flag to start moving to warehouse parking point

    private bool getTarget;                 // Flag shows that truck has a current target

    public GameObject targetCell;           // Waiting cell connected to the truck by manager

    private int i = 0;                      // Waypoints counter

    void FixedUpdate()
    {
        //
        // WAITING ZONE
        //
        // Listening for flag moving to the waiting cell
        if (loadingEnter && !getTarget)
        {
            // Get the waypoints from selected cell
            var cellEnterWay = targetCell.GetComponent<LoadingCell>().cellEnter;

            // Move until the last waypoint
            if (i < cellEnterWay.Count)
            {
                // Select waypoint as truck target
                //GetComponent<TruckAIControl>().m_Target = cellEnterWay[i].transform;
                getTarget = true;

                // Truck start moving
                //TruckStartMoving(true);
            } 
            else
            {
                // Stop in last waypoint
                //GetComponent<TruckAIControl>().m_Driving = false;

                // Reset counter and change flag
                i = 0;
                loadingEnter = false;
            }
        }

        // Calculate the local-relative position of the target
        //var localTarget = transform.InverseTransformPoint(GetComponent<CarAIControl>().m_Target.position);
        
        // Listening for waypoint when moving to the target cell 
        /*if  (loadingEnter && getTarget && localTarget.magnitude < 2f)
        {
            // Flag to start search next waypoint
            i++;
            getTarget = false;
        }

        //
        // LOADING ZONE
        //
        // Listening loading manager to start moving to loading zone
        if (loadingExit && !getTarget)
        {
            // Get the waypoints from selected cell
            var cellExitWay = targetCell.GetComponent<LoadingCell>().cellExit;

            // Move until the last waypoint
            if (i < cellExitWay.Count)
            {
                // Select waypoint as truck target
                GetComponent<CarAIControl>().m_Target = cellExitWay[i].transform;
                getTarget = true;

                // Truck start moving
                TruckStartMoving(true);
            } 
            else
            {
                // Stop in last waypoint
                GetComponent<CarAIControl>().m_Driving = false;

                // Disconnect truck from the cell and change cell status to empty
                targetCell.GetComponent<LoadingCell>().readyForLoading = null;
                targetCell.GetComponent<LoadingCell>().status = true;

                // Reset counter and change flag
                i = 0;
                loadingExit = false;

                // Start parking to warehouse
                parking = true;
            }
        }

        // Calculate the local-relative position of the target
        localTarget = transform.InverseTransformPoint(GetComponent<CarAIControl>().m_Target.position);

        // Listening for waypoint when moving to loading zone
        if  (loadingExit && getTarget && localTarget.magnitude < 2f)
        {
            // Flag to start search next waypoint
            i++;
            getTarget = false;
        }

        //
        // PARKING ZONE
        //
        // Listening to start moving to warehouse parking zone
        if (parking && !getTarget)
        {
            // Select warehouse parking point as truck target
            GetComponent<CarAIControl>().m_Target = FindObjectOfType<LoadingZoneManager>().parkingPoint.transform;
            getTarget = true;

            // Truck start moving (back)
            TruckStartMoving(false);
        }

        // Calculate the local-relative position of the target
        localTarget = transform.InverseTransformPoint(GetComponent<CarAIControl>().m_Target.position);

        // Listening for waypoint when moving to parking zone
        if  (parking && getTarget && localTarget.magnitude < 2f)
        {
            // Change flags and stop moving
            parking = false;
            getTarget = false;
            GetComponent<CarAIControl>().m_Driving = false;
            GetComponent<CarAIControl>().moveBack = false;

            // For testing
            StartCoroutine(ExitLoadingZone());
        }*/
    }
/*
    // Truck start moving function
    void TruckStartMoving(bool cancelMoveBack)
    {
        // Start moving (controls by two variables, check CarAIControl for more info)
        GetComponent<CarAIControl>().startMoving = true;
        GetComponent<CarAIControl>().m_Driving = true;

        // Fix: truck dont move forward after switch off m_Driving parametr. Need to revert accel value for a 0.1 sec.
        GetComponent<CarAIControl>().moveBack = true;
        if (cancelMoveBack)
            StartCoroutine(CancelMoveBack());
    }

    // Fix: truck dont move forward after switch off m_Driving parametr. Need to revert accel value for a 0.1 sec.
    IEnumerator CancelMoveBack()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<CarAIControl>().moveBack = false;
    }

    // For testing
    IEnumerator ExitLoadingZone()
    {
        yield return new WaitForSeconds(3f);

        // Select exit point as target
        GetComponent<CarAIControl>().m_Target = FindObjectOfType<LoadingZoneManager>().exit.transform;

        // Truck start moving
        TruckStartMoving(true);

        yield return new WaitForSeconds(3f);

        // Finish truck loading process, send flag to loading manager to start search next truck for loading
        FindObjectOfType<LoadingZoneManager>().loading = false;
    }*/
}
