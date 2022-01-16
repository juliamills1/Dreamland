using UnityEngine;

//-----------------------------------------------------------------------------
// name: BasicFollower.cs
// desc: mirror player position
//-----------------------------------------------------------------------------


public class BasicFollower : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;

    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}