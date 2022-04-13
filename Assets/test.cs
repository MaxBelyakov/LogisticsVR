using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class test : XRGrabInteractable
{
    public GameObject rud;

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        transform.rotation = rud.transform.rotation;
        base.ProcessInteractable(updatePhase);
    }
}
