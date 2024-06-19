using UnityEngine;

public class LevelLimit : MonoBehaviour
{
    public bool isView;
    Renderer render;

    private void Start()
    {
        render = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (render.isVisible)
        {
            isView = true;
        }
        else
        {
            isView = false;
        }
    }
}
