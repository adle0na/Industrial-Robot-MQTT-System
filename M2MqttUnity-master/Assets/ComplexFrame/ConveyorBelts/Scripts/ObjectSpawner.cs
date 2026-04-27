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

        public bool power = false;
        
        void Update()
        {
            if (!power) return;
            
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
