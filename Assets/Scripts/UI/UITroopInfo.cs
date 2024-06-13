using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITroopInfo : MonoBehaviour
{
    [SerializeField] private Transform gridUnitImageParent;
    [SerializeField] private TextMeshProUGUI attackTextInfo, attackSpeedTextInfo, defenseTextInfo, speedTextInfo, rangeTextInfo;

    [SerializeField] private UIUnitFrameTroopInfo unitFramePrefab;
    [SerializeField] private Color baseColor = Color.white, bonusColor = Color.green, malusColor = Color.red;
    
    private List<UIUnitFrameTroopInfo> L_InstanciatedUnitImages = new List<UIUnitFrameTroopInfo>();
    private Troop displayedTroop;
    
    public Button btnGo;

    public void SetupTroop(Troop troop)
    {
        displayedTroop = troop;
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

        UpdateBonus(troop);

        btnGo.interactable = false;
    }

    public void UpdateBonus(Troop troopRef)
    {
        if (troopRef == displayedTroop)
        {
            EntityUnit troopEntityUnity = troopRef.L_Units[0];
        
            attackTextInfo.text = troopEntityUnity.AttackDamage.ToString();
            attackTextInfo.color = troopEntityUnity.IsAttackModified.Item1 
                ? (troopEntityUnity.IsAttackModified.Item2 ? bonusColor : malusColor) 
                : baseColor;
            
            attackSpeedTextInfo.text = troopEntityUnity.AttackSpeed.ToString();
            attackSpeedTextInfo.color = troopEntityUnity.IsAttackModified.Item1 
                ? (troopEntityUnity.IsAttackModified.Item2 ? bonusColor : malusColor) 
                : baseColor;
            
            defenseTextInfo.text = troopEntityUnity.Armor.ToString();
            defenseTextInfo.color = troopEntityUnity.IsArmorModified.Item1 
                ? (troopEntityUnity.IsArmorModified.Item2 ? bonusColor : malusColor) 
                : baseColor;
            
            speedTextInfo.text = troopEntityUnity.Speed.ToString();
            speedTextInfo.color = troopEntityUnity.IsSpeedModified.Item1 
                ? (troopEntityUnity.IsSpeedModified.Item2 ? bonusColor : malusColor) 
                : baseColor;
            
            rangeTextInfo.text = troopEntityUnity.Range.ToString();
            rangeTextInfo.color = troopEntityUnity.IsRangeModified.Item1 
                ? (troopEntityUnity.IsRangeModified.Item2 ? bonusColor : malusColor) 
                : baseColor;
            
        }
    }
}
