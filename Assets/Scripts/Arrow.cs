using UnityEngine;

public class Arrow : MonoBehaviour
{
    public EntityUnit unit;
    public UnitHelper unitHelper;
    private int speed = 22;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, unit.target.transform.position, speed * Time.deltaTime);
        transform.LookAt(unit.target.transform);

        if (Vector3.Distance(transform.position, unit.target.transform.position) <= unit.target.size)
        {
            unitHelper.Deal();

            Destroy(gameObject);
        }
    }
}
