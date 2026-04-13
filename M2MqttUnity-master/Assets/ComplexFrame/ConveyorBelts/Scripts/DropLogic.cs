using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConveyorBelt
{
    public class DropLogic : MonoBehaviour
    {
        [Range(0, 10)] public int Height;
        float Timer = 0.25f;
        public bool FallingObject;
        public GameObject GlassWall, Walls, FallObject;
        ConveyorBeltLogic ConveyorBelt;
        // Start is called before the first frame update
        void Start()
        {
            GlassWall.transform.localScale = new Vector3(1, Height, 1);
            ConveyorBelt = GetComponent<ConveyorBeltLogic>();
            Walls.SetActive(false);
        }
        private void Update()
        {
            if (ConveyorBelt.ConveyorBeltList.Count > 0) { Walls.SetActive(false); }

            if (FallingObject)
            {
                if (Timer <= 0)
                {
                    Walls.SetActive(true);
                    Timer = 0.25f;
                    FallObject.transform.position = new Vector3(transform.position.x, FallObject.transform.position.y, transform.position.z);
                    FallObject.GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, 0);
                    FallObject.transform.eulerAngles = new Vector3(0, 0, 0);
                    FallingObject = false;
                }
                else
                {
                    Timer -= Time.deltaTime;
                }
            }
        }
    }
}
