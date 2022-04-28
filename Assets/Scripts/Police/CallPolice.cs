// Script generates police car in respawn point by random time

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallPolice : MonoBehaviour
{
    public GameObject policeCarPrefab;          // Police car prefab

    private bool waitingForPolice;              // Flag shows that police is already called

    public List<GameObject> enterPoints;        // The way from respawn to warehouse
    public List<GameObject> exitPoints;         // The exit way

    void FixedUpdate()
    {
        // Random call police
        if (GameObject.FindGameObjectWithTag("police car") == null && waitingForPolice == false)
            StartCoroutine(CallPoliceWaiter());
    }

    // Waiting for police
    IEnumerator CallPoliceWaiter()
    {
        waitingForPolice = true;

        // Random police waiting time
        float t = Random.Range(30f, 60f);

        yield return new WaitForSeconds(t);

        // Respawn police car
        GameObject policeCar = Instantiate(policeCarPrefab, transform.position, transform.rotation);

        // Send request to police car
        policeCar.GetComponent<PoliceCar>().policeManager = gameObject;
        policeCar.GetComponent<PoliceCar>().policeEnter = true;

        waitingForPolice = false;
    }
}