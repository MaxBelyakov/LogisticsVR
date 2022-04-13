using UnityEngine;

public class TruckExitZone : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        // Check for truck tag and exit flag. Destroy truck
        if (collider.transform.tag == "big truck" && collider.GetComponentInParent<Truck>().loadingExit)
            Destroy(collider.GetComponentInParent<Truck>().transform.gameObject);
    }
}
