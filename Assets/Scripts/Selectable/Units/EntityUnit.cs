using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum Type
{
    Paladin,
    Guard,
    Mutant
}

public class EntityUnit : Entity
{
    public State state = State.SLEEPING;

    public Entity target;
    public float timerAttackMax;
    float timerAttack = 0f;

    public int speed;
    public int range;
    public int damage = 1;
    public int maxTroop;

    GameObject body;

    [SerializeField] bool enemyStop;

    public Constructible areaToCapture;

    [SerializeField] int goldToWin;

    public Type type;

    public Troop myTroop;

    public Outline outline;

    public override void Start()
    {
        base.Start();

        healthBar.UpdateValue();

        body = transform.Find("Body").gameObject;
        animator = body.GetComponent<Animator>();
    }

    public void Attack()
    {
        animator.speed = 1f / timerAttackMax;
        animator.Play("Attack");
    }

    public override void Die()
    {
        base.Die();

        if (areaToCapture)
        {
            if (tag == "Enemy") areaToCapture.enemyCapturing = false;
            if (tag == "Player") areaToCapture.playerCapturing = false;
        }

        if (tag == "Enemy")
        {
            GameManager.Instance.gold += goldToWin;
            GameManager.Instance.updateRessources();
        }

        if (myTroop) myTroop.L_Units.Remove(this);
    }
}
