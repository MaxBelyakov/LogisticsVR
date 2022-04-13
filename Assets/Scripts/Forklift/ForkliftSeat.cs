// Teleportation to forklift seat

using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class ForkliftSeat : TeleportationArea
{
    public Transform seat;      // Forklift seat
    public Transform playerCamera;
    public Transform player;

    // Standart teleport request
    protected override bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
    {
        if (raycastHit.collider == null)
            return false;

        // Set destination position and rotation
        teleportRequest.destinationPosition = seat.position;
        //teleportRequest.destinationRotation = seat.rotation;
        //float angle = Vector3.Angle(player.transform.forward, playerCamera.forward);
        //playerCamera.RotateAround(new Vector3(player.transform.position.x, 0, player.transform.position.z), Vector3.up, angle);
        return true;
    }
}