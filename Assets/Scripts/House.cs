using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class House : Selectable
{
    public List<GameObject> L_Vikings;
    public List<Button> L_Buttons;
    [SerializeField] Transform spawn;

    public override void Start()
    {
        base.Start();

        if (L_Vikings.Count == L_Buttons.Count)
        {
            for (int i = 0; i < L_Buttons.Count; i++)
            {
                int n = i;
                L_Buttons[i].onClick.AddListener(() =>
                {
                    GameManager.Instance.createViking(L_Vikings[n], spawn);
                });
            }
        }
        else
        {
            Debug.LogWarning("The Building " + name + " miss units or buttons");
        }
    }
}
