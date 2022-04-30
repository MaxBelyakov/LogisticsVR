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
    private bool waiting;                   // Flag truck is waiting

    private bool getTarget;                 // Flag shows that truck has a current target
    public bool haveCargo;                  // Flag shows that truck has cargo

    public List<Transform> waypoints;       // Waypoints list
    private int i = 0;                      // Waypoints counter

    private Transform parkingHelper;        // Coordinates truck needs parking to use help function

    private int boxesInTruck;               // Amount of boxes in truck before unloading

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
        if (parking && !getTarget && parkingHelper == null)
            MoveToParkingZone();

        // Listening to start moving to exit zone
        if (loadingExit && !getTarget)
            MoveToExitZone();
        
        // Listening when truck will be ready to finish loading
        if (parked && !waiting)
        {
            // Threshold angle for each door
            float doorsThreshold = 5f;

            // Check is the trailer empty from boxes and start exit process
            if (CheckTrailerCargo(false))
            {
                // Check is the doors closed
                if (leftDoor.eulerAngles.y - doorsThreshold <= 180f && rightDoor.eulerAngles.y + doorsThreshold >= 180f)
                    StartCoroutine(WaiterExitLoadingZone());
            }
        }
        
        // Check for target
        if (GetTarget(parking))
        {
            // Change flag and stop moving
            parking = false;
            parked = true;
            GetComponent<TruckAIControl>().m_Driving = false;
            GetComponent<TruckAIControl>().moveBack = false;

            // Move truck to parking point
            parkingHelper = FindObjectOfType<LoadingZoneManager2>().trailerParkingPoint;

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

            // Count boxes in truck before unloading
            CheckTrailerCargo(true);
        }
        else if (GetTarget(loadingEnter) || GetTarget(loadingExit))
            i++;
        
        // Parking helper
        if (parkingHelper != null)
            ParkingHelper(parkingHelper);
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

            // Freeze truck
            transform.GetComponent<Rigidbody>().isKinematic = true;
            
            // Move and rotate truck
            parkingHelper = waypoints[waypoints.Count - 1];

            // Reset counter and start parking
            i = 0;
            loadingEnter = false;
            parking = true;
        }
    }

    // Move to PARKING ZONE
    void MoveToParkingZone()
    {
        // Freeze trailer rotation
        trailer.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;

        // Unfreeze truck
        transform.GetComponent<Rigidbody>().isKinematic = false;
        
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
        // Ignore when there is no target
        if (!getTarget)
            return false;

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

    // Truck exiting warehouse
    IEnumerator WaiterExitLoadingZone()
    {
        // Freeze doors
        leftDoor.GetComponent<Rigidbody>().isKinematic = true;
        rightDoor.GetComponent<Rigidbody>().isKinematic = true;
        
        // Start waiting
        waiting = true;
        
        yield return new WaitForSeconds(2f);

        // Unfreeze trailer rotation
        trailer.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        // Send exit waypoints to truck
        waypoints = FindObjectOfType<LoadingZoneManager2>().exitWaypoints;

        // Reset boxes storage
        boxesInTruck = 0;

        // Reset counter and flags
        i = 0;
        loadingExit = true;
        haveCargo = false;
        waiting = false;
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
    private bool CheckTrailerCargo(bool saveCount)
    {
        // Trailer lenght
        float trailerLenght = 14f;

        // Reset counter
        int m = 0;

        // Use box cast to expand ray of search
        RaycastHit[] targets = Physics.BoxCastAll(trailerCargoCollider.position, trailerCargoCollider.lossyScale, trailerCargoCollider.forward, trailerCargoCollider.rotation, trailerLenght);
        if (targets.Length != 0)
        {
            foreach (var target in targets)
            {
                if (target.transform.tag == "box")
                    m++;
            }
        }

        // Save amount of boxes before truck unloading
        if (saveCount)
            boxesInTruck = m;
        else
        {
            // Send counter to info desk. Take in account amount of boxes before unloading truck
            GameObject.FindGameObjectWithTag("info desk").GetComponent<InfoDesk>().unloaded += boxesInTruck - m;
            boxesInTruck = m;
        }

        // Truck can't leave unloading while there are boxes inside
        if (m != 0)
            return false;
        else
            return true;
    }

    // Helping truck to park correct by moving and rotating it
    void ParkingHelper(Transform target)
    {
        // Flags shows is a rotation or position in target value
        var rotation = false;
        var position = false;
        var trailerRotation = false;

        // Thresholds for rotation and position
        var rotationThreshold = -0.9999f;
        var positionThreshold = 0.01f;

        // Parking to loading zone helper
        if (parking)
        {
            // Determine which direction to rotate towards. Get parking point and move it forward
            Vector3 targetDirection = target.position + target.forward * 40f - transform.position;
            
            // Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 0.5f * Time.deltaTime, 0.0f);

            // Calculate difference between truck and target position
            var posX = Mathf.Abs(transform.position.x - target.position.x);
            var posZ = Mathf.Abs(transform.position.z - target.position.z);

            // Compare truck rotation with the target
            if (newDirection.z > rotationThreshold)
                // Applies rotation to truck target
                transform.rotation = Quaternion.LookRotation(newDirection);
            else
                rotation = true;

            // Compare truck position with the target
            if (posX > positionThreshold && posZ > positionThreshold)
                // Applies moving to truck target
                transform.position = Vector3.MoveTowards(transform.position, target.position, 1.5f * Time.deltaTime);
            else
                position = true;
            
            // Correct target height (trailer and truck center on different height)
            Vector3 truckTarget = new Vector3(transform.position.x, trailer.transform.position.y, transform.position.z);

            // Determine which direction to rotate towards. Get parking point and move it forward
            Vector3 trailerTargetDirection = truckTarget - trailer.transform.position;
            
            // Rotate the forward vector towards the target direction by one step
            Vector3 trailerNewDirection = Vector3.RotateTowards(trailer.transform.forward, trailerTargetDirection, 1f * Time.deltaTime, 0.0f);

            // Compare trailer rotation with the target
            if (Mathf.Abs(trailerNewDirection.x) > 0.02f)
                // Applies rotation to trailer target
                trailer.transform.rotation = Quaternion.LookRotation(trailerNewDirection);
            else
                trailerRotation = true;
            
            // Finish parking helper
            if (rotation && position && trailerRotation)
                parkingHelper = null;
        }
        
        // Parking to warehouse helper
        if (parked)
        {
            // Calculate difference between truck and target position
            var trailerPosX = Mathf.Abs(trailer.transform.position.x - target.position.x);
            var trailerPosZ = Mathf.Abs(trailer.transform.position.z - target.position.z);

            // Compare truck position with the target
            if (trailerPosX > positionThreshold && trailerPosZ > positionThreshold)
                // Applies moving to truck target
                trailer.transform.position = Vector3.MoveTowards(trailer.transform.position, target.position, 1.5f * Time.deltaTime);
            else
                // Finish parking helper
                parkingHelper = null;
        }
    }
}