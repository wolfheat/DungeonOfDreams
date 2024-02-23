using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] GameObject player;

    private void Update()
    {
        transform.SetPositionAndRotation(player.transform.position, player.transform.rotation);
    }
}
