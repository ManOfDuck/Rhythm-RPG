using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseUI : MonoBehaviour
{
    private VisualElement root;
    private Label heroHealthLabel;
    private Label villianHealthLabel;
    private Button attack1Button;
    private Button attack2Button;
    private Button attack3Button;

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        attack1Button = root.Q<Button>("attack1-button");
        attack2Button = root.Q<Button>("attack2-button");
        attack3Button = root.Q<Button>("attack3-button");

        //subscribe attack buttons
        attack1Button.clicked += AttackButton1Pressed;
        attack2Button.clicked += AttackButton2Pressed;
        attack3Button.clicked += AttackButton3Pressed;

        villianHealthLabel = root.Q<Label>("VillianHealth");
        heroHealthLabel = root.Q<Label>("HeroHealth");
    }

    private void OnDestroy()
    {
        //unsubscribe attack buttons
        attack1Button.clicked -= AttackButton1Pressed;
        attack2Button.clicked -= AttackButton2Pressed;
        attack3Button.clicked -= AttackButton3Pressed;
    }

    private void AttackButton1Pressed()
    {
        //call minigame
    }

    private void AttackButton2Pressed()
    {
        //call minigame
    }

    private void AttackButton3Pressed()
    {
        //calll minigame
    }

}
