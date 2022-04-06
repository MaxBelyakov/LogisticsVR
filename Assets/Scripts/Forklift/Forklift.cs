using UnityEngine;

public class Forklift : MonoBehaviour
{
    public GameObject shifter;      // Forklift shifter
    public GameObject lift;         // Forklift lift

    private HingeJoint hinge;

    //angle threshold to trigger if we reached limit
    public float angleBetweenThreshold = 1f;
    private float angleWithMinLimit;
    private float angleWithMaxLimit;

    private bool limit;

    void Awake()
    {
        hinge = shifter.GetComponent<HingeJoint>();
        angleWithMinLimit = shifter.transform.eulerAngles.x + hinge.limits.min;
        angleWithMaxLimit = shifter.transform.eulerAngles.x + hinge.limits.max;
    }

    void FixedUpdate()
    {
        print(angleWithMinLimit + "/" + angleWithMaxLimit + "/" + shifter.transform.eulerAngles.x);
        //Reached Max
        if(shifter.transform.eulerAngles.x - angleWithMinLimit < angleBetweenThreshold)
        {
            print("min");
            if (!limit)
                lift.transform.Translate(transform.up*1f*Time.deltaTime);
        }
        //Reached Min
        else if (angleWithMaxLimit - shifter.transform.eulerAngles.x < angleBetweenThreshold)
        {
            print("max");
            if (!limit)
                lift.transform.Translate(transform.up*-1f*Time.deltaTime);
        }
        //No Limit reached
        else
            limit = false;

        if (!limit && lift.transform.localPosition.y < 0.636f)
        {
            print("limit min");
            lift.transform.localPosition = new Vector3(lift.transform.localPosition.x, 0.636f, lift.transform.localPosition.z);
            limit = true;
        }
        if (!limit && lift.transform.localPosition.y > 2f)
        {
            print("limit max");
            lift.transform.localPosition = new Vector3(lift.transform.localPosition.x, 2f, lift.transform.localPosition.z);
            limit = true;
        }
    }
}
