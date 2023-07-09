using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public FruitManager fruitManager;
    public Snake snake;

    public TMPro.TextMeshProUGUI text;
    public Button retryButton;
    public Button exitGameButton;
    public Image panel;

    public void TriggerGameOver()
    {
        snake.StopAllCoroutines();
        fruitManager.StopAllCoroutines();
        Destroy(snake);
        panel.gameObject.SetActive(true);
        text.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        exitGameButton.gameObject.SetActive(true);
    }

    public void Retry()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
