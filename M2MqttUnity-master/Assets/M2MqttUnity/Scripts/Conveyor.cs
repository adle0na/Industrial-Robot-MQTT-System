using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public float speed = 2f;
    private bool isRunning = false;

    void Update()
    {
        if (isRunning)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    public void StartConveyor()
    {
        isRunning = true;
    }

    public void StopConveyor()
    {
        isRunning = false;
    }
}