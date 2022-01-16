using UnityEngine;

//------------------------------------------------------------------------------
// name: LightingPreset.cs
// desc: serializable lighting preset, including ambient light color, directional
//       light color, and fog color
//------------------------------------------------------------------------------

[System.Serializable]
[CreateAssetMenu(fileName = "Lighting Preset",
                 menuName = "Scriptables/Lighting Preset",
                 order = 1)]

public class LightingPreset : ScriptableObject
{
    public Gradient ambientColor;
    public Gradient directionalColor;
    public Gradient fogColor;
}