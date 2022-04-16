// Truck movement in different zones controller

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Trucks;

public class Truck : MonoBehaviour
{   
    public GameObject trailer;                                              // Trailer
    public WheelCollider[] t_WheelColliders = new WheelCollider[3];         // Trailer wheel colliders
    public GameObject[] t_WheelMeshes = new GameObject[3];                  // Trailer wheels meshes

    public Transform leftDoor;                                              // Trailer left door
    public Transform rightDoor;                                             // Trailer right door

    public Transform trailerCargoCollider;                                  // Trailer cargo collider (use for box cast scan)
    public Transform cargo;                                                 // Cargo slot inside the trailer

    public bool loadingEnter;               // Flag to start moving to loading zone
    private bool parking;                   // Flag to start parking
    private bool parked;                    // Flag truck is parked
    public bool loadingExit;                // Flag to leave loading zone

    private bool getTarget;                 // Flag shows that truck has a current target
    public bool haveCargo;                  // Flag shows that truck has cargo

    public List<Transform> waypoints;       // Waypoints list
    private int i = 0;                      // Waypoints counter

    void Awake()
    {
        // Truck have cargo by default
        haveCargo = true;
    }

    void FixedUpdate()
    {
        // Listening to start moving to loading zone (by LoadingZoneManager2)
        if (loadingEnter && !getTarget)
            MoveToLoadingZone();

        // Listening to start moving to warehouse parking zone
        if (parking && !getTarget)
            MoveToParkingZone();

        // Listening to start moving to exit zone
        if (loadingExit && !getTarget)
            MoveToExitZone();
        
        // Listening when truck will be ready to finish loading
        if (parked)
        {
            // Threshold angle for each door
            float doorsThreshold = 5f;

            // Check is the doors closed
            if (leftDoor.eulerAngles.y - doorsThreshold <= 180f && rightDoor.eulerAngles.y + doorsThreshold >= 180f)
            {
                // Check is the trailer empty from boxes and start exit process
                if (CheckTrailerCargo())
                    StartCoroutine(WaiterExitLoadingZone());
            }
        }
        
        // Check for target
        if (GetTarget(parking))
        {
            // Change flag and stop moving
            parking = false;
            GetComponent<TruckAIControl>().m_Driving = false;
            GetComponent<TruckAIControl>().moveBack = false;

            // Correct truck position for easy parking (trailer is still kinematic - don't need correction)
            // Freeze truck
            transform.GetComponent<Rigidbody>().isKinematic = true;
            // Move truck to parking point
            transform.position = FindObjectOfType<LoadingZoneManager2>().parkingPoint.transform.position;
            // Rotate truck
            transform.eulerAngles = new Vector3(0f, 180f, 0f);

            // Unfreeze truck head
            transform.GetComponent<Rigidbody>().isKinematic = false;

            // Unfreeze doors
            leftDoor.GetComponent<Rigidbody>().isKinematic = false;
            rightDoor.GetComponent<Rigidbody>().isKinematic = false;

            // Unfreeze and unparent cargo
            Rigidbody[] items = cargo.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody item in items)
            {
                item.isKinematic = false;
            }
            cargo.DetachChildren();

            // Set parked flag
            parked = true;
        }
        else if (GetTarget(loadingEnter) || GetTarget(loadingExit))
            i++;
    }

    // Move to LOADING ZONE
    void MoveToLoadingZone()
    {
        // Move until the last waypoint
        if (i < waypoints.Count)
        {
            // Select waypoint as truck target
            GetComponent<TruckAIControl>().m_Target = waypoints[i];
            getTarget = true;

            // Truck start moving
            TruckStartMoving(true);
        } 
        else
        {
            // Stop in last waypoint
            GetComponent<TruckAIControl>().m_Driving = false;

            // Reset counter and start parking
            i = 0;
            loadingEnter = false;

            // Correct truck and trailer position to easy parking
            // Freeze truck
            transform.GetComponent<Rigidbody>().isKinematic = true;
            // Disable joint
            trailer.transform.GetComponent<HingeJoint>().connectedBody = null;
            trailer.transform.GetComponent<HingeJoint>().useSpring = false;
            // Freeze trailer
            trailer.transform.GetComponent<Rigidbody>().isKinematic = true;
            // Rotate trailer
            Quaternion trailerRotation = trailer.transform.localRotation;
            trailer.transform.localRotation = Quaternion.Euler(trailerRotation.x, 0f, trailerRotation.z);
            // Move trailer
            Vector3 trailerPosition = trailer.transform.localPosition;
            trailer.transform.localPosition = new Vector3(0f, trailerPosition.y, trailerPosition.z);
            // Move and rotate truck
            transform.position = waypoints[waypoints.Count - 1].position;
            transform.eulerAngles = new Vector3(0f, 180f, 0f);

            // Waiting for parking. Just little pause to fix physics movement between truck and trailer
            StartCoroutine(WaitingForParking());
        }
    }

    // Move to PARKING ZONE
    void MoveToParkingZone()
    {
        // Select warehouse parking point as truck target
        GetComponent<TruckAIControl>().m_Target = FindObjectOfType<LoadingZoneManager2>().parkingPoint.transform;
        getTarget = true;

        // Truck start moving (back)
        TruckStartMoving(false);
    }

    // Move to EXIT ZONE
    void MoveToExitZone()
    {
        // Move until the last waypoint
        if (i < waypoints.Count)
        {
            // Select waypoint as truck target
            GetComponent<TruckAIControl>().m_Target = waypoints[i];
            getTarget = true;

            // Truck start moving
            TruckStartMoving(true);
        }
    }

    // Check for get target
    bool GetTarget(bool checker)
    {
        // Calculate the local-relative position of the target
        var localTarget = transform.InverseTransformPoint(GetComponent<TruckAIControl>().m_Target.position);
        
        // Listening for waypoint target
        if  (checker && getTarget && localTarget.magnitude < 2f)
        {
            // Flag to start search next waypoint
            getTarget = false;
            return true;
        }
        return false;
    }

    // Waiting for parking. Just little pause to fix physics movement between truck and trailer
    IEnumerator WaitingForParking()
    {
        yield return new WaitForSeconds(1f);

        // Unfreeze truck
        transform.GetComponent<Rigidbody>().isKinematic = false;

        yield return new WaitForSeconds(1f);

        // Parking flag
        parking = true;
    }

    // For testing. FIXME. Add finish loading triggers
    IEnumerator WaiterExitLoadingZone()
    {
        yield return new WaitForSeconds(2f);

        // Freeze doors
        leftDoor.GetComponent<Rigidbody>().isKinematic = true;
        rightDoor.GetComponent<Rigidbody>().isKinematic = true;

        // Enable joint
        trailer.transform.GetComponent<HingeJoint>().connectedBody = transform.GetComponent<Rigidbody>();
        trailer.transform.GetComponent<HingeJoint>().useSpring = true;

        // Unfreeze trailer
        trailer.transform.GetComponent<Rigidbody>().isKinematic = false;

        // Send exit waypoints to truck
        waypoints = FindObjectOfType<LoadingZoneManager2>().exitWaypoints;

        // Reset counter and flags
        i = 0;
        loadingExit = true;
        haveCargo = false;
    }

    // Truck start moving function
    void TruckStartMoving(bool cancelMoveBack)
    {
        // Start moving (controls by two variables, check CarAIControl for more info)
        GetComponent<TruckAIControl>().startMoving = true;
        GetComponent<TruckAIControl>().m_Driving = true;

        // Fix: truck dont move forward after switch off m_Driving parametr. Need to revert accel value for a 0.1 sec.
        GetComponent<TruckAIControl>().moveBack = true;
        if (cancelMoveBack)
            StartCoroutine(CancelMoveBack());
    }

    // Fix: truck dont move forward after switch off m_Driving parametr. Need to revert accel value for a 0.1 sec.
    IEnumerator CancelMoveBack()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<TruckAIControl>().moveBack = false;
    }

    // Inspect trailer cargo slot. Looking for boxes inside trailer.
    private bool CheckTrailerCargo()
    {
        // Trailer lenght
        float trailerLenght = 14f;

        // Use box cast to expand ray of search
        RaycastHit[] targets = Physics.BoxCastAll(trailerCargoCollider.position, trailerCargoCollider.lossyScale, trailerCargoCollider.forward, trailerCargoCollider.rotation, trailerLenght);
        if (targets.Length != 0)
        {
            foreach (var target in targets)
            {
                if (target.transform.tag == "box")
                    return false;
            }
        }
        return true;
    }
}