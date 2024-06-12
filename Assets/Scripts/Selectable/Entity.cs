using System;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    public Animator animator;

    //Stats
    public int PV;
    public int maxPV;
    public float size;

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
        PV = maxPV;
        animator = GetComponent<Animator>();
    }

    public virtual void Die() { }
    
    public virtual void TakeDamage(float damage)
    {
        PV -= (int) damage;
        onDamageTaken.Invoke((float) PV/maxPV);
    }

    public void AddListenerOnDamageTaken(UnityAction<float> action)
    {
        onDamageTaken.AddListener(action);
    }

}
