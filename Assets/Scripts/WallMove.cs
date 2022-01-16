using UnityEngine;
using UnityEngine.SceneManagement;

//-----------------------------------------------------------------------------
// name: WallMove.cs
// desc: canyon scene cliff face; mirror player position; change scene at end
//-----------------------------------------------------------------------------

public class WallMove : MonoBehaviour
{
    public GameObject player;
    public GameObject fadeEffect;
    private Vector3 offset;

    void Start()
    {
        GetComponent<ChuckSubInstance>().RunFile("canyon.ck", true);
        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;

        // at canyon end
        if (transform.position.x >= 930)
        {
            // trigger fade & change scenes
            fadeEffect.SetActive(true);
            if (fadeEffect.GetComponent<GDTFadeEffect>().HasFinished())
            {
                SceneManager.LoadScene(1);
            }

            // fade out audio
            iTween.AudioTo(gameObject, iTween.Hash("volume", 0.0f,
                                                   "time", 1,
                                                   "easeType", iTween.EaseType.easeInOutSine));
        }
    }
}