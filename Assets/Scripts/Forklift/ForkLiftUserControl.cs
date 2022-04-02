// Forklift rudder rotation and movement

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ForkLift
{
    [RequireComponent(typeof (ForkLiftController))]
    public class ForkLiftUserControl : MonoBehaviour
    {
        public GameObject rudder;
        public List<GameObject> rudderElements;   // Forklift rudder
        public GameObject player;   // Player XR Rig
        public Transform seat;

        public XRController RightHandController;                  // Right hand ray
        public InputHelpers.Button backButton;        // Back button

        private bool driving;

        private float accel;        // Acceleration multiplicator

        [SerializeField] private float minRudderAngle = 40f;   // Min rudder rotation angle
        [SerializeField] private float maxRudderAngle = 160f;    // Max rudder rotation angle

        private ForkLiftController m_Car; // the car controller we want to use

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<ForkLiftController>();

            // No move
            accel = 0f;

            // Connect to rudder grab script and set the listeners
            for (int i = 0; i < rudderElements.Count; i++)
            {
                XRGrabInteractable rudderGrab = rudderElements[i].GetComponent<XRGrabInteractable>();
                rudderGrab.selectEntered.AddListener(GetRudder);
                rudderGrab.selectExited.AddListener(DropRudder);
                rudderGrab.activated.AddListener(MoveForward);
                rudderGrab.deactivated.AddListener(StopMoving);
            }
        }

        private void GetRudder(SelectEnterEventArgs arg0)
        {
            driving = true;
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }

        private void DropRudder(SelectExitEventArgs arg0)
        {
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

           // print(player.transform.position + " / " + seat.position);
            if (driving)
            {
                player.transform.position = seat.position;
               //player.transform.rotation = seat.rotation;

               // Moving back
                if (RightHandController)
                    if (CheckIfPressedBack(RightHandController))
                        accel *= -1f;
            }

            // Get rudder current rotation
            var rotation = rudder.transform.rotation.eulerAngles;

            if (rotation.z > 270f)
                rudder.transform.eulerAngles = new Vector3(rotation.x, rotation.y, 270f);
            if (rotation.z < 90f)
                rudder.transform.eulerAngles = new Vector3(rotation.x, rotation.y, 90f);

            // Get the rotation axis to turn the wheels linery
            var h = (rotation.z / 90f) - 2f;

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