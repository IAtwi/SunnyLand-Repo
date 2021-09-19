using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]float offset = -10f;

    private Transform player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position + new Vector3(0,0,offset) ;
        }
    }
}
