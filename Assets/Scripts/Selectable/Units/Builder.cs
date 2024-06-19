using UnityEngine;
using UnityEngine.AI;

public class Builder : MonoBehaviour
{
    public Constructible constructible;

    [SerializeField] private float speed;
    public bool isRunning;

    public NavMeshAgent _navMeshAgent;
    [SerializeField] private Animator _anim;
    private GameObject body;
    
    public Player owner;
    
    public void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = speed;
        body = transform.Find("Body").gameObject;

        _anim = body.GetComponent<Animator>();

        Stop();
    }

    private void Update()
    {

        if (!_navMeshAgent.enabled && isRunning)
        {
            transform.position = Vector3.MoveTowards(transform.position, constructible.transform.position, speed * Time.deltaTime);
            transform.LookAt(constructible.transform.position);
        }

        if (Vector3.Distance(transform.position, constructible.transform.position) < 0.2f)
        {
            constructible.ChangeOwnership(owner);
        }
    }

    public void Go()
    {
        isRunning = true;
        _anim.Play("Run");
        if (_navMeshAgent.enabled)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(constructible.transform.position);
        }
    }

    public void Stop()
    {
        isRunning = false;
        _anim.Play("Idle");
        if(_navMeshAgent.enabled) _navMeshAgent.isStopped = true;
    }
}
