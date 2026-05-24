using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SoulCollectPopup : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField] private GameObject popupUI;
    [SerializeField] private Image promptImage;

    [Header("Prompt Sprites")]
    [SerializeField] private Sprite keyboardSprite;
    [SerializeField] private Sprite xboxSprite;

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInput playerInput;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (popupUI != null)
            popupUI.SetActive(false);
    }

    private void LateUpdate()
    {
        FaceCamera();
        UpdatePromptImage();
    }

    public void Show()
    {
        if (popupUI != null)
            popupUI.SetActive(true);
    }

    public void Hide()
    {
        if (popupUI != null)
            popupUI.SetActive(false);
    }

    private void FaceCamera()
    {
        if (popupUI == null || mainCamera == null || !popupUI.activeSelf)
            return;

        popupUI.transform.rotation = Quaternion.LookRotation(
            popupUI.transform.position - mainCamera.transform.position
        );
    }

    private void UpdatePromptImage()
    {
        if (promptImage == null || playerInput == null)
            return;

        string scheme = playerInput.currentControlScheme;

        if (scheme == "KeyboardMouse")
        {
            promptImage.sprite = keyboardSprite;
        }
        else if (scheme == "Gamepad")
        {
            promptImage.sprite = xboxSprite;
        }
    }
}