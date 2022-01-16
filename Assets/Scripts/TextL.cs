using System.Collections;
using UnityEngine;

//-----------------------------------------------------------------------------
// name: TextL.cs
// desc: falling scene left wall; quit application at end of animation
//-----------------------------------------------------------------------------

public class TextL : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        GetComponent<ChuckSubInstance>().RunFile("fallingL.ck", true);
    }

    void Update()
    {
        // when falling animation is ending
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f)
        {
            // fade out audio
            iTween.AudioTo(gameObject, iTween.Hash("volume", 0.0f,
                                                   "time", 0.7f,
                                                   "easeType", iTween.EaseType.easeInOutSine));
            StartCoroutine(Quit());
        }
    }

    // wait 0.7 seconds and then quit
    IEnumerator Quit()
    {
        yield return new WaitForSeconds(0.7f);
        print("quitting");
        Application.Quit();
    }
}
