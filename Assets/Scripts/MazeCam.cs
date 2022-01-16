using UnityEngine;

//-----------------------------------------------------------------------------
// name: MazeCam.cs
// desc: maze scene camera control; rotates into shot and then follows player
//-----------------------------------------------------------------------------

public class MazeCam : MonoBehaviour
{
    public GameObject player;
    public GameObject fadeEffect;
    private Vector3 offset;

    void Start()
    {
        iTween.RotateTo(gameObject, iTween.Hash("rotation", Vector3.zero,
                                                "time", 4.4f,
                                                "easetype", iTween.EaseType.easeOutCirc));

        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}