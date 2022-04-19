// Loading zone manager. Before truck enter the loading zone manager check it

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingZoneManager2 : MonoBehaviour
{
    public List<Transform> enterWaypoints;      // Enter to loading zone waypoints
    public List<Transform> exitWaypoints;       // Exit from loading zone waypoints
    public GameObject parkingPoint;             // Warehouse parking point

    private bool truckStart;                    // Flag shows truck start searching waypoints (enought just 1 trigger)
    private Collider waitArrested;               // Save triggered collider if in moment of trigger police arrest warehouse

    void FixedUpdate()
    {
        // Check the case if police arrested warehouse, get saved collider
        if (waitArrested != null)
            if (!GameObject.FindGameObjectWithTag("loading zone manager").GetComponent<LoadingZoneManager>().arrested)
                StartCoroutine(LoadingManagerWaiter(waitArrested));
    }

    // Truck collide with loading manager. Manager check loading zone
    void OnTriggerEnter(Collider collider)
    {
        // Check for truck tag and loading Enter flag (to collide just once)
        if (collider.transform.tag == "big truck" && !collider.GetComponentInParent<Truck>().loadingEnter)
        {
            // Check truck going to unload cargo and police not arrested warehouse
            if (!truckStart && collider.GetComponentInParent<Truck>().haveCargo
            && !GameObject.FindGameObjectWithTag("loading zone manager").GetComponent<LoadingZoneManager>().arrested)
            {
                // Truck start searching waypoints
                truckStart = true;

                // Wait for loading zone ready
                StartCoroutine(LoadingManagerWaiter(collider));
            }

            // Save trigger collider if police arrested warehouse
            if (GameObject.FindGameObjectWithTag("loading zone manager").GetComponent<LoadingZoneManager>().arrested)
                waitArrested = collider;
        }
    }

    // Wait before start moving to the loading zone
    IEnumerator LoadingManagerWaiter(Collider collider)
    {
        yield return new WaitForSeconds(1f);

        // Send enter waypoints to the truck
        collider.GetComponentInParent<Truck>().waypoints = enterWaypoints;

        // Send request to truck move to the loading zone
        collider.GetComponentInParent<Truck>().loadingEnter = true;
    }
}