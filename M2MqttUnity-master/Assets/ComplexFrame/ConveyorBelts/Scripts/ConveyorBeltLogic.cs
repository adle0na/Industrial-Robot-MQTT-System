using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConveyorBelt
{
    public class ConveyorBeltLogic : MonoBehaviour
    {
        //Set the speed of the Conveyor Belt
        [Range(0f, 10.0f)] public float speed;
        public List<GameObject> ConveyorBeltList;
        Vector3 BlockPosition;


        private void Update()
        {
            //Get the Conveyor Belt material in order to change it's offset
            GetComponent<MeshRenderer>().material.mainTextureOffset += new Vector2(0, 1) * speed * Time.deltaTime / 5;
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
                    ConveyorBeltList[i].transform.position = BlockPosition;
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<Rigidbody>() != null)
            {
                //Adjust position when colliding with another conveyor belt
                if(collision.transform.position.x - transform.position.x > 0) { collision.transform.position = new Vector3(collision.transform.position.x-0.1f, collision.transform.position.y, collision.transform.position.z); } 
                else{ collision.transform.position = new Vector3(collision.transform.position.x + 0.1f, collision.transform.position.y, collision.transform.position.z); }

                if (collision.transform.position.x - transform.position.z > 0) { collision.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y, collision.transform.position.z - 0.1f); }
                else { collision.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y, collision.transform.position.z + 0.1f); }

                //collision.transform.position = BlockPosition;
                ConveyorBeltList.Add(collision.gameObject);
                BlockPosition = collision.gameObject.transform.position;
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
