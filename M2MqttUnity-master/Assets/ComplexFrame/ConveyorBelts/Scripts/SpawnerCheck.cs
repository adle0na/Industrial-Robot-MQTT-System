using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConveyorBelt
{
    public class SpawnerCheck : MonoBehaviour
    {
        public bool Full;

        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<Rigidbody>() != null)
            {
                Full = true;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<Rigidbody>() != null)
            {
                Full = false;
            }
        }
    }
}
