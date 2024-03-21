using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class House : Selectable
{
    public List<GameObject> L_Units;
    public List<Button> L_Buttons;
    [SerializeField] Transform spawn;
    
    Animator animator;

    Selectable currentUnit;
    Slider currentSliderUnit = null;
    bool isBuilding = false;

    public Constructible constructible;

    private float timer = 0;

    public bool isBase;

    public override void Start()
    {
        base.Start();

        //Health Bar
        healthBar = Instantiate(healthBar, transform.position, Quaternion.identity);
        healthBar.StartBar(gameObject);

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
                        if (L_Units[n])
                        {
                            currentUnit = L_Units[n].GetComponent<Selectable>();
                            sliderUnit.gameObject.SetActive(true);
                            currentSliderUnit = sliderUnit;
                            isBuilding = true;
                            timer = currentUnit.timeBuildMax;
                            GameManager.Instance.gold -= currentUnit.priceGold;
                            GameManager.Instance.updateRessources();
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

        if (isBase && tag == "Player") UIManager.Instance.Defeat();

        if (isBase && tag == "Enemy") UIManager.Instance.Victory();
    }

    private void createUnit(Selectable unit, Transform spawn)
    {
         Selectable v = Instantiate(unit);
         v.transform.position = spawn.transform.position;     
    }

    private void Update()
    {
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
            timer -= Time.deltaTime;

            currentSliderUnit.value = (currentUnit.timeBuildMax - timer) / currentUnit.timeBuildMax;

            if (currentSliderUnit.value >= 1)
            {
                createUnit(currentUnit, spawn);
                currentSliderUnit.gameObject.SetActive(false);
                isBuilding = false;
                currentSliderUnit = null;
                currentUnit = null;
            }
        }
    }
}
