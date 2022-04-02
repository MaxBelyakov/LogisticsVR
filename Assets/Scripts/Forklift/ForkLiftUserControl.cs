// Forklift rudder rotation and movement

using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ForkLift
{
    [RequireComponent(typeof (ForkLiftController))]
    public class ForkLiftUserControl : MonoBehaviour
    {
        public GameObject rudder;   // Forklift rudder
        public GameObject player;   // Player XR Rig

        private float accel;        // Acceleration multiplicator

        [SerializeField] private float minRudderAngle = -35f;   // Min rudder rotation angle
        [SerializeField] private float maxRudderAngle = 35f;    // Max rudder rotation angle

        private ForkLiftController m_Car; // the car controller we want to use

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<ForkLiftController>();

            // No move
            accel = 0f;

            // Connect to rudder grab script and set the listeners
            XRGrabInteractable rudderGrab = rudder.GetComponent<XRGrabInteractable>();
            rudderGrab.selectEntered.AddListener(GetRudder);
            rudderGrab.selectExited.AddListener(DropRudder);
            rudderGrab.activated.AddListener(MoveForward);
            rudderGrab.deactivated.AddListener(StopMoving);
        }

        private void GetRudder(SelectEnterEventArgs arg0)
        {
            player.GetComponent<Rigidbody>().isKinematic = true;
        }

        private void DropRudder(SelectExitEventArgs arg0)
        {
            player.GetComponent<Rigidbody>().isKinematic = false;
        }

        private void MoveForward(ActivateEventArgs arg0)
        {
            accel = 1f;
        }

        private void StopMoving(DeactivateEventArgs arg0)
        {
            accel = 0f;
        }

        private void FixedUpdate()
        {

            // Get rudder current rotation
            var rotation = rudder.transform.rotation.eulerAngles;

            // Correct the parametrs
            rotation.z = (rotation.z > 180) ? rotation.z - 360 : rotation.z;

            // Set the rotation limits
            //rotation.z = Mathf.Clamp(rotation.z, minRudderAngle, maxRudderAngle);

            // Update rudder rotation
            //rudder.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);

            // Get the rotation axis to turn the wheels linery
            var h = rotation.z / 35f;

            // Turn the wheels and move
            m_Car.Move(h, accel, accel, 0f);
        }
    }
}