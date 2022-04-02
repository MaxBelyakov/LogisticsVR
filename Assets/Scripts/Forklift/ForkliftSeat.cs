namespace UnityEngine.XR.Interaction.Toolkit
{
    public class ForkliftSeat : TeleportationArea
    {
        public Transform seat;

        protected override bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
        {
            if (raycastHit.collider == null)
                return false;

            teleportRequest.destinationPosition = seat.position;
            teleportRequest.destinationRotation = seat.rotation;
            return true;
        }
    }
}