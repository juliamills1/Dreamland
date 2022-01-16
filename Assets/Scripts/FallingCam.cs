using System.Collections;
using UnityEngine;

//-----------------------------------------------------------------------------
// name: FallingCam.cs
// desc: falling scene camera control; change background color over time
//-----------------------------------------------------------------------------

public class FallingCam : MonoBehaviour
{
    [SerializeField] Gradient gradient;
    [SerializeField] float duration;
    float t = 0f;
    public GameObject endFade;

    void Start()
    {
        StartCoroutine(End());
    }

    // increment through gradient color values over duration
    void Update()
    {
        float value = Mathf.Lerp(0f, 1f, t);
        t += Time.deltaTime / duration;
        Camera.main.backgroundColor = gradient.Evaluate(value);
    }

    // fade out at 29 seconds
    IEnumerator End()
    {
        yield return new WaitForSeconds(29);
        endFade.SetActive(true);
    }
}