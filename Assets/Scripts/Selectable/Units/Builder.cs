using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public Constructible constructible;
    [SerializeField] private float speed;
    public bool isRunning;
    private Animator _anim;
    private GameObject body;

    public void Start()
    {
        body = transform.Find("Body").gameObject;

        _anim = body.GetComponent<Animator>();
    }

    private void Update()
    {
        if (isRunning)
        {
            _anim.Play("Run");
            transform.position = Vector3.MoveTowards(transform.position, constructible.transform.position, speed * Time.deltaTime);
            transform.LookAt(constructible.transform);
        }
        else
        {
            _anim.Play("Idle");
        }

        if (Vector3.Distance(transform.position, constructible.transform.position) < 0.1f){
            constructible.BecomeConstructible();
        }
    }
}
