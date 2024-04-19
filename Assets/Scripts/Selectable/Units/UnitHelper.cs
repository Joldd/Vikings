using UnityEngine;

public class UnitHelper : MonoBehaviour
{
    [SerializeField] EntityUnit unit;

    public void Deal()
    {
        if (!unit.target) return;
        if (unit.target.PV > 0)
        {
            unit.target.PV -= (int) unit.GetDamage();
            unit.target.healthBar.UpdateValue();
        }
    }
}
