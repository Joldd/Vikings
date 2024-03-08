using UnityEngine;

public interface ISelectable
{
    bool isSelect { get; set; }
    int PV { get; set; }

    void Die();
    void Select();
    void UnSelect();

    void OnMouseDown();
}
