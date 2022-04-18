// Needs to parent pallet to forklift to move it by forklift

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxParenter : MonoBehaviour
{
    public Transform boxScanner;
    private float boxSize;

    void Update()
    {
        RaycastHit[] items = Physics.BoxCastAll(boxScanner.position, boxScanner.lossyScale, boxScanner.forward, boxScanner.rotation, boxSize);

        foreach (var item in items)
        {
            // layer "Forklift lift"
            if (item.transform.gameObject.layer == 13)
                transform.parent = GameObject.FindGameObjectWithTag("forklift").transform;

            // layer "Box"
            if (item.transform.gameObject == this)
                item.transform.parent.parent = transform;
        }
    }
}
