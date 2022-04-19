using UnityEngine;

public class ExitZone : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        // Check for truck tag and exit flag. Destroy truck
        if (collider.transform.tag == "big truck" && collider.GetComponentInParent<Truck>().loadingExit)
            Destroy(collider.GetComponentInParent<Truck>().transform.gameObject);

        // Check for police tag and exit flag. Destroy police
        if (collider.transform.tag == "police car" && collider.GetComponentInParent<PoliceCar>().policeExit)
            Destroy(collider.GetComponentInParent<PoliceCar>().transform.gameObject);
    }
}
