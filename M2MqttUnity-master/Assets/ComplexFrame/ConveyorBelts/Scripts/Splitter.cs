using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConveyorBelt
{
    public class Splitter : MonoBehaviour
    {
        //Set the speed of the Conveyor Belt
        [Range(0f, 10.0f)] public float speed;
        public bool Left, Front, Right;
        bool IsLeft, IsFront, IsRight;
        public List<GameObject> ConveyorBeltList;
        public Transform DetectObject;
        public ConveyorBeltLogic QueueBelt;
        public bool Switch = false, Change;
        float QueueBeltStartSpeed;

        public void Start()
        {
            if (Left) { transform.localRotation = Quaternion.Euler(0, -90, 0); IsLeft = true; }
            else if (Front) { transform.localRotation = Quaternion.Euler(0, 0, 0); IsFront = true; }
            else if (Right) { transform.localRotation = Quaternion.Euler(0, 0 + 90, 0); IsRight = true; }
            QueueBeltStartSpeed = QueueBelt.speed;
        }

        private void Update()
        {
            //Get the Conveyor Belt material in order to change it's offset
            GetComponent<MeshRenderer>().material.mainTextureOffset += new Vector2(0, 1) * speed * Time.deltaTime;

            if (Switch)
            {
                QueueBelt.speed = 0;
                if (Change)
                {
                    if (Vector3.Distance(DetectObject.position, ConveyorBeltList[0].transform.position) < 2)
                    {
                        if (IsLeft)
                        {
                            if (Front) { transform.localRotation = Quaternion.Euler(0, 0, 0); IsFront = true; IsLeft = false; }
                            else if (Right) { transform.localRotation = Quaternion.Euler(0, 90, 0); IsRight = true; IsLeft = false; }
                            else { transform.localRotation = Quaternion.Euler(0, -90, 0); IsLeft = true; IsFront = false; }
                            Change = false;
                            return;
                        }
                        if (IsFront)
                        {
                            if (Right) { transform.localRotation = Quaternion.Euler(0, 90, 0); IsRight = true; IsFront = false; }
                            else if (Left) { transform.localRotation = Quaternion.Euler(0, -90, 0); IsLeft = true; IsFront = false; }
                            else { transform.localRotation = Quaternion.Euler(0, 0, 0); IsFront = true; IsLeft = false; }
                            Change = false;
                            return;
                        }
                        if (IsRight)
                        {
                            if (Left) { transform.localRotation = Quaternion.Euler(0, -90, 0); IsLeft = true; IsRight = false; }
                            else if (Front) { transform.localRotation = Quaternion.Euler(0, 0, 0); IsFront = true; IsRight = false; }
                            else { transform.localRotation = Quaternion.Euler(0, 90, 0); IsRight = true; IsFront = false; }
                            Change = false;
                            return;
                        }
                    }
                }
            }
            else
            {
                QueueBelt.speed = QueueBeltStartSpeed;
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        private void FixedUpdate()
        {
            float velocityObject;
            //Add force to the objects on the Conveyor Belt
            for (int i = 0; i <= ConveyorBeltList.Count - 1; i++)
            {
                if (i == 0)
                {
                    velocityObject = Calculation
                        (
                        ConveyorBeltList[i].GetComponent<Rigidbody>().linearVelocity.x,
                        ConveyorBeltList[i].GetComponent<Rigidbody>().linearVelocity.y,
                        ConveyorBeltList[i].GetComponent<Rigidbody>().linearVelocity.z
                        );

                    if (velocityObject < speed * 5)
                    {
                        ConveyorBeltList[i].GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, 0);
                        ConveyorBeltList[i].GetComponent<Rigidbody>().linearVelocity += -transform.forward * speed * 5;
                    }
                }
                else
                {
                    ConveyorBeltList[i].GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, 0);
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            //Add the object that just collided to the Conveyor Belt 
            if (collision.gameObject.GetComponent<Rigidbody>() != null)
            {
                ConveyorBeltList.Add(collision.gameObject);
                Change = true;
                Switch = true;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            //Remove the object from the Conveyor Belt list
            ConveyorBeltList.Remove(collision.gameObject);
            Switch = false;
        }

        private float Calculation(float x, float y, float z)
        {
            return Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
        }
    }
}
