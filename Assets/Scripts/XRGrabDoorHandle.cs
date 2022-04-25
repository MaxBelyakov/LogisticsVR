using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabDoorHandle : XRGrabInteractable
{
    public Transform handler;
    private Vector3 grabScale;

    private IXRSelectInteractor interactor;
    private IXRSelectInteractable interactable;

    void Start()
    {
        grabScale = transform.localScale;
    }

    private void Update()
    {
        if (Vector3.Distance(handler.position, transform.position) > 0.4f)
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
        transform.position = handler.transform.position;
        transform.rotation = handler.transform.rotation;
        transform.localScale = grabScale;

        handler.GetComponent<Rigidbody>().velocity = Vector3.zero;
        handler.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
