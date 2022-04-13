// Forklift rudder rotation and movement

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;

namespace ForkLift
{
    public class ForkLiftUserControl : MonoBehaviour
    {
        [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];

        public GameObject forklift;
        public GameObject rudder;
        public List<GameObject> rudderPoints;
        public GameObject player;   // Player XR Rig
        public Transform playerCamera;
        public Transform seat;
        private GameObject currentSeat;

        public XRController RightHandController;                  // Right hand ray
        public InputHelpers.Button backButton;        // Back button

        private bool driving;
        private bool moveBackFlag;

        private bool inertia;

        private float accel;        // Acceleration multiplicator
        private float footbrake;

        private HingeJoint hinge;

        private float oldAngle;
        private float h;
        private bool limit;

        private XRGrabInteractable rudderGrab;

        private void Awake()
        {
            hinge = rudder.GetComponent<HingeJoint>();

            rudderGrab = rudder.GetComponent<XRGrabInteractable>();
            rudderGrab.selectEntered.AddListener(GetRudder);

            oldAngle = hinge.angle;

            // No move
            accel = 0f;
            footbrake = 0f;
        }

        private void GetRudder(SelectEnterEventArgs arg0)
        {
            GameObject grab = new GameObject("Grab Pivot");
            grab.transform.SetParent(rudder.transform.parent, false);

            grab.transform.position = arg0.interactorObject.transform.position;
            grab.transform.eulerAngles = new Vector3(arg0.interactableObject.transform.eulerAngles.x + 180f, arg0.interactableObject.transform.eulerAngles.y, arg0.interactableObject.transform.eulerAngles.z);

            FixedJoint joint = grab.AddComponent<FixedJoint>();
            joint.connectedBody = rudder.GetComponent<Rigidbody>();
            grab.GetComponent<Rigidbody>().useGravity = false;

            XRGrabInteractable newGrab = grab.AddComponent<test>();
            newGrab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
            newGrab.GetComponent<test>().rud = rudder;
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

            currentSeat = new GameObject("current seat");
            currentSeat.transform.SetParent(seat, false);
            currentSeat.transform.position = player.transform.position;

            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }

        private void DropRudderPivot(SelectExitEventArgs arg0)
        {
            Destroy(arg0.interactableObject.transform.gameObject);

            accel = 0f;
            footbrake = 1f;
            moveBackFlag = false;

            driving = false;

            StartCoroutine(WaitForPlayerInertia());
        }

        private void MoveForward(ActivateEventArgs arg0)
        {
            accel = 1f;
            footbrake = 0f;
        }

        private void StopMoving(DeactivateEventArgs arg0)
        {
            accel = 0f;
            footbrake = 1f;
        }

        private void MoveBack()
        {
            accel = -1f;
            footbrake = 0f;
            moveBackFlag = true;
        }

        private void StopMoveBack()
        {
            accel = 0f;
            footbrake = 1f;
            moveBackFlag = false;
        }

        private void FixedUpdate()
        {
            if (driving || inertia)
            {
                player.transform.position = currentSeat.transform.position;
                player.transform.rotation = seat.transform.rotation;

               // Moving back
                if (RightHandController)
                    if (CheckIfPressedBack(RightHandController) && !inertia)
                        MoveBack();
                    else if (moveBackFlag)
                        StopMoveBack();
            }

            // Rudder rotation
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
            Move(h, accel, footbrake);
        }

        private bool CheckIfPressedBack(XRController controller)
        {
            // Listening for activation button press with activation threshold
            InputHelpers.IsPressed(controller.inputDevice, backButton, out bool isActivated, 0.1f);
            return isActivated;
        }

        public void Move(float steering, float accel, float footbrake)
        {
            //Set the steer on the front wheels.
            //Assuming that wheels 0 and 1 are the front wheels.
            float m_MaximumSteerAngle = 35f;
            float m_SteerAngle = steering*m_MaximumSteerAngle;
            m_WheelColliders[0].steerAngle = m_SteerAngle;
            m_WheelColliders[1].steerAngle = m_SteerAngle;

            for (int i = 0; i < 4; i++)
            {
                Quaternion quat;
                Vector3 position;
                m_WheelColliders[i].GetWorldPose(out position, out quat);
                m_WheelMeshes[i].transform.position = position;
                m_WheelMeshes[i].transform.rotation = quat;
            }

            for (int i = 0; i < 4; i++)
            {
                m_WheelColliders[i].motorTorque = accel * 4000f;
                m_WheelColliders[i].brakeTorque = footbrake * 100000f;
            }

            Rigidbody rb = forklift.GetComponent<Rigidbody>();
            float topSpeed = 0.5f;
            float speed = rb.velocity.magnitude;

            if (speed > topSpeed)
                rb.velocity = topSpeed * rb.velocity.normalized;
        }

        private IEnumerator WaitForPlayerInertia()
        {
            inertia = true;
            yield return new WaitForSeconds(1f);

            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            inertia = false;
        }
    }
}