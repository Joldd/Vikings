using System.Collections.Generic;
using UnityEngine;

public class Troop : MonoBehaviour
{
    public List<Viking> L_Vikings = new List<Viking>();
    private float radius = 0.8f;
    public Type type;

    public void AddUnit(Viking viking)
    {
        viking.transform.SetParent(transform);
        L_Vikings.Add(viking);
        for (int i = 0; i < L_Vikings.Count; i++)
        {
            Viking v = L_Vikings[i];
            v.transform.position = new Vector3(transform.position.x + radius * Mathf.Cos(i * 2 * Mathf.PI / L_Vikings.Count), transform.position.y, transform.position.z + radius * Mathf.Sin(i * 2 * Mathf.PI / L_Vikings.Count));
        }
    }
}
