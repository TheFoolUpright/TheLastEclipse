using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MenuFogToggle : MonoBehaviour
{
    [SerializeField] private ScriptableRendererData rendererData;
    [SerializeField] private string fogFeatureName = "Volumetric Fog";

    private ScriptableRendererFeature fogFeature;

    private void Awake()
    {
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature.name == fogFeatureName)
            {
                fogFeature = feature;
                break;
            }
        }
    }

    public void SetFogEnabled(bool enabled)
    {
        if (fogFeature != null)
            fogFeature.SetActive(enabled);
    }
}
