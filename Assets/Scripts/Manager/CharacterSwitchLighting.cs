using UnityEngine;
using UnityEngine.Rendering;

public class CharacterSwitchLighting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;

    [Header("Directional Lights")]
    [SerializeField] private Light sunLight;
    [SerializeField] private Light moonLight;

    [Header("Skyboxes")]
    [SerializeField] private Material sunSkybox;
    [SerializeField] private Material moonSkybox;

    [Header("Ambient Colors")]
    [SerializeField] private Color sunAmbientColor = new Color(0.79216f, 0.47843f, 0.43137f, 1f);
    [SerializeField] private Color moonAmbientColor = new Color(0.44706f, 0.43137f, 0.79216f, 1f);

    [Header("Optional Extra Lights")]
    [SerializeField] private GameObject[] sunOnlyLights;
    [SerializeField] private GameObject[] moonOnlyLights;

    private void OnEnable()
    {
        if (playerController != null)
        {
            playerController.OnCharacterChanged += ApplyLighting;
        }
    }

    private void Start()
    {
        if (playerController == null)
        {
            Debug.LogError("PlayerLighting: PlayerController reference is missing.");
            enabled = false;
            return;
        }

        ApplyLighting(playerController.CurrentCharacter);
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.OnCharacterChanged -= ApplyLighting;
        }
    }

    private void ApplyLighting(Character character)
    {
        Debug.Log($"ApplyLighting called with: {character}");

        bool isSun = character == Character.Sun;

        if (sunLight != null) sunLight.enabled = isSun;
        if (moonLight != null) moonLight.enabled = !isSun;

        Debug.Log($"Sun enabled: {sunLight.enabled}, Moon enabled: {moonLight.enabled}");

        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = isSun ? sunAmbientColor : moonAmbientColor;

        if (isSun && sunSkybox != null)
        {
            RenderSettings.skybox = sunSkybox;
        }
        else if (!isSun && moonSkybox != null)
        {
            RenderSettings.skybox = moonSkybox;
        }

        DynamicGI.UpdateEnvironment();

        SetGroupActive(sunOnlyLights, isSun);
        SetGroupActive(moonOnlyLights, !isSun);
    }

    private void SetGroupActive(GameObject[] objects, bool isActive)
    {
        if (objects == null) return;

        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }
        }
    }
}