using System;
using UnityEngine;
using UnityEngine.Events;

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

    private UnityEvent<float> onDamageTaken = new UnityEvent<float>();

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
    
    public void TakeDamage(float damage)
    {
        PV -= (int) damage;
        healthBar.UpdateValue();
        onDamageTaken.Invoke(healthBar.slider.value);
    }

    public void AddListenerOnDamageTaken(UnityAction<float> action)
    {
        onDamageTaken.AddListener(action);
    }

}
