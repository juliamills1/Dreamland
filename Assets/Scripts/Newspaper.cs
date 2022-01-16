using UnityEngine;

//-----------------------------------------------------------------------------
// name: Newspaper.cs
// desc: maze walls; rotate through textures every 0.5s
//-----------------------------------------------------------------------------

public class Newspaper : MonoBehaviour
{
    Texture[] patterns;
    Material[] mats;
    int offset = 0;
    int count;

    void Start()
    {
        count = gameObject.transform.childCount;
        patterns = new Texture[count];
        mats = new Material[count];
        GameObject[] walls = new GameObject[count];

        // for each wall in maze
        for (int i = 0; i < count; i++)
        {
            // add newspaper clipping to texture array
            patterns[i] = Resources.Load<Texture>("Newspaper/" + (i + 1));

            // set texture to wall base map
            walls[i] = gameObject.transform.GetChild(i).gameObject;
            mats[i] = walls[i].GetComponent<Renderer>().material;
            mats[i].SetTexture("_BaseMap", patterns[i]);

            // change texture tiling according to wall width
            if (walls[i].transform.localScale.z >= 30)
            {
                mats[i].mainTextureScale = new Vector2(7, 1);
            }
            else if (walls[i].transform.localScale.z >= 20)
            {
                mats[i].mainTextureScale = new Vector2(5, 1);
            }
            else if (walls[i].transform.localScale.z >= 10)
            {
                mats[i].mainTextureScale = new Vector2(3, 1);
            }
        }

        // change pattern every 0.5 seconds, starting in 2 seconds
        InvokeRepeating("ChangeTexture", 2, 0.5f);
    }

    void ChangeTexture()
    {
        offset++;

        // reset at end of cycle
        if (offset > count)
        {
            offset = 0;
        }

        // for each wall, change texture to next in array
        for (int i = 0; i < count; i++)
        {
            mats[i].SetTexture("_BaseMap", patterns[(i + offset) % count]);
        }
    }
}
