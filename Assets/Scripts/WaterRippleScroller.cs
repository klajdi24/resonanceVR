using UnityEngine;

public class WaterRippleScroller : MonoBehaviour
{
    [Header("Material to scroll (water)")]
    public Renderer targetRenderer;

    [Header("Texture property names (URP/Built-in)")]
    public string baseMapPropertyURP = "_BaseMap";
    public string mainTexPropertyBuiltin = "_MainTex";

    [Header("Scroll speeds")]
    public Vector2 scrollSpeedA = new Vector2(0.015f, 0.01f);
    public Vector2 scrollSpeedB = new Vector2(-0.01f, 0.02f);

    private Material matInstance;
    private bool isURP;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (targetRenderer == null) return;

        matInstance = targetRenderer.material; 
        isURP = matInstance.HasProperty(baseMapPropertyURP);
    }

    private void Update()
    {
        if (matInstance == null) return;

        string prop = isURP ? baseMapPropertyURP : mainTexPropertyBuiltin;

        if (!matInstance.HasProperty(prop))
            return;

        Vector2 a = scrollSpeedA * Time.time;
        Vector2 b = scrollSpeedB * Time.time;

        
        Vector2 offset = new Vector2(a.x + b.x, a.y + b.y);
        matInstance.SetTextureOffset(prop, offset);
    }
}
