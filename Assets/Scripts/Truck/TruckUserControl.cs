using System;
using UnityEngine;

namespace Trucks
{
    [RequireComponent(typeof (TruckController))]
    public class TruckUserControl : MonoBehaviour
    {
        private TruckController m_Car; // the car controller we want to use


        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<TruckController>();
        }


        private void FixedUpdate()
        {
            m_Car.Move(-1f, 1f, 1f, 0f);
        }
    }
}