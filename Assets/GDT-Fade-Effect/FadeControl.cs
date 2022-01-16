using UnityEngine;

public class FadeControl : MonoBehaviour
{
    public GameObject fadeEffect;
    public GameObject wall;

    // Update is called once per frame
    void Update()
    {
        if (wall.transform.position.x >= 950)
        {
            fadeEffect.SetActive(true);
        }
    }
}
