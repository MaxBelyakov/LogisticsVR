// Needs to parent pallet to forklift to move it by forklift

using UnityEngine;

public class PalletParenter : MonoBehaviour
{
    public Transform palletScanner;
    private float palletSize = 1.5f;

    private bool forkliftCollide;

    void Update()
    {
        forkliftCollide = false;

        RaycastHit[] items = Physics.BoxCastAll(palletScanner.position, palletScanner.lossyScale, palletScanner.forward, palletScanner.rotation, palletSize);

        foreach (var item in items)
        {
            // layer "Forklift lift"
            if (item.transform.gameObject.layer == 13)
            {
                transform.parent = GameObject.FindGameObjectWithTag("forklift").transform;
                forkliftCollide = true;
            }
        }

        if (!forkliftCollide)
            transform.parent = null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(palletScanner.position, palletScanner.forward * palletSize);
    }
}