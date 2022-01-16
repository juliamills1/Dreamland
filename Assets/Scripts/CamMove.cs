using UnityEngine;

//-----------------------------------------------------------------------------
// name: CamMove.cs
// desc: canyon scene camera/player control; change position and rotation with
//       arrow keys; update Chuck audio accordingly
//-----------------------------------------------------------------------------

public class CamMove : MonoBehaviour
{
    // current movement (stationary/walk/run)
    private int mode = 0;
    private int previousMode = 0;

    float moveSpeed;
    float rotationSpeed = 20;

    bool clusterOn = false;
    private ChuckIntSyncer miniClusterOn;

    // start Chuck audio
    void Start()
    {
        GetComponent<ChuckSubInstance>().RunFile("footsteps.ck", true);
        miniClusterOn = gameObject.AddComponent<ChuckIntSyncer>();
        miniClusterOn.SyncInt(GetComponent<ChuckSubInstance>(), "miniOn");
    }

    void Update()
    {
        // update mode and speed according to current keys pressed
        if (Running())
        {
            moveSpeed = 24;
            mode = 2;
        }
        else if (Walking())
        {
            moveSpeed = 12;
            mode = 1;
        }
        else
        {
            mode = 0;
        }

        // use left/right arrows to rotate camera from side to side
        Vector3 rotation = Vector3.zero;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotation = new Vector3(0,-1,0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rotation = new Vector3(0,1,0);
        }

        // trigger miniCluster sound during rotation
        if (rotation != Vector3.zero && miniClusterOn.GetCurrentValue() == 0)
        {
            GetComponent<ChuckSubInstance>().BroadcastEvent("miniClusterTrigger");
        }

        // apply new rotation over time
        transform.Rotate(rotation * Time.deltaTime * rotationSpeed, Space.World);

        // up arrow = forward, down arrow = backwards; adjust for rotation
        float y = transform.rotation.eulerAngles.y;
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            // if facing back wall
            if (y < 0 || y > 180)
            {
                direction = new Vector3(-1,0,0);
            }
            else
            {
                direction = new Vector3(1,0,0);
            }
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (y < 0 || y > 180)
            {
                direction = new Vector3(1,0,0);
            }
            else
            {
                direction = new Vector3(-1,0,0);
            }
        }

        // apply new position over time
        transform.Translate(direction * Time.deltaTime * moveSpeed, Space.World);

        // prevent player from leaving canyon at start
        float x = Mathf.Max(transform.position.x, 18);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);

        // if mode has changed
        if (mode != previousMode)
        {
            UpdateChuck();
            previousMode = mode;
        }

        // trigger cluster sound partway through canyon
        if (transform.position.x >= 120)
        {
            if (!clusterOn)
            {
                GetComponent<ChuckSubInstance>().BroadcastEvent("clusterTrigger");
                clusterOn = true;
            }
        }
    }

    // use left shift + up/down arrow keys to run
    bool Running()
    {
        return Input.GetKey(KeyCode.LeftShift) && Walking();
    }

    // use up/down arrow keys to move forward/backwards
    bool Walking()
    {
        return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow);
    }

    // send change in movement to Chuck
    void UpdateChuck()
    {
        GetComponent<ChuckSubInstance>().SetInt("mode", mode);
        GetComponent<ChuckSubInstance>().BroadcastEvent("changeHappened");
    }
}