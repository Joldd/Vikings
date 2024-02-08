using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mark : MonoBehaviour
{
    int layer_mask;
    public bool isDragging;
    public bool deleted;
    Vector3 baseScale;
    float lastClickTime;

    const float DOUBLE_CLICK_TIME = 0.2f;

    private void Start()
    {
        layer_mask = LayerMask.GetMask("Floor");
        baseScale = transform.localScale;
    }

    private void OnMouseDrag()
    {
        if (GameManager.Instance.isPathing) return;

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

    private void OnMouseOver()
    {
        if (GameManager.Instance.isPathing) return;

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            deleted = true;
        }

        //DOUBLE CLICK => Create Mark
        if (Input.GetMouseButtonDown(0))
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if(timeSinceLastClick <= DOUBLE_CLICK_TIME)
            {
                Debug.Log("DblClick");
            }

            lastClickTime = Time.time;
        }
    }

    private void OnMouseEnter()
    {
        transform.localScale = 2*baseScale;
    }

    private void OnMouseExit()
    {
        transform.localScale = baseScale;
    }
}
