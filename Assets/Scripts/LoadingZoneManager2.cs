// Loading zone manager. Before truck enter the loading zone manager check it

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingZoneManager2 : MonoBehaviour
{
    public List<Transform> enterWaypoints;      // Enter to loading zone waypoints
    public List<Transform> exitWaypoints;       // Exit from loading zone waypoints
    public GameObject parkingPoint;             // Warehouse parking point

    // Truck collide with loading manager. Manager check loading zone
    void OnTriggerEnter(Collider collider)
    {
        // Check for truck tag and loading Enter flag (to collide just once)
        if (collider.transform.tag == "big truck" && !collider.GetComponentInParent<Truck>().loadingEnter)
        {
            // Check truck going to unload cargo
            if (collider.GetComponentInParent<Truck>().haveCargo)
            {
                // Stop driving
                collider.GetComponentInParent<Trucks.TruckAIControl>().m_Driving = false;

                // FIXME. Wait for empty loading zone
                StartCoroutine(LoadingManagerWaiter(collider));
            }
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