// Script make active teleportation ray when primary button is pressed

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionManager : MonoBehaviour
{
    public XRController LeftHandRayController;                  // Left hand ray
    public InputHelpers.Button teleportActivationButton;        // Activation button

    public float activationThreshold = 0.1f;                    // Button threshold

    void Update()
    {
        // Activate/Deactivate teleportation ray
        if (LeftHandRayController)
            LeftHandRayController.gameObject.SetActive(CheckIfActivated(LeftHandRayController));
    }

    private bool CheckIfActivated(XRController controller)
    {
        // Listening for activation button press with activation threshold
        InputHelpers.IsPressed(controller.inputDevice, teleportActivationButton, out bool isActivated, activationThreshold);
        return isActivated;
    }
}