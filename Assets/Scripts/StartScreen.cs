using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    private VisualElement root;
    private Button startButton;
    private Button quitButton;

    [SerializeField] string gameScene;

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        startButton = root.Q<Button>("Start");
        quitButton = root.Q<Button>("Quit");

        //subscribe attack buttons
        startButton.clicked += StartPressed;
        quitButton.clicked += QuitPressed;
    }

    private void StartPressed()
    {
        SceneManager.LoadScene(gameScene);
    }

    private void QuitPressed()
    {
        Application.Quit();
    }

}
