using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class House : Selectable
{
    public List<GameObject> L_Units;
    public List<Button> L_Buttons;
    [SerializeField] Transform spawn;
    
    Animator animator;
    Slider sliderBuilding;

    Selectable currentUnit = null;
    Slider currentSliderUnit = null;
    bool isBuilding = false;

    public override void Start()
    {
        base.Start();

        canvas = transform.Find("CanvasBuilding").gameObject;

        animator = GetComponent<Animator>();
        animator.Play("Build");

        sliderBuilding = transform.Find("HUDBuilding").Find("SliderBuilding").GetComponent<Slider>();
        sliderBuilding.enabled = false;

        if (L_Units.Count == L_Buttons.Count)
        {
            for (int i = 0; i < L_Buttons.Count; i++)
            {
                Slider sliderUnit = L_Buttons[i].transform.Find("Slider").GetComponent<Slider>();
                sliderUnit.gameObject.SetActive(false);
                int n = i;
                L_Buttons[i].onClick.AddListener(() =>
                {
                    if (!isBuilding && GameManager.Instance.gold >= L_Units[n].GetComponent<Selectable>().priceGold)
                    {
                        currentUnit = GameManager.Instance.createUnit(L_Units[n], spawn).GetComponent<Selectable>();
                        if (currentUnit != null)
                        {
                            sliderUnit.gameObject.SetActive(true);
                            currentSliderUnit = sliderUnit;
                            isBuilding = true;
                        }
                    }
                });
            }
        }
        else
        {
            Debug.LogWarning("The Building " + name + " miss units or buttons");
        }
    }

    public override void Update()
    {
        base .Update();

        /////////////////////////  BuildingHouse ///////////////////////
        if (!isBuilt)
        {
            timeBuild -= Time.deltaTime;
            sliderBuilding.value = (timeBuildMax - timeBuild) / timeBuildMax;
        }
        if ( timeBuild <= 0 && !isBuilt)
        {
            animator.Play("Idle");
            isBuilt = true;
            sliderBuilding.gameObject.SetActive(false);
        }

        /////////////////////////  BuildingViking ///////////////////////
        if (isBuilding)
        {
            currentSliderUnit.value = (currentUnit.timeBuildMax - currentUnit.timeBuild) / currentUnit.timeBuildMax;
            if (currentUnit.isBuilt)
            {
                currentSliderUnit.gameObject.SetActive(false);
                isBuilding = false;
                currentSliderUnit = null;
                currentUnit = null;
            }
        }
    }
}
