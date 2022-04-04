// Loading zone manager. Before enter the waiting area trucks managed by the waiting cells

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingZoneManager2 : MonoBehaviour
{
    public List<Transform> enterWaypoints;
    //public List<Transform> exitWaypoints;

    // Truck collide with loading manager. Manager select empty cell
    void OnTriggerEnter(Collider collider)
    {
        // Check for truck tag
        if (collider.transform.tag == "big truck" && !collider.GetComponentInParent<Truck>().loadingEnter)
        {
            if (collider.GetComponentInParent<Truck>().haveCargo)
            {
                // Stop driving
                collider.GetComponentInParent<Trucks.TruckAIControl>().m_Driving = false;

                // Wait for empty loading zone

                // Wait before start moving by waypoints
                collider.GetComponentInParent<Truck>().waypoints = enterWaypoints;
                StartCoroutine(LoadingManagerWaiter(collider));
            }
        }
    }

    // Wait before start moving to the target cell
    IEnumerator LoadingManagerWaiter(Collider collider)
    {
        yield return new WaitForSeconds(1f);

        // Send request to truck move to the target cell
        collider.GetComponentInParent<Truck>().loadingEnter = true;
    }
}
