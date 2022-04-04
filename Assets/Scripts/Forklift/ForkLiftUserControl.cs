// Forklift rudder rotation and movement

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ForkLift
{
    [RequireComponent(typeof (ForkLiftController))]
    public class ForkLiftUserControl : MonoBehaviour
    {
        public GameObject rudder;
        public GameObject player;   // Player XR Rig
        public Transform seat;

        public XRController RightHandController;                  // Right hand ray
        public InputHelpers.Button backButton;        // Back button

        private bool driving;

        private float accel;        // Acceleration multiplicator

        private ForkLiftController m_Car; // the car controller we want to use

        private HingeJoint hinge;

        private float oldAngle;
        private float h;
        private bool limit;

        private XRGrabInteractable rudderGrab;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<ForkLiftController>();

            hinge = rudder.GetComponent<HingeJoint>();

            rudderGrab = rudder.GetComponent<XRGrabInteractable>();
            rudderGrab.selectEntered.AddListener(GetRudder);

            oldAngle = hinge.angle;

            // No move
            accel = 0f;
        }

        private void GetRudder(SelectEnterEventArgs arg0)
        {
            GameObject grab = new GameObject("Grab Pivot");
            grab.transform.SetParent(rudder.transform, false);

            grab.transform.position = arg0.interactorObject.transform.position;
            grab.transform.rotation = arg0.interactableObject.transform.rotation;

            FixedJoint joint = grab.AddComponent<FixedJoint>();
            joint.connectedBody = rudder.GetComponent<Rigidbody>();
            grab.GetComponent<Rigidbody>().useGravity = false;

            XRGrabInteractable newGrab = grab.AddComponent<XRGrabInteractable>();
            newGrab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
            newGrab.attachTransform = grab.transform;

            newGrab.selectEntered.AddListener(GetRudderPivot);
            newGrab.selectExited.AddListener(DropRudderPivot);
            newGrab.activated.AddListener(MoveForward);
            newGrab.deactivated.AddListener(StopMoving);
            
            
            rudderGrab.interactionManager.SelectExit(arg0.interactorObject, arg0.interactableObject);
            newGrab.interactionManager.SelectEnter(arg0.interactorObject, newGrab);
        }

        private void GetRudderPivot(SelectEnterEventArgs arg0)
        {
            driving = true;

            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }

        private void DropRudderPivot(SelectExitEventArgs arg0)
        {
            Destroy(arg0.interactableObject.transform.gameObject);

            driving = false;
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            accel = 0f;
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
            if (driving)
            {
                player.transform.position = seat.position;

               // Moving back
                if (RightHandController)
                    if (CheckIfPressedBack(RightHandController))
                        accel = -1f;
            }

            // Rudder rotation
            if (Mathf.RoundToInt(hinge.angle) > Mathf.RoundToInt(oldAngle))
            {
                if (!limit)
                    h += 0.05f;
            }
            else if (Mathf.RoundToInt(hinge.angle) < Mathf.RoundToInt(oldAngle))
            {
                if (!limit)
                    h -= 0.05f;
            }
            else
                limit = false;

            oldAngle = hinge.angle;

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
            m_Car.Move(h, accel, accel, 0f);
        }

        private bool CheckIfPressedBack(XRController controller)
        {
            // Listening for activation button press with activation threshold
            InputHelpers.IsPressed(controller.inputDevice, backButton, out bool isActivated, 0.1f);
            return isActivated;
        }
    }
}