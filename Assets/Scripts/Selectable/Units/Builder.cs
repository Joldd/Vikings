using UnityEngine;
using UnityEngine.AI;

public class Builder : MonoBehaviour
{
    public Constructible constructible;
    
    [SerializeField] private float speed;
    public bool isRunning;
    
    private NavMeshAgent _navMeshAgent;
    private Animator _anim;
    private GameObject body;
    
    public Player owner;

    private GameManager gameManager;
    
    public void Start()
    {
        gameManager = GameManager.Instance;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        body = transform.Find("Body").gameObject;

        _anim = body.GetComponent<Animator>();
    }

    private void Update()
    {
        if (isRunning)
        {
            _anim.Play("Run");
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(constructible.transform.position);
        }
        else
        {
            _anim.Play("Idle");
            _navMeshAgent.isStopped = true;
        }

        if (Vector3.Distance(transform.position, constructible.transform.position) < 0.2f)
        {
            constructible.ChangeOwnership(owner);
        }
    }
}
