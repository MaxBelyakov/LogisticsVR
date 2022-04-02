using UnityEngine;

public class Forklift : MonoBehaviour
{
    public GameObject shifter;      // Forklift shifter
    public GameObject lift;         // Forklift lift

    [SerializeField] private float minShifterAngle = -90f;      // Min shifter rotation angle
    [SerializeField] private float maxShifterAngle = -50f;      // Max shifter rotation angle

    void Update()
    {
        // Get shifter current rotation
        var rotation = shifter.transform.rotation.eulerAngles;

        // Correct the parametrs
        rotation.x = (rotation.x > 180) ? rotation.x - 360 : rotation.x;

        // Get the rotation axis to move lift linery (calculated for minLiftHeigth = 0.636f, maxLiftHeigth = 2f)
        var y = (-42.76f - 1.3639999999999999f * rotation.x) / 40f;
        lift.transform.localPosition = new Vector3(lift.transform.localPosition.x, y, lift.transform.localPosition.z);
    }
}
