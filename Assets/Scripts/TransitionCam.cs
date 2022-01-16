using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//-----------------------------------------------------------------------------
// name: TransitionCam.cs
// desc: doors scene player control; activate quad at timer ring; move with
//       arrow keys after timer rings; update Chuck audio accordingly
//-----------------------------------------------------------------------------

public class TransitionCam : MonoBehaviour
{
    public GameObject quad;
    private float moveSpeed;

    // 1 = timer audio has gone off
    private ChuckIntSyncer ding;

    // current movement (stationary/walk)
    private int mode = 0;
    private int previousMode = 0;

    // playback rate to send to Chuck
    private float rate = 0;
    private float previousRate = 0;

    void Start()
    {
        ding = gameObject.AddComponent<ChuckIntSyncer>();
        ding.SyncInt(GetComponent<ChuckSubInstance>(), "triggered");
        StartCoroutine(Waiting());
    }

    // wait 1 second before playing Chuck audio & rotating camera
    IEnumerator Waiting()
    {
        yield return new WaitForSeconds(1);
        GetComponent<ChuckSubInstance>().RunFile("transition.ck", true);
        iTween.RotateBy(gameObject, iTween.Hash("z", 1.0f,
                                                "time", 9.9f,
                                                "easetype", iTween.EaseType.linear));
    }

    void Update()
    {
        // change scene once reach doorway
        if (transform.position.x >= 112)
        {
            SceneManager.LoadScene(2);
        }

        // once timer audio goes off
        if (ding.GetCurrentValue() == 1)
        {
            quad.SetActive(true);

            // use up arrow to move forward
            Vector3 direction = Vector3.zero;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                mode = 1;
                direction = new Vector3(1,0,0);
            }
            else
            {
                mode = 0;
            }

            // scale player movement & playback speed to progress (slows down)
            float invLerp = Mathf.InverseLerp(18, 112, transform.position.x);
            moveSpeed = Mathf.Lerp(8, 1.5f, invLerp);
            transform.Translate(direction * Time.deltaTime * moveSpeed, Space.World);
            rate = Mathf.Lerp(1.0f, 0.3f, invLerp);

            // if mode has changed
            if (mode != previousMode)
            {
                GetComponent<ChuckSubInstance>().SetInt("walking", mode);
                GetComponent<ChuckSubInstance>().BroadcastEvent("changeHappened");
                previousMode = mode;
            }

            // if rate has changed
            if (rate != previousRate)
            {
                GetComponent<ChuckSubInstance>().SetFloat("rate", rate);
                GetComponent<ChuckSubInstance>().BroadcastEvent("rateChange");
                previousRate = rate;
            }
        }
    }
}
