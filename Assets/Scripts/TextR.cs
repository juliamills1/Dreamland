using UnityEngine;

//-----------------------------------------------------------------------------
// name: TextR.cs
// desc: falling scene right wall
//-----------------------------------------------------------------------------

public class TextR : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        GetComponent<ChuckSubInstance>().RunFile("fallingR.ck", true);
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
        }
    }
}
