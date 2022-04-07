// Teleportation to forklift seat

using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class ForkliftSeat : TeleportationArea
{
    public Transform seat;      // Forklift seat

    // Standart teleport request
    protected override bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
    {
        if (raycastHit.collider == null)
            return false;

        // Set destination position and rotation
        teleportRequest.destinationPosition = seat.position;
        teleportRequest.destinationRotation.eulerAngles = seat.eulerAngles;
        return true;
    }
}