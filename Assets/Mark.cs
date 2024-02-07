using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mark : MonoBehaviour
{
    int layer_mask;
    public bool isDragging;

    private void Start()
    {
        layer_mask = LayerMask.GetMask("Floor");
    }

    private void OnMouseDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, layer_mask))
        {
            transform.position = hit.point;
        }
        isDragging = true;
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }
}
