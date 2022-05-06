// Grab interactable script modifiaction. Use for grab items without rotating

using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class XROffsetGrabInteractable : XRGrabInteractable
{
    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        // Boxes are not grabbable if kinematic (on pallet)
        if (transform.tag == "box" && GetComponent<Rigidbody>().isKinematic)
            return false;
        
        // Prizes are not grabbable if not enought money
        if (transform.tag == "prize" && GetComponent<Rigidbody>().isKinematic)
        {
            // Get price
            int price = transform.GetComponent<PrizeManager>().price;

            if (price > FindObjectOfType<InfoDesk>().money)
            {
                StartCoroutine(NotEnoughtMoney(transform.GetComponent<PrizeManager>().textCanvas.transform.GetChild(0)));
                return false;
            }
        }

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

        // Get prize
        if (args.interactableObject.transform.tag == "prize")
        {
            // Hide hands when get prize
            args.interactorObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

            // Buy prize
            if (!args.interactableObject.transform.GetComponent<PrizeManager>().sold)
            {
                // Get price
                int price = args.interactableObject.transform.GetComponent<PrizeManager>().price;

                // Decrease amount of money on info desk
                FindObjectOfType<InfoDesk>().money -= price;

                // Hide price text
                args.interactableObject.transform.GetComponent<PrizeManager>().textCanvas.enabled = false;

                // Change flag
                args.interactableObject.transform.GetComponent<PrizeManager>().sold = true;
            }
        }

        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        // Show hands
        if (!args.interactorObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled)
            args.interactorObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        
        base.OnSelectExited(args);

        // Disable kinematic
        if (args.interactableObject.transform.tag == "prize" && args.interactableObject.transform.GetComponent<Rigidbody>().isKinematic)
            args.interactableObject.transform.GetComponent<Rigidbody>().isKinematic = false;
    }

    IEnumerator NotEnoughtMoney(Transform text)
    {
        text.GetComponent<TMP_Text>().color = Color.red;
        yield return new WaitForSeconds(2f);
        text.GetComponent<TMP_Text>().color = Color.white;
    }
}