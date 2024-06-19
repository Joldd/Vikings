using UnityEngine;

public class HealthBarHouse : HealthBar
{
    Entity unit;

    private void Update()
    {
        Vector3 worldTargetPosition = unit.transform.position + Vector3.up * healthBarUpOffset;
        rectTransform.position = Camera.main.WorldToScreenPoint(worldTargetPosition);

        if (unit && unit.tag == "Enemy" && unit.TryGetComponent<EntityUnit>(out EntityUnit e))
        {
            if (gameManager.fogWar.CheckVisibility(worldTargetPosition, 1))
            {
                blocToHide.SetActive(true);
            }
            else
            {
                blocToHide.SetActive(false);
            }
        }
    }
    
    public override void StartBar(GameObject target, Color color)
    {
        base.StartBar(target, color);
        unit = target.GetComponent<Entity>();
    }
    
    public void UpdateValue()
    {
        slider.value = (float) unit.PV / unit.maxPV;
    }


}