using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//-----------------------------------------------------------------------------
// name: Trails.cs
// desc: create trail dot via prefab at positions specified by TesseractMove.cs;
//       flicker dot emission
//-----------------------------------------------------------------------------

public class Trails : MonoBehaviour
{
    public GameObject prefab;
    public GameObject tessParent;
    List<GameObject> trails = new List<GameObject>();

    // list of positions for trail dots
    private List<Vector3> hist;
    private int oldCount;

    // for emission flickering
    public float minIntensity = 4;
    public float maxIntensity = 9;
    int smoothing = 20;
    Queue<float> smoothQueue;
    float lastSum = 0;

    void Start()
    {
        hist = tessParent.GetComponent<TesseractMove>().history;
        oldCount = hist.Count;
        smoothQueue = new Queue<float>(smoothing);
    }

    void Update()
    {
        // for each trail dot
        for (int i = 0; i < trails.Count; i++)
        {
            // slightly vary trail dot emission
            float intensity = Mathf.Pow(2, Flicker(smoothQueue));
            Color c = intensity * new Color(1, 2, 4.5f);
            Material mat = trails[i].gameObject.GetComponent<Renderer>().material;
            mat.SetColor("_EmissionColor", c);
        }
    }

    void LateUpdate()
    {
        // if hist has been updated
        if (hist.Count != oldCount)
        {
            // for each new position
            foreach(Vector3 h in hist.Skip(oldCount))
            {
                // create new trail dot object & add to list
                GameObject go = Instantiate(prefab, h, Quaternion.identity);
                go.transform.parent = this.transform;
                trails.Add(go);
            }

            oldCount = hist.Count;
        }
    }

    float Flicker(Queue<float> q)
    {
        while (q.Count >= smoothing)
        {
            lastSum -= q.Dequeue();
        }

        // generate random new item, calculate new average
        float newVal = Random.Range(minIntensity, maxIntensity);
        q.Enqueue(newVal);
        lastSum += newVal;

        // calculate new smoothed average
        float intensity = lastSum / (float) q.Count;
        return intensity;
    }
}