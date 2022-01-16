using UnityEngine;

//-----------------------------------------------------------------------------
// name: LightManager.cs
// desc: canyon scene lighting update with progress; uses LightingPreset.cs
//-----------------------------------------------------------------------------

[ExecuteInEditMode]
public class LightManager : MonoBehaviour
{
    [SerializeField] private Light dirLight;
    [SerializeField] private LightingPreset preset;
    [SerializeField, Range(0, 24)] private float timeOfDay;

    public GameObject cam;
    public float speed = 0.2f;

    void Update()
    {
        // scale time (set sun) according to player progress down canyon
        float invLerp = Mathf.InverseLerp(0, 1000, cam.transform.position.x);
        float timeLerp = Mathf.Lerp(12, 0, invLerp);

        if (Application.isPlaying)
        {
            UpdateLighting(timeLerp / 24f);
        }
        else
        {
            // set time using slider in edit mode
            UpdateLighting(timeOfDay / 24f);
        }
    }

    // change lighting settings according to current time
    private void UpdateLighting(float time)
    {
        RenderSettings.ambientLight = preset.ambientColor.Evaluate(time);
        RenderSettings.fogColor = preset.fogColor.Evaluate(time);
        dirLight.color = preset.directionalColor.Evaluate(time);

        Vector3 currentRotation = new Vector3((time * 360f) - 90f, 30, 0);
        dirLight.transform.localRotation = Quaternion.Euler(currentRotation);
    }
}
