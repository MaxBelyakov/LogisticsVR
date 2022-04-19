// Loading zone manager. Before enter the waiting area trucks managed by the waiting cells

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingZoneManager : MonoBehaviour
{
    public List<GameObject> cells;              // Waiting cells

    public bool loading;                        // Flag shows that any truck start moving to loading zone
    public bool arrested;                        // Flag police car arested trucks

    public GameObject parkingPoint;             // Warehouse parking point
    public GameObject exit;                     // For testing

    void Update()
    {
        // Check for waiting trucks get ready to load when no moving in loading zone
        if (!loading && !arrested)
        {
            // Check all waiting cells
            for (var i = 0; i < cells.Count; i++)
            {
                // Get truck gameobject connected to the cell
                GameObject truck = cells[i].GetComponent<LoadingCell>().readyForLoading;

                // Find the truck in the waiting cell
                if (truck != null && !truck.GetComponent<SmallTruckController>().cellEnter)
                {
                    // Send request to truck to move in loading zone
                    cells[i].GetComponent<LoadingCell>().readyForLoading.GetComponent<SmallTruckController>().loadingEnter = true;
                    loading = true;
                    break;
                }
            }
        }
    }

    // Truck collide with loading manager. Manager select empty cell
    void OnTriggerEnter(Collider collider)
    {
        // Check for truck tag
        if (collider.transform.tag == "small truck")
        {
            // Check for empty cell
            for (var i = 0; i < cells.Count; i++)
            {
                if (cells[i].GetComponent<LoadingCell>() != null && cells[i].GetComponent<LoadingCell>().status == true)
                {
                    // Send to truck target cell gameobject
                    collider.GetComponentInParent<SmallTruckController>().targetCell = cells[i];

                    // Connect the cell with truck gameobject
                    cells[i].GetComponent<LoadingCell>().waitingTruck = collider.transform.parent.parent.gameObject;

                    // Mark the cell as busy
                    cells[i].GetComponent<LoadingCell>().status = false;

                    // Wait before start moving to the target cell
                    StartCoroutine(LoadingManagerWaiter(collider));

                    break;
                }
            }
        }
    }

    // Wait before start moving to the target cell
    IEnumerator LoadingManagerWaiter(Collider collider)
    {
        yield return new WaitForSeconds(1f);

        // Send request to truck move to the target cell
        collider.GetComponentInParent<SmallTruckController>().cellEnter = true;
    }
}
