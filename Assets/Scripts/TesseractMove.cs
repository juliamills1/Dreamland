using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//-----------------------------------------------------------------------------
// name: TesseractMove.cs
// desc: maze player control; move with arrow keys and update Chuck audio
//       accordingly; dim and shrink tesseract with progress; create list for
//       trail dot positions; flash emission at checkpoints; change scenes when
//       all checkpoints triggered
//-----------------------------------------------------------------------------

public class TesseractMove : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject cam;
    public GameObject trails;
    public GameObject checkParent;
    Material tesseractMat;

    // list of trail dot positions
    public List<Vector3> history = new List<Vector3>();

    // total number of checkpoints
    int numCheckPts;
    HashSet<String> checkpoints = new HashSet<String>();

    // direction of movement
    private int mode = 0;
    private int previousMode = 0;

    // baseline tesseract color values
    private float intensity = 2;
    private float colorFac;
    float startRGB = 0.7490196f;
    float endRGB = 0.15f;

    private Vector3 startPos;
    int runs = 0;
    bool endGame = false;
    int firstRunCheckPts = 0;

    void Start()
    {
        startPos = transform.position;
        numCheckPts = checkParent.transform.childCount;

        GameObject tesseract = transform.GetChild(0).gameObject;
        tesseractMat = tesseract.GetComponent<Renderer>().material;
        colorFac = startRGB;

        // start audio in Chuck
        GetComponent<ChuckSubInstance>().RunFile("maze.ck", true);
        UpdateChuckLayers();
    }

    IEnumerator Waiting()
    {
        yield return new WaitForSeconds(4.4f);
        rb.velocity = 4 * GetInputDirection();
    }

    void Update()
    {
        // wait until camera is done rotating into shot
        StartCoroutine(Waiting());

        // stationary
        if (rb.velocity == Vector3.zero)
        {
            mode = 0;
        }

        // if mode has changed
        if (mode != previousMode)
        {
            UpdateChuckMode();
            previousMode = mode;
        }

        // snap position to integer grid
        Vector3 roundedPos = Vector3Int.RoundToInt(transform.position);

        // add to list if no neighbor vectors are in it
        if (!HasNeighbors(history, roundedPos))
        {
            history.Add(roundedPos);
        }

        // at maze exit
        if (transform.position.x > 47 && transform.position.z < -12)
        {
            // if game is over
            if (checkpoints.Count == numCheckPts)
            {
                // trigger ending only once
                if (!endGame)
                {
                    GameEnd();
                    endGame = true;
                }
            }
            else
            {
                runs++;
                UpdateChuckLayers();
                transform.position = startPos;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!endGame)
        {
            FlashEmission();
            GetComponent<ChuckSubInstance>().BroadcastEvent("trigger");
        }

        // tally checkpoints hit during 1st run
        if (runs == 0)
        {
            firstRunCheckPts = checkpoints.Count;
        }

        // if checkpoint not triggered before
        if (!checkpoints.Contains(other.name))
        {
            checkpoints.Add(other.name);
            //print(checkpoints.Count + " of " + numCheckPts);

            // dim emission & shrink tesseract after 1st run
            if (runs > 0)
            {
                DimEmission();
                transform.localScale -= new Vector3(0.028f, 0, 0.028f);
                GetComponent<ChuckSubInstance>().BroadcastEvent("newTrigger");
            }
        }
    }

    // return vector corresponding to arrow keys movement
    Vector3 GetInputDirection()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction = new Vector3(1, 0, 0);
            mode = 1;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            direction = new Vector3(0, 0, 1);
            mode = 2;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction = new Vector3(-1, 0, 0);
            mode = 3;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            direction = new Vector3(0, 0, -1);
            mode = 4;
        }
        return direction;
    }

    // check if any neighboring vectors appear in list, including self
    bool HasNeighbors(List<Vector3> h, Vector3 a)
    {
        bool hasNeighbors = false;
        HashSet<Vector3> neighbors = NearNeighbors(a);

        foreach(Vector3 n in neighbors)
        {
            if (h.Contains(n))
            {
                hasNeighbors = true;
            }
        }
        return hasNeighbors;
    }

    // return all positions within 2 units on xz plane
    HashSet<Vector3> NearNeighbors(Vector3 a)
    {
        HashSet<Vector3> neighbors = new HashSet<Vector3>();
        neighbors.Add(a);

        for (int i = 1; i <= 2; i++)
        {
            neighbors.Add(new Vector3(a.x + i, a.y, a.z));
            neighbors.Add(new Vector3(a.x - i, a.y, a.z));
            neighbors.Add(new Vector3(a.x, a.y, a.z + i));
            neighbors.Add(new Vector3(a.x, a.y, a.z - i));
            neighbors.Add(new Vector3(a.x + i, a.y, a.z + i));
            neighbors.Add(new Vector3(a.x - i, a.y, a.z - i));
            neighbors.Add(new Vector3(a.x + i, a.y, a.z - i));
            neighbors.Add(new Vector3(a.x - i, a.y, a.z + i));
        }
        return neighbors;
    }

    // send current movement mode to Chuck
    void UpdateChuckMode()
    {
        GetComponent<ChuckSubInstance>().SetInt("mode", mode);
        GetComponent<ChuckSubInstance>().BroadcastEvent("changeHappened");
    }

    // send number of runs to Chuck
    void UpdateChuckLayers()
    {
        GetComponent<ChuckSubInstance>().SetInt("runs", runs);
        GetComponent<ChuckSubInstance>().BroadcastEvent("layerChange");
    }

    // lower tesseract emission with progress
    void DimEmission()
    {
        // percent progress between end of 1st run and maze completion
        float invLerp = Mathf.InverseLerp(firstRunCheckPts,
                                          numCheckPts,
                                          checkpoints.Count);
        float colorLerp = Mathf.Lerp(startRGB, endRGB, invLerp);
        float intensityLerp = Mathf.Lerp(2, 0.5f, invLerp);
        float intensityFac = Mathf.Pow(2, intensityLerp);

        Color c = intensityFac * new Color(colorLerp, colorLerp, colorLerp);
        tesseractMat.SetColor("_EmissionColor", c);

        // update current baseline color values
        intensity = intensityLerp;
        colorFac = colorLerp;
    }

    // quickly blink tesseract emission
    void FlashEmission()
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", Mathf.Max(intensity + 2, 3),
                                               "to", intensity,
                                               "time", 0.5f,
                                               "easeType", iTween.EaseType.easeInOutSine,
                                               "onupdate", "IncrementEmission"));
    }

    // for iTween on-update function: change tesseract emission
    void IncrementEmission(float em)
    {
        float intensityFac = Mathf.Pow(2, em);
        Color c = intensityFac * new Color(colorFac, colorFac, colorFac);
        tesseractMat.SetColor("_EmissionColor", c);
    }

    // ending of maze scene
    void GameEnd()
    {
        // move trail dots towards tesseract
        for (int i = 0; i < trails.transform.childCount; i++)
        {
            GameObject trailDot = trails.transform.GetChild(i).gameObject;
            iTween.MoveTo(trailDot, iTween.Hash("position", transform.position,
                                                "time", 3,
                                                "easetype", iTween.EaseType.easeOutCubic));
        }

        // increase tesseract brightness
        iTween.ValueTo(gameObject, iTween.Hash("from", 1,
                                               "to", 8,
                                               "time", 3,
                                               "easeType", iTween.EaseType.easeInCubic,
                                               "onupdate", "IncrementEmission"));

        // increase tesseract size
        iTween.ScaleTo(gameObject, iTween.Hash("scale", new Vector3(6, 6, 6),
                                               "delay", 0.2f,
                                               "time", 4,
                                               "easetype", iTween.EaseType.easeInCubic));

        StartCoroutine(WaitToChange());
    }

    // change scenes & fade out audio once tesseract expansion done
    IEnumerator WaitToChange()
    {
        yield return new WaitForSeconds(3);
        iTween.AudioTo(gameObject, iTween.Hash("volume", 0.0f,
                                               "time", 1,
                                               "easeType", iTween.EaseType.easeInOutSine));

        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(3);
    }
}