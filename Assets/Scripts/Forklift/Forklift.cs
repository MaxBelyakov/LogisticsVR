using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Forklift : MonoBehaviour
{
    public GameObject shifter;      // Forklift shifter
    public GameObject lift;         // Forklift lift

    //private bool lifting;

    private HingeJoint hinge;

    //angle threshold to trigger if we reached limit
    public float angleBetweenThreshold = 1f;

    private bool limit;

    void Awake()
    {
        hinge = shifter.GetComponent<HingeJoint>();
    }

    void FixedUpdate()
    {
        float angleWithMinLimit = Mathf.Abs(hinge.angle - hinge.limits.min);
        float angleWithMaxLimit = Mathf.Abs(hinge.angle - hinge.limits.max);

        //Reached Max
        if(angleWithMinLimit < angleBetweenThreshold)
        {
            if (!limit)
                lift.transform.Translate(transform.up*1f*Time.deltaTime);
                //lift.GetComponent<Rigidbody>().MovePosition(lift.GetComponent<Rigidbody>().position + transform.up*1f*Time.deltaTime);
        }
        //Reached Min
        else if (angleWithMaxLimit < angleBetweenThreshold)
        {
            if (!limit)
                lift.transform.Translate(transform.up*-1f*Time.deltaTime);
                //lift.GetComponent<Rigidbody>().MovePosition(lift.GetComponent<Rigidbody>().position + transform.up*-1f*Time.deltaTime);
        }
        //No Limit reached
        else
            limit = false;

        if (!limit && lift.transform.localPosition.y < 0.636f)
        {
            lift.transform.localPosition = new Vector3(lift.transform.localPosition.x, 0.636f, lift.transform.localPosition.z);
            limit = true;
        }
        if (!limit && lift.transform.localPosition.y > 2f)
        {
            lift.transform.localPosition = new Vector3(lift.transform.localPosition.x, 2f, lift.transform.localPosition.z);
            limit = true;
        }
    }
}
