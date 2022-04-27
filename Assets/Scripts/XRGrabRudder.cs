// XR Rudder interactable controller

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabRudder : XRGrabInteractable
{
    public Transform rudder;                        // Rudder

    private IXRSelectInteractor interactor;         // Hand interaction object
    private IXRSelectInteractable interactable;     // Rudder interactable object

    private void Update()
    {
        // Drop the rudder if there is a distance between hand and rudder
        if (Vector3.Distance(rudder.position, transform.position) > 0.4f)
            interactionManager.SelectExit(interactor, interactable);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Save interactor and interactable objects
        interactor = args.interactorObject;
        interactable = args.interactableObject;

        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        // Stop rudder rotataing when dropped
        rudder.GetComponent<Rigidbody>().velocity = Vector3.zero;
        rudder.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}