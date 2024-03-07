using UnityEngine;
using UnityEngine.UI;

public class Messenger : Selectable
{
    public Viking vikingSelected;
    [SerializeField] private Image panel;
    private Color startColor;
    [SerializeField] private Color chooseColor;
    private bool isMoving;
    [SerializeField] private float speed;

    public override void Start()
    {
        base.Start();
        startColor = panel.color;
    }

    public override void Update()
    {
        base.Update();

        if(isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, vikingSelected.transform.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, vikingSelected.transform.position) < 0.1f )
            {
                isMoving = false;
                vikingSelected.myWayPoints = vikingSelected.changingWayPoints;
                vikingSelected.Run();
            }
        }
    }

    public void ChooseViking()
    {
        if (!GameManager.Instance.isChoosingMessager)
        {
            GameManager.Instance.isChoosingMessager = true;
            panel.color = chooseColor;
        }
        else if (GameManager.Instance.isChoosingMessager)
        {
            GameManager.Instance.isChoosingMessager = false;
            panel.color = startColor;
        }
    }

    public void StopChooseViking()
    {
        GameManager.Instance.isChoosingMessager = !GameManager.Instance.isChoosingMessager;
        panel.color = startColor;
        GameManager.Instance.createNewPath();
    }

    public void Go()
    {
        isMoving = true;
    }
}
