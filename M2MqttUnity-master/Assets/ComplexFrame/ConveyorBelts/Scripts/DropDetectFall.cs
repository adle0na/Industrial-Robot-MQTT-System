using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConveyorBelt
{
    public class DropDetectFall : MonoBehaviour
    {
        public DropLogic drop;
        private void OnTriggerEnter(Collider other)
        {
            drop.FallingObject = true;
            drop.FallObject = other.gameObject;
        }
    }
}
