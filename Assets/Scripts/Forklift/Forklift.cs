using UnityEngine;

public class Forklift : MonoBehaviour
{
    public GameObject shifter;                  // Forklift shifter
    public GameObject lift;                     // Forklift lift

    private float speed = 0.5f;                 // Lift speed
    private float liftMaxHeight = 2f;           // Lift max height
    private float liftMinHeight = 0.636f;       // Lift min height

    private float liftX;                        // Saved X position of lift
    private float liftZ;                        // Saved Z position of lift

    private HingeJoint hinge;                   // Forklift hinge joint

    public float angleBetweenThreshold = 1f;    // Angle threshold to trigger if we reached limit
    private float angleWithMinLimit;            // Min limit angle for shifter, depends on hinge limits
    private float angleWithMaxLimit;            // Max limit angle for shifter, depends on hinge limits

    private bool limit;                         // Flag reach the limit

    void Awake()
    {
        hinge = shifter.GetComponent<HingeJoint>();

        // Calculate shifter limits
        angleWithMinLimit = shifter.transform.eulerAngles.x + hinge.limits.min;
        angleWithMaxLimit = shifter.transform.eulerAngles.x + hinge.limits.max;

        // Save lift local position X and Z
        liftX = lift.transform.localPosition.x;
        liftZ = lift.transform.localPosition.z;
    }

    void FixedUpdate()
    {
        // Hold lift local position X and Z
        lift.transform.localPosition = new Vector3(liftX, lift.transform.localPosition.y, liftZ);

        // Reached Min
        if(shifter.transform.eulerAngles.x - angleWithMinLimit < angleBetweenThreshold)
        {
            // Moving lift if there is no limit
            if (!limit)
                lift.transform.Translate(transform.up * speed * Time.deltaTime);
        }
        // Reached Max
        else if (angleWithMaxLimit - shifter.transform.eulerAngles.x < angleBetweenThreshold)
        {
            // Moving lift if there is no limit
            if (!limit)
                lift.transform.Translate(transform.up * -speed * Time.deltaTime);
        }
        else
            // No Limit reached
            limit = false;

        // Return minimum height to lift when exceeded limit
        if (!limit && lift.transform.localPosition.y < liftMinHeight)
        {
            lift.transform.localPosition = new Vector3(lift.transform.localPosition.x, liftMinHeight, lift.transform.localPosition.z);
            limit = true;
        }
        // Return maximum height to lift when exceeded limit
        if (!limit && lift.transform.localPosition.y > liftMaxHeight)
        {
            lift.transform.localPosition = new Vector3(lift.transform.localPosition.x, liftMaxHeight, lift.transform.localPosition.z);
            limit = true;
        }
    }
}