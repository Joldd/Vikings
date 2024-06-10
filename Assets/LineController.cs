using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private Texture[] textures;
    private int animationStep;
    private float fpsCounter;
    [SerializeField] private float fps = 30f;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        fpsCounter += Time.deltaTime;
        if ( fpsCounter >= 1 / fps)
        {
            animationStep++;
            if (animationStep == textures.Length) animationStep = 0;
            lineRenderer.material.SetTexture("_MainTex", textures[animationStep]);
            fpsCounter = 0;
        }
    }
}
