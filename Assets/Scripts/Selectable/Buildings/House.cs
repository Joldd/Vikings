using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class House : Selectable
{
    public List<GameObject> L_Units;
    public List<Button> L_Buttons;
    [SerializeField] Transform spawn;
    
    Animator animator;

    Selectable currentUnit = null;
    Slider currentSliderUnit = null;
    bool isBuilding = false;

    public Constructible constructible;

    public override void Start()
    {
        base.Start();

        canvas = transform.Find("CanvasBuilding").gameObject;

        animator = GetComponent<Animator>();

        if (!isBuilt) animator.Play("Build");

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

    public override void Die()
    {
        base.Die();

        if (constructible) constructible.houseDestroy = true;
    }

    public override void Update()
    {
        base .Update();

        /////////////////////////  BuildingHouse ///////////////////////
        if (!isBuilt)
        {
            timeBuild -= Time.deltaTime;
            healthBar.slider.value = (timeBuildMax - timeBuild) / timeBuildMax;
        }
        if ( timeBuild <= 0 && !isBuilt)
        {
            animator.Play("Idle");
            isBuilt = true;
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
