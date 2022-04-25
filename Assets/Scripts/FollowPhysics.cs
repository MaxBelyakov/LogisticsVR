// Grabbed Rigidbody follow the target (for example grabbed virtual object follow for door handle)

using UnityEngine;

public class FollowPhysics : MonoBehaviour
{
    public Transform target;                // Target
    Rigidbody rb;                           // Rigidbody
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.MovePosition(target.transform.position);
    }
}
