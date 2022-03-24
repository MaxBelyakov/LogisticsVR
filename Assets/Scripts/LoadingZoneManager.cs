// Loading zone manager. Before enter the waiting area trucks managed by the waiting cells

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingZoneManager : MonoBehaviour
{
    public List<GameObject> cells;              // Waiting cells

    public bool loading;                        // Flag shows that any truck start moving to loading zone

    public GameObject parkingPoint;             // Warehouse parking point
    public GameObject exit;                     // For testing

    void Update()
    {
        // Check for waiting trucks get ready to load when no moving in loading zone
        if (!loading)
        {
            // Check all waiting cells
            for (var i = 0; i < cells.Count; i++)
            {
                // Find the truck in the waiting cell
                if (cells[i].GetComponent<LoadingCell>().readyForLoading != null)
                {
                    // Send request to truck to move in loading zone
                    cells[i].GetComponent<LoadingCell>().readyForLoading.GetComponent<SmallTruckController>().loadingExit = true;
                    loading = true;
                    break;
                }
            }
        }

        // Block cells if previous cell in line is busy
        // Unblock cells if previous cell in line become free
        if (cells[2].GetComponent<LoadingCell>().status == false)
        {
            cells[1].GetComponent<LoadingCell>().status = false;
            cells[0].GetComponent<LoadingCell>().status = false;
        }
        else
            if (cells[1].GetComponent<LoadingCell>().readyForLoading == null)
                cells[1].GetComponent<LoadingCell>().status = true;

        if (cells[1].GetComponent<LoadingCell>().status == false)
            cells[0].GetComponent<LoadingCell>().status = false;
        else
            if (cells[0].GetComponent<LoadingCell>().readyForLoading == null)
                cells[0].GetComponent<LoadingCell>().status = true;

        if (cells[5].GetComponent<LoadingCell>().status == false)
        {
            cells[4].GetComponent<LoadingCell>().status = false;
            cells[3].GetComponent<LoadingCell>().status = false;
        }
        else
            if (cells[4].GetComponent<LoadingCell>().readyForLoading == null)
                cells[4].GetComponent<LoadingCell>().status = true;

        if (cells[4].GetComponent<LoadingCell>().status == false)
            cells[3].GetComponent<LoadingCell>().status = false;
        else
            if (cells[3].GetComponent<LoadingCell>().readyForLoading == null)
                cells[3].GetComponent<LoadingCell>().status = true;

        if (cells[8].GetComponent<LoadingCell>().status == false)
        {
            cells[7].GetComponent<LoadingCell>().status = false;
            cells[6].GetComponent<LoadingCell>().status = false;
        }
        else
            if (cells[7].GetComponent<LoadingCell>().readyForLoading == null)
                cells[7].GetComponent<LoadingCell>().status = true;

        if (cells[7].GetComponent<LoadingCell>().status == false)
            cells[6].GetComponent<LoadingCell>().status = false;
        else
            if (cells[6].GetComponent<LoadingCell>().readyForLoading == null)
                cells[6].GetComponent<LoadingCell>().status = true;

        if (cells[11].GetComponent<LoadingCell>().status == false)
        {
            cells[10].GetComponent<LoadingCell>().status = false;
            cells[9].GetComponent<LoadingCell>().status = false;
        }
        else
            if (cells[10].GetComponent<LoadingCell>().readyForLoading == null)
                cells[10].GetComponent<LoadingCell>().status = true;

        if (cells[10].GetComponent<LoadingCell>().status == false)
            cells[9].GetComponent<LoadingCell>().status = false;
        else
            if (cells[9].GetComponent<LoadingCell>().readyForLoading == null)
                cells[9].GetComponent<LoadingCell>().status = true;
    }

    // Truck collide with loading manager. Manager select empty cell
    void OnTriggerEnter(Collider collider)
    {
        // Check for truck tag
        if (collider.transform.tag == "small truck")
        {
            // Stop driving
            collider.GetComponentInParent<UnityStandardAssets.Vehicles.Car.CarAIControl>().m_Driving = false;

            // Check for empty cell
            for (var i = 0; i < cells.Count; i++)
            {
                if (cells[i].GetComponent<LoadingCell>() != null && cells[i].GetComponent<LoadingCell>().status == true)
                {
                    // Send to truck target cell gameobject
                    collider.GetComponentInParent<SmallTruckController>().targetCell = cells[i];

                    // Connect the cell with truck gameobject
                    cells[i].GetComponent<LoadingCell>().readyForLoading = collider.transform.parent.parent.gameObject;

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
        collider.GetComponentInParent<SmallTruckController>().loadingEnter = true;
    }
}
