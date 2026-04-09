using UnityEngine;

public class PlayerDropShadow : MonoBehaviour
{
    [SerializeField] private Transform shadowVisual;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float rayDistance = 20f;
    [SerializeField] private float yOffset = 0.02f;
    [SerializeField] private float minScale = 0.35f;
    [SerializeField] private float maxScale = 1f;
    [SerializeField] private float minAlpha = 0.15f;
    [SerializeField] private float maxAlpha = 0.6f;
    [SerializeField] private float maxHeight = 8f;

    private Renderer shadowRenderer;
    private Material shadowMaterial;

    private void Awake()
    {
        if (shadowVisual != null)
        {
            shadowRenderer = shadowVisual.GetComponent<Renderer>();
            if (shadowRenderer != null)
                shadowMaterial = shadowRenderer.material;
        }
    }

    private void LateUpdate()
    {
        if (shadowVisual == null)
            return;

        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayDistance, groundLayers, QueryTriggerInteraction.Ignore))
        {
            shadowVisual.gameObject.SetActive(true);
            shadowVisual.position = hit.point + Vector3.up * yOffset;
            shadowVisual.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(90f, 0f, 0f);

            float t = Mathf.Clamp01(hit.distance / maxHeight);
            float scale = Mathf.Lerp(maxScale, minScale, t);
            shadowVisual.localScale = new Vector3(scale, scale, scale);

            if (shadowMaterial != null)
            {
                Color c = shadowMaterial.color;
                c.a = Mathf.Lerp(maxAlpha, minAlpha, t);
                shadowMaterial.color = c;
            }
        }
        else
        {
            shadowVisual.gameObject.SetActive(false);
        }
    }
}
