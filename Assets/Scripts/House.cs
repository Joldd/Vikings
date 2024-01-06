using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class House : Selectable
{
    public List<GameObject> L_Vikings;
    public List<Button> L_Buttons;
    [SerializeField] Transform spawn;
    [SerializeField] float timeBuildMax;
    float timeBuild;
    Animator animator;
    bool isBuilt;
    Slider sliderBuilding;

    public override void Start()
    {
        base.Start();

        canBeSelected = false;
        timeBuild = timeBuildMax;
        animator = GetComponent<Animator>();
        animator.Play("Build");

        sliderBuilding = transform.Find("HUDBuilding").Find("SliderBuilding").GetComponent<Slider>();
        sliderBuilding.enabled = false;

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

    private void Update()
    {
        if ( timeBuild <= 0 && !isBuilt)
        {
            animator.Play("Idle");
            isBuilt = true;
            canBeSelected = true;
        }
        else
        {
            timeBuild -= Time.deltaTime;
            sliderBuilding.value = (timeBuildMax - timeBuild) / timeBuildMax;
        }
    }
}
