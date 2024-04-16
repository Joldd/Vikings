using UnityEngine;

public class UnitHelper : MonoBehaviour
{
    [SerializeField] EntityUnit unit;

    public void Deal()
    {
        if (!unit.target) return;
        unit.target.PV -= unit.damage;
        unit.target.healthBar.UpdateValue();
    }
}
