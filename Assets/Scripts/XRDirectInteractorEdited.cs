// Edited version of XRDirectInteractor.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRDirectInteractorEdited : XRDirectInteractor
{
    public SphereCollider handCollider;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Deactivate hand collider to avoid conflict with weapon colliders
        handCollider.enabled = false;

        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        // Return hand collider
        handCollider.enabled = true;

        base.OnSelectExited(args);
    }
}
