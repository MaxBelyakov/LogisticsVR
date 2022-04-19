using UnityEngine;
using System;

public class CallTruckButton : MonoBehaviour
{
    public GameObject lamp;
    public Light light1;
    public Light light2;
    public Material red;
    public Material green;

    private float threshold = 0.1f;
    private float deadZone = 0.025f;

    private bool isPressed;
    private Vector3 startPos;
    private ConfigurableJoint joint;

    public GameObject truckPrefab;
    public Transform truckRespawn;
    public Transform truckTarget;
    
    void Start()
    {
        startPos = transform.localPosition;
        joint = GetComponent<ConfigurableJoint>();
    }

    void Update()
    {
        if (GameObject.FindGameObjectWithTag("big truck") == null)
        {
            lamp.GetComponent<Renderer>().materials = new Material[] {lamp.GetComponent<Renderer>().materials[0], green};
            light1.color = Color.green;
            light2.color = Color.green;

            if (!isPressed && GetValue() + threshold >= 1f)
                Pressed();
        }
        else
        {
            lamp.GetComponent<Renderer>().materials = new Material[] {lamp.GetComponent<Renderer>().materials[0], red};
            light1.color = Color.red;
            light2.color = Color.red;
        }

        if (isPressed && GetValue() - threshold <= 0f)
            Released();
    }

    private float GetValue()
    {
        var value = Vector3.Distance(startPos, transform.localPosition) / joint.linearLimit.limit;

        if (Math.Abs(value) < deadZone)
            value = 0f;
        
        return Mathf.Clamp(value, -1f, 1f);
    }

    private void Pressed()
    {
        isPressed = true;
        lamp.GetComponent<Renderer>().materials = new Material[] {lamp.GetComponent<Renderer>().materials[0], red};
        light1.color = Color.red;
        light2.color = Color.red;

        GameObject newTruck = Instantiate(truckPrefab, truckRespawn.position, truckRespawn.rotation);
    }

    private void Released()
    {
        isPressed = false;
    }
}
