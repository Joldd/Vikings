using UnityEngine;

public class UnitHelper : MonoBehaviour
{
    [SerializeField] EntityUnit unit;

    public void Deal()
    {
        if (!unit.target) return;
        unit.target.PV -= (int) unit.GetDamage();
        unit.target.healthBar.UpdateValue();
    }
}
