using UnityEngine;
using UnityEngine.UI;

public class HeroButton : MonoBehaviour
{
    private Button heroButton;
    public Hero myHero;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        heroButton = GetComponent<Button>();
        heroButton.onClick.AddListener(() =>
        {
            if (gameManager.isPathing) return;
            myHero.myTroop.Select();
            gameManager.cameraController.CenterTo(myHero.myTroop.transform.position);
        });
    }
}
