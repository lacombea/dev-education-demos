using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    float offsetY;

    void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        offsetY = transform.position.y - player.position.y;
    }

    void LateUpdate()
    {
        if (player == null) return;
        if (player.position.y + offsetY > transform.position.y)
        {
            transform.position = new Vector3(transform.position.x, player.position.y + offsetY, transform.position.z);
        }
    }
}