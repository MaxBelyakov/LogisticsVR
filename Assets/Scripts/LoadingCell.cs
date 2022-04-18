// Waiting Cell controller

using UnityEngine;
using System.Collections.Generic;

public class LoadingCell : MonoBehaviour
{
    public bool status;                             // Cell busy or free
    public GameObject waitingTruck;                 // Truck gameobject that is waiting by the cell 
    public GameObject readyForLoading;              // Truck gameobject connected to the Cell 

    public List<GameObject> cellEnter;              // The way from manager zone to the cell
    public List<GameObject> cellExit;               // The way from the cell to loading area

    void Start()
    {
        // All cells are free at the beggining
        status = true;
    }

    void OnTriggerEnter(Collider collider)
    {
        // Check for waiting truck
        if (collider.transform.parent.parent.gameObject == waitingTruck)
            readyForLoading = waitingTruck;
    }
}
