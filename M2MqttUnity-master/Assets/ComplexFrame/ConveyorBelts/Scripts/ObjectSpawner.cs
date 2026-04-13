using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConveyorBelt
{
    public class ObjectSpawner : MonoBehaviour
    {
        public GameObject Object;
        public Transform SpawnPoint; 

        [Range(0, 10)] public int SpawnRate;
        float Timer = 0.5f;
        public SpawnerCheck spawnCheck;

        void Update()
        {
            if (!spawnCheck.Full)
            {
                if (Timer <= 0)
                {
                    Instantiate(Object, SpawnPoint.position, Quaternion.identity);
                    Timer = SpawnRate;
                }
                else
                {
                    Timer -= Time.deltaTime;
                }
            }
        }
    }
}
