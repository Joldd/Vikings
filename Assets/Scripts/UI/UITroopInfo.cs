using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITroopInfo : MonoBehaviour
{
    [SerializeField] private Transform gridUnitImageParent;
    [SerializeField] private TextMeshProUGUI attackTextInfo, attackSpeedTextInfo, defenseTextInfo, speedTextInfo, rangeTextInfo;

    [SerializeField] private UIUnitFrameTroopInfo unitFramePrefab;
    
    private List<UIUnitFrameTroopInfo> L_InstanciatedUnitImages = new List<UIUnitFrameTroopInfo>();

    public Button btnGo;
    
    public void SetupTroop(Troop troop)
    {
        troop.uiInfos = this;

        btnGo.onClick.RemoveAllListeners();

        btnGo.onClick.AddListener(() =>
        {
            troop.Run();
        });

        if (L_InstanciatedUnitImages.Count > 0)
        {
            foreach (var unitFrame in L_InstanciatedUnitImages)
            {
                Destroy(unitFrame.gameObject);
            }    
            L_InstanciatedUnitImages.Clear();
        }
        
        foreach (var unit in troop.L_Units)
        {
            UIUnitFrameTroopInfo newUIUnitFrame = Instantiate(unitFramePrefab, gridUnitImageParent);
            newUIUnitFrame.SetupUnitData(unit);
            L_InstanciatedUnitImages.Add(newUIUnitFrame);
            
        }

        EntityUnit troopEntityUnity = troop.L_Units[0];

        attackTextInfo.text = troopEntityUnity.AttackDamage.ToString();
        attackSpeedTextInfo.text = troopEntityUnity.AttackSpeed.ToString();
        defenseTextInfo.text = troopEntityUnity.Armor.ToString();
        speedTextInfo.text = troopEntityUnity.Speed.ToString();
        rangeTextInfo.text = troopEntityUnity.Range.ToString();

        btnGo.interactable = false;
    }
}
