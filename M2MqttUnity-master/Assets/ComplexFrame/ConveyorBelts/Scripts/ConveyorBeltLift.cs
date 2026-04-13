using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConveyorBelt
{
    public class ConveyorBeltLift : MonoBehaviour
    {
        [Range(0, 10)] public int Height;
        [Range(0f, 10.0f)] public float speed;
        float Timer = 2.5f, StartHeight, MaxHeight;
        public bool Lifting;
        public ConveyorBeltLogic Belt;
        public List<GameObject> ConveyorBeltList;
        public Transform DetectObject;
        float PreSpeed;
        private void Start()
        {
            StartHeight = transform.position.y;
            MaxHeight = transform.position.y + Height * 10;
            PreSpeed = Belt.speed;
        }

        private void Update()
        {
            if (!Lifting)
            {
                //Get the Conveyor Belt material in order to change it's offset
                GetComponent<MeshRenderer>().material.mainTextureOffset += new Vector2(0, 1) * speed * Time.deltaTime;

                float velocityObject;
                //Add force to the objects on the Conveyor Belt
                for (int i = 0; i <= ConveyorBeltList.Count - 1; i++)
                {
                    velocityObject = Calculation
                        (
                        ConveyorBeltList[i].GetComponent<Rigidbody>().linearVelocity.x,
                        ConveyorBeltList[i].GetComponent<Rigidbody>().linearVelocity.y,
                        ConveyorBeltList[i].GetComponent<Rigidbody>().linearVelocity.z
                        );

                    if (velocityObject < speed * 5)
                    {
                        ConveyorBeltList[i].GetComponent<Rigidbody>().linearVelocity += -transform.forward * speed;
                    }
                }
            }
            else
            {
                if (transform.position.y < MaxHeight)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + 10 * Time.deltaTime, transform.position.z);
                }
                else
                {
                    Lifting = false;
                }
            }

            if (ConveyorBeltList.Count > 0)
            {
                if (!Lifting && transform.position.y < MaxHeight)
                {
                    if (Vector3.Distance(DetectObject.position, ConveyorBeltList[0].transform.position) < 2)
                    {
                        Lifting = true;
                    }
                }
            }
            else
            {
                if (transform.position.y > StartHeight)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y - 10 * Time.deltaTime, transform.position.z);
                }
                else
                {
                    Belt.speed = PreSpeed;
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            //Add the object that just collided to the Conveyor Belt 
            if (collision.gameObject.GetComponent<Rigidbody>() != null)
            {
                ConveyorBeltList.Add(collision.gameObject);
                Belt.speed = 0;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            //Remove the object from the Conveyor Belt list
            ConveyorBeltList.Remove(collision.gameObject);
        }

        private float Calculation(float x, float y, float z)
        {
            return Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
        }
    }
}