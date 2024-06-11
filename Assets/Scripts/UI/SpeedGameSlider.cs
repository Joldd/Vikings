using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpeedGameSlider : Slider
{
    UIManager uIManager;
    [SerializeField] TextMeshProUGUI currentMultiplierTMP;
    private GameManager gameManager;

    protected override void Start()
    {
        gameManager = GameManager.Instance;
        uIManager = UIManager.Instance;
        currentMultiplierTMP.text = "x 1";
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (gameManager.isPause) return;

        base.OnPointerUp(eventData);

        if (value <= 0.25f)
        {
            value = 0;
        }
        else if (value < 0.75f && value > 0.25f)
        {
            value = 0.5f;
        }
        else if (value >= 0.75f)
        {
            value = 1;
        }

        float speedMultiplier = 2 * value * value + value + 1;

        uIManager.SpeedGame(speedMultiplier);
        currentMultiplierTMP.text = "x " + speedMultiplier;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (gameManager.isPause) return;
        base.OnDrag(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (gameManager.isPause) return;
        base.OnPointerDown(eventData);
    }
}
