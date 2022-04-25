using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabRudder : XRGrabInteractable
{
    public Transform rudder;

    private IXRSelectInteractor interactor;
    private IXRSelectInteractable interactable;

    private void Update()
    {
        if (Vector3.Distance(rudder.position, transform.position) > 0.4f)
            interactionManager.SelectExit(interactor, interactable);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject;
        interactable = args.interactableObject;

        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        rudder.GetComponent<Rigidbody>().velocity = Vector3.zero;
        rudder.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
