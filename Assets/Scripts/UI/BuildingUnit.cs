using UnityEngine;
using UnityEngine.UI;

public class BuildingUnit : MonoBehaviour
{
    public Image myImage;
    public Slider mySlider;
    public EntityUnit myEntityUnit;
    public House myHouse;

    public void StopBuilUnit()
    {
        myHouse.StopBuildUnit(this);
    }

}
