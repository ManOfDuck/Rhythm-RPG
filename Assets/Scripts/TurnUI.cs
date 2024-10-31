using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private PlayerMove PlayerMove1;
    [SerializeField] private PlayerMove PlayerMove2;
    [SerializeField] private PlayerMove PlayerMove3;

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

        BattleManager.Instance.enemyHealed.AddListener(ChangeHealthVillian);
        BattleManager.Instance.enemyHurt.AddListener(ChangeHealthVillian);

        BattleManager.Instance.playerHurt.AddListener(ChangeHealthHero);
        BattleManager.Instance.playerHealed.AddListener(ChangeHealthHero);

        BattleManager.Instance.playerStatusesUpdated.AddListener(ChangeHealthHero);
        BattleManager.Instance.enemyStatusesUpdated.AddListener(ChangeHealthVillian);

        ChangeHealthHero();
        ChangeHealthVillian();
    }

    private void ChangeHealthHero(float i)
    {
        heroHealthLabel.text = (int) BattleManager.Instance.playerHealth + "/" + BattleManager.Instance.maxPlayerHealth;
    }

    private void ChangeHealthVillian(float i)
    {
        villianHealthLabel.text = (int) BattleManager.Instance.enemyHealth + "/" + BattleManager.Instance.maxEnemyHealth;
    }

    private void ChangeHealthHero()
    {
        heroHealthLabel.text = (int) BattleManager.Instance.playerHealth + "/" + BattleManager.Instance.maxPlayerHealth;
    }

    private void ChangeHealthVillian()
    {
        villianHealthLabel.text = (int) BattleManager.Instance.enemyHealth + "/" + BattleManager.Instance.maxEnemyHealth;
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
        if (AudioManager.Instance.minigameGoing || BattleManager.Instance.phase == BattleManager.Phase.EnemyTurn) return;
        Debug.Log("1");
        //call minigame
        PlayerMove1.GoFightMode();
        BattleManager.Instance.phase = BattleManager.Phase.PlayerTurn;
    }

    private void AttackButton2Pressed()
    {
        if (AudioManager.Instance.minigameGoing || BattleManager.Instance.phase == BattleManager.Phase.EnemyTurn) return;
        Debug.Log("2");
        //call minigame
        PlayerMove2.GoFightMode();
        BattleManager.Instance.phase = BattleManager.Phase.PlayerTurn;
    }

    private void AttackButton3Pressed()
    {
        print(AudioManager.Instance.minigameGoing);
        if (AudioManager.Instance.minigameGoing || BattleManager.Instance.phase == BattleManager.Phase.EnemyTurn) return;
        Debug.Log("3");
        //calll minigame
        PlayerMove3.GoFightMode();
        BattleManager.Instance.phase = BattleManager.Phase.PlayerTurn;
    }

}
