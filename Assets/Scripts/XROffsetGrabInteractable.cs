// Grab interactable script modifiaction. Use for grab items without rotating

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XROffsetGrabInteractable : XRGrabInteractable
{
    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        // Boxes are not grabbable if kinematic (on pallet)
        if (GetComponent<Rigidbody>() != null)
            if (GetComponent<Rigidbody>().isKinematic)
                return false;

        return base.IsSelectableBy(interactor);
    }

    void Start()
    {
        // Create new attach pivot
        if (!attachTransform)
        {
            GameObject grab = new GameObject("Grab Pivot");
            grab.transform.SetParent(transform, false);
            attachTransform = grab.transform;
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Set attach pivot in position of interactable object without rotation
        attachTransform.position = args.interactableObject.transform.position;
        attachTransform.rotation = args.interactorObject.transform.rotation;

        base.OnSelectEntered(args);
    }
}