using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public HealthBar healthBar;
    public Animator animator;

    //Stats
    public int PV;

    //Construction
    public float timeBuildMax;
    public bool isBuilt = false;
    public float timeBuild;
    public int priceReputation;
    public int priceGold;

    public virtual void Start()
    {
        timeBuild = timeBuildMax;

        animator = GetComponent<Animator>();

        //Health Bar
        healthBar = Instantiate(healthBar, transform.position, Quaternion.identity);
        healthBar.StartBar(gameObject);
        healthBar.UpdateValue();
    }

    public virtual void Die()
    {
        Destroy(healthBar.gameObject);
    }
}
