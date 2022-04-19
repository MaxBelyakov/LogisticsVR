// Small truck controller
// There are 3 listeners: waiting zone - loading zone - parking zone

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class SmallTruckController : MonoBehaviour
{   
    public bool cellEnter;                  // Flag to start moving to target waiting cell
    public bool loadingEnter;               // Flag to start moving to loading zone
    public bool parking;                    // Flag to start moving to warehouse parking point
    private bool parked;                    // Flag truck is parked
    public bool loaded;                     // Flag truck is loaded
    private bool unloading;                 // Flag truck unloading
    private bool goToManager;               // Flag truck finish work and need to start again

    public List<GameObject> deliveryPoints;     // The way from manager zone to the cell
    public List<GameObject> unloadingPoints;    // Points where truck unload boxes

    public Transform truckCargoScan;        // Truck cargo collider (use for box cast scan)
    
    private float truckLenght = 5f;         // Lenght of small truck cargo slot
    private int truckCargoLimit = 9;        // Limit amount of boxes in one truck
    private int cargoToUnload = 3;          // Limit amount of boxes to unload for each point

    private bool getTarget;                 // Flag shows that truck has a current target

    public GameObject targetCell;           // Waiting cell connected to the truck by manager

    private int i = 0;                      // Waypoints counter

    void FixedUpdate()
    {
        // Listening for flag moving to the waiting cell (by LoadingZoneManager)
        if (cellEnter && !getTarget)
            MoveToWaitingCell();
        
        // Listening loading manager to start moving to loading zone
        if (loadingEnter && !getTarget)
            MoveToLoadingZone();

        // Listening to start moving to warehouse parking zone
        if (parking && !getTarget)
            MoveToParkingZone();
      
        // Listening when truck will be ready to finish loading
        if (parked)
        {
            // Check is the truck full of boxes and start exit process
            if (CheckTruckCargo())
                StartCoroutine(WaitForExitLoadingZone());
        }

        // Listening to start delivery
        if (loaded && !getTarget)
            Delivery();

        // Listening to start search loading manager
        if (goToManager && !getTarget)
            SearchLoadingManager();            
        
        // Check for target
        if (GetTarget(parking))
        {
            // Change flags and stop moving
            parking = false;
            getTarget = false;
            GetComponent<CarAIControl>().m_Driving = false;
            GetComponent<CarAIControl>().moveBack = false;
            
            // Correct truck position for easy parking
            // Freeze truck
            transform.GetComponent<Rigidbody>().isKinematic = true;
            // Move truck to parking point
            transform.position = FindObjectOfType<LoadingZoneManager>().parkingPoint.transform.position;
            // Rotate truck
            transform.eulerAngles = new Vector3(0f, 180f, 0f);

            // Unfreeze truck
            transform.GetComponent<Rigidbody>().isKinematic = false;

            // Set parked flag
            parked = true;
        }
        else if (GetTarget(loaded))
        {
            // Change flags
            loaded = false;
            getTarget = true;

            // Check current point for unloading point
            foreach (var point in unloadingPoints)
            {
                if (GetComponent<CarAIControl>().m_Target.gameObject == point)
                {
                    StartCoroutine(Unloading());
                    break;
                }
            }

            // Return to delivery if there is not unloading point
            if (!unloading)
            {
                // Change flag and get next point
                loaded = true;
                getTarget = false;
                i++;
            }
        }
        else if (GetTarget(goToManager))
        {
            // Stop moving
            GetComponent<CarAIControl>().m_Driving = false;

            // Change flags
            goToManager = false;
            getTarget = false;
        }
        else if (GetTarget(cellEnter) || GetTarget(loadingEnter))
        {
            // Flag to start search next waypoint
            getTarget = false;
            i++;
        }
    }

    // Move to WAITING ZONE
    void MoveToWaitingCell()
    {
        // Get the waypoints from selected cell
        var cellEnterWay = targetCell.GetComponent<LoadingCell>().cellEnter;

        // Move until the last waypoint
        if (i < cellEnterWay.Count)
        {
            // Select waypoint as truck target
            GetComponent<CarAIControl>().m_Target = cellEnterWay[i].transform;
            getTarget = true;

            // Truck start moving
            TruckStartMoving(true);
        } 
        else
        {
            // Stop in last waypoint
            GetComponent<CarAIControl>().m_Driving = false;

            // Reset counter and change flag
            i = 0;
            cellEnter = false;
        }
    }

    // Move to LOADING ZONE
    void MoveToLoadingZone()
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
            loadingEnter = false;

            // Start parking to warehouse
            parking = true;
        }
    }

    // Move to PARKING ZONE
    void MoveToParkingZone()
    {
        // Select warehouse parking point as truck target
        GetComponent<CarAIControl>().m_Target = FindObjectOfType<LoadingZoneManager>().parkingPoint.transform;
        getTarget = true;

        // Truck start moving (back)
        TruckStartMoving(false);
    }

    // Delivery process
    void Delivery()
    {
        // Move until the last waypoint
        if (i < deliveryPoints.Count)
        {
            // Select waypoint as truck target
            GetComponent<CarAIControl>().m_Target = deliveryPoints[i].transform;
            getTarget = true;

            // Truck start moving
            TruckStartMoving(true);
        } 
        else
        {
            // Stop in last waypoint
            GetComponent<CarAIControl>().m_Driving = false;

            // Reset counter and change flag
            i = 0;
            loaded = false;
            getTarget = false;
            goToManager = true;
        }
    }

    // Unloading process
    IEnumerator Unloading()
    {
        unloading = true;

        // Stop moving
        GetComponent<CarAIControl>().m_Driving = false;

        yield return new WaitForSeconds(3f);

        // Use box cast to expand ray of search inside truck cargo slot
        RaycastHit[] targets = Physics.BoxCastAll(truckCargoScan.position, truckCargoScan.lossyScale, truckCargoScan.forward, truckCargoScan.rotation, truckLenght);
        
        int k = 1;

        if (targets.Length != 0)
        {
            // Check all items inside truck cargo slot
            foreach (var target in targets)
            {
                // Remove box each circle but not more than unload limit
                if (target.transform.tag == "box" && k <= cargoToUnload)
                {
                    Destroy(target.transform.gameObject);
                    k++;
                }
            }
        }

        yield return new WaitForSeconds(1f);

        // Change flag and get next point
        unloading = false;
        loaded = true;
        getTarget = false;
        i++;

        // Truck start moving
        TruckStartMoving(true);
    }

    // Go to Loading manager
    void SearchLoadingManager()
    {
        // Select loading manager as truck target
        GetComponent<CarAIControl>().m_Target = FindObjectOfType<LoadingZoneManager>().transform;
        getTarget = true;

        // Truck start moving
        TruckStartMoving(true);
    }

    // Check for get target
    bool GetTarget(bool checker)
    {
        // Calculate the local-relative position of the target
        var localTarget = transform.InverseTransformPoint(GetComponent<CarAIControl>().m_Target.position);
        
        // Listening for waypoint target
        if  (checker && getTarget && localTarget.magnitude < 2f)
            return true;
        return false;
    }

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

    // Waiting a little to start move to exit
    IEnumerator WaitForExitLoadingZone()
    {
        yield return new WaitForSeconds(3f);

        // Change flags
        parked = false;
        loaded = true;

        yield return new WaitForSeconds(6f);

        // Finish truck loading process, send flag to loading manager to start search next truck for loading
        FindObjectOfType<LoadingZoneManager>().loading = false;
    }

    // Inspect truck cargo slot. Looking for boxes inside.
    private bool CheckTruckCargo()
    {
        // Reset box counter
        int n = 0;

        // Use box cast to expand ray of search
        RaycastHit[] targets = Physics.BoxCastAll(truckCargoScan.position, truckCargoScan.lossyScale, truckCargoScan.forward, truckCargoScan.rotation, truckLenght);
        if (targets.Length != 0)
        {
            foreach (var target in targets)
            {
                // Fix: forklift can load the truck, so don't move while it happends 
                if (target.transform.tag == "forklift")
                {
                    n = 0;
                    break;
                }

                // Add box to counter
                if (target.transform.tag == "box")
                    n++;
            }
        }

        // Check for cargo limit
        if (n >= truckCargoLimit)
            return true;
        else
            return false;
    }
}