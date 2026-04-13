using UnityEngine;

public class MQTT_PlayerController : MonoBehaviour
{
    public void Move(string msg)
    {
        if (msg.Contains("\"status\": \"on\""))
        {
            transform.position += new Vector3(1, 0, 0); // 오른쪽
        }
        else if (msg.Contains("\"status\": \"off\""))
        {
            transform.position += new Vector3(-1, 0, 0); // 왼쪽
        }
    }
}