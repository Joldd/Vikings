using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class House : Selectable
{
    public List<GameObject> L_Vikings;
    public List<Button> L_Buttons;
    [SerializeField] Transform spawn;
    
    Animator animator;
    Slider sliderBuilding;

    Viking currentViking = null;
    Slider currentSliderViking = null;
    bool isBuilding = false;

    public override void Start()
    {
        base.Start();

        canvas = transform.Find("CanvasBuilding").gameObject;

        animator = GetComponent<Animator>();
        animator.Play("Build");

        sliderBuilding = transform.Find("HUDBuilding").Find("SliderBuilding").GetComponent<Slider>();
        sliderBuilding.enabled = false;

        if (L_Vikings.Count == L_Buttons.Count)
        {
            for (int i = 0; i < L_Buttons.Count; i++)
            {
                Slider sliderViking = L_Buttons[i].transform.Find("Slider").GetComponent<Slider>();
                sliderViking.gameObject.SetActive(false);
                int n = i;
                L_Buttons[i].onClick.AddListener(() =>
                {
                    if (!isBuilding && GameManager.Instance.gold >= L_Vikings[n].GetComponent<Viking>().priceGold)
                    {
                        currentViking = GameManager.Instance.createViking(L_Vikings[n], spawn).GetComponent<Viking>();
                        if (currentViking != null)
                        {
                            sliderViking.gameObject.SetActive(true);
                            currentSliderViking = sliderViking;
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

    private void Update()
    {
        /////////////////////////  BuildingHouse ///////////////////////
        if ( timeBuild <= 0 && !isBuilt)
        {
            animator.Play("Idle");
            isBuilt = true;
            canBeSelected = true;
            sliderBuilding.gameObject.SetActive(false);
        }
        else
        {
            timeBuild -= Time.deltaTime;
            sliderBuilding.value = (timeBuildMax - timeBuild) / timeBuildMax;
        }

        /////////////////////////  BuildingViking ///////////////////////
        if (isBuilding)
        {
            currentSliderViking.value = (currentViking.timeBuildMax - currentViking.timeBuild) / currentViking.timeBuildMax;
            if (currentViking.isBuilt)
            {
                currentSliderViking.gameObject.SetActive(false);
                isBuilding = false;
                currentSliderViking = null;
                currentViking = null;
            }
        }
    }
}
