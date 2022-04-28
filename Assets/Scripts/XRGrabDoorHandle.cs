// XR Door Handle interactable controller

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabDoorHandle : XRGrabInteractable
{
    public Transform handler;       // Handler
    private Vector3 grabScale;      // Scale of grabbed object

    private IXRSelectInteractor interactor;         // Hand interactor object
    private IXRSelectInteractable interactable;     // Handler virtual interactable object

    void Start()
    {
        // Save local scale of handle
        grabScale = transform.localScale;
    }

    private void Update()
    {
        // Drop the handle if there is a distance between hand and handle
        if (Vector3.Distance(handler.position, transform.position) > 0.4f)
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

        // Return handler virtual interactable to handler position, rotation and scale
        transform.position = handler.transform.position;
        transform.rotation = handler.transform.rotation;
        transform.localScale = grabScale;

        // Stop handler rotataing when dropped
        //handler.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //handler.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}