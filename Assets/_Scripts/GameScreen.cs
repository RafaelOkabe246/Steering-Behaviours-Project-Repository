using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameScreen : MonoBehaviour
{
    public GameObject endGameUi;

    private void Start()
    {
        endGameUi.SetActive(false);

    }

    private void OnEnable()
    {
        GameplayController.instance.onGameEnded += DisplayEndGameUI;
    }
    private void OnDisable()
    {
        GameplayController.instance.onGameEnded -= DisplayEndGameUI;
    }

    void DisplayEndGameUI()
    {
        endGameUi.SetActive(true);
    }
}
