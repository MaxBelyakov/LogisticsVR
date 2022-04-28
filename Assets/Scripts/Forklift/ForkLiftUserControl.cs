// Forklift rudder rotation and movement

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;

namespace ForkLift
{
    public class ForkLiftUserControl : MonoBehaviour
    {
        [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];           // Wheel colliders
        [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];                    // Wheel meshes

        public GameObject forklift;                     // Forklift
        public GameObject rudder;                       // Rudder
        public List<GameObject> rudderPoints;           // Rudder attach points
        public GameObject player;                       // Player XR Rig
        public Transform seat;                          // Forklift seat position
        private GameObject currentSeat;                 // Player in XR can move after seat, need to save position when get rudder

        private ActionBasedController controller;       // Controller that get rudder
        private HingeJoint hinge;                       // Rudder joint

        private MonoBehaviour snapTurn;                 // Snap Turn script (need to switch on/off)

        private float m_MaximumSteerAngle = 35f;        // Maximum wheels rotation angle
        private float topSpeed = 0.5f;                  // Forklift maximum speed

        private bool driving;                           // Flag that forklift is driving by player
        private bool inertia;                           // Flag show inertia when forklift stop 

        private float accel;                            // Acceleration value
        private float footbrake;                        // Brake value
        public Vector3 m_CentreOfMassOffset;            // Decentralization center of mass

        private float oldAngle;                         // Save rudder angle variable
        private float h;                                // Save rudder angle counter
        private bool limit;                             // Flag shows that wheels in limit position

        private void Awake()
        {
            // Get Snap Turn Script
            snapTurn = GameObject.FindGameObjectWithTag("Player").GetComponent<ActionBasedSnapTurnProvider>();

            // Get rudder hinge joint
            hinge = rudder.GetComponent<HingeJoint>();

            // Add listeners for each rudder attach point
            foreach (var point in rudderPoints)
            {
                XRGrabInteractable rudderGrab = point.GetComponent<XRGrabInteractable>();
                rudderGrab.selectEntered.AddListener(GetRudder);
                rudderGrab.selectExited.AddListener(DropRudder);
            }

            // Save rudder hinge angle
            oldAngle = hinge.angle;

            // No move
            accel = 0f;
            footbrake = 0f;

            // Set offset center of mass (to avoid forklift spring when get cargo)
            m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;
        }

        // Listener. Get Rudder
        private void GetRudder(SelectEnterEventArgs arg0)
        {
            // Start driving
            driving = true;

            // Get controller
            controller = arg0.interactorObject.transform.GetComponent<ActionBasedController>();

            // Create new seat position to freeze player there. 
            // Player in XR can move after seat, need to save position when get rudder
            currentSeat = new GameObject("current seat");
            currentSeat.transform.SetParent(seat, false);
            currentSeat.transform.position = player.transform.position;

            // Unblock players rotation while riding forklift
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        // Listener. Drop rudder
        private void DropRudder(SelectExitEventArgs arg0)
        {
            // Stop forklift
            accel = 0f;
            footbrake = 1f;

            // Stop driving
            driving = false;

            // Wait for inertia
            StartCoroutine(WaitForPlayerInertia());
        }

        private void FixedUpdate()
        {
            // Driving forklift
            if (driving || inertia)
            {
                // Hold player on saved seat position
                player.transform.position = currentSeat.transform.position;

                // Disable player snap turn
                snapTurn.enabled = false;

                // Get acceleration by reading controller button value
                if (controller && !inertia)
                    accel = controller.translateAnchorAction.action.ReadValue<Vector2>().y;

                    // Stop when no acceleration
                    if (accel != 0)
                        footbrake = 0f;
                    else
                        footbrake = 1f;
            }
            else if (snapTurn.enabled == false)
                // Enable player snap turn
                snapTurn.enabled = true;

            // Rotate wheels with rudder by compare old and new rudder angle
            if (Mathf.RoundToInt(hinge.angle) > Mathf.RoundToInt(oldAngle))
            {
                if (!limit)
                    h += 0.01f;
            }
            else if (Mathf.RoundToInt(hinge.angle) < Mathf.RoundToInt(oldAngle))
            {
                if (!limit)
                    h -= 0.01f;
            }
            else
                limit = false;

            // Renew old rudder angle
            oldAngle = hinge.angle;

            // Looking for limit angle
            if (!limit && h < -1f)
            {
                h = -1f;
                limit = true;
            }
            if (!limit && h > 1f)
            {
                h = 1f;
                limit = true;
            }

            // Turn the wheels and move
            Move(h, accel, footbrake);
        }

        // Forklft moving
        public void Move(float steering, float accel, float footbrake)
        {
            // Set the steer on the front wheels.
            // Assuming that wheels 0 and 1 are the front wheels.
            float m_SteerAngle = steering * m_MaximumSteerAngle;
            m_WheelColliders[0].steerAngle = m_SteerAngle;
            m_WheelColliders[1].steerAngle = m_SteerAngle;

            // Move and rotate wheels meshes
            for (int i = 0; i < 4; i++)
            {
                Quaternion quat;
                Vector3 position;
                m_WheelColliders[i].GetWorldPose(out position, out quat);
                m_WheelMeshes[i].transform.position = position;
                m_WheelMeshes[i].transform.rotation = quat;
            }

            // Add force to wheels
            for (int i = 0; i < 4; i++)
            {
                m_WheelColliders[i].motorTorque = accel * 4000f;
                m_WheelColliders[i].brakeTorque = footbrake * 100000f;
            }

            // Get current forklift speed
            Rigidbody rb = forklift.GetComponent<Rigidbody>();
            float speed = rb.velocity.magnitude;

            // Set limit speed
            if (speed > topSpeed)
                rb.velocity = topSpeed * rb.velocity.normalized;
        }

        // Wait for forklift inertia after break
        private IEnumerator WaitForPlayerInertia()
        {
            inertia = true;
            yield return new WaitForSeconds(0.3f);

            // Return blocking player rig rotation
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

            inertia = false;
        }
    }
}