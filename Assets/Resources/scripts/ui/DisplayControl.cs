using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To be used by the main camera to display messages and UI things
/// </summary>
public class DisplayControl : MonoBehaviour
{
    public GameObject gameOverFont;
    public Transform canvas;
    private TextAnimation gameOver = null;
    private bool firstRun = true;

    private void GameOverStart()
    {
        int rng = firstRun ? 0 : Random.Range(0, 8);
        firstRun = false;
        string message;
        switch (rng)
        {
            default:
                message = "game over";
                break;
            case 1:
                message = "you suck";
                break;
            case 2:
                message = "fuck you";
                break;
            case 3:
                message = "try next time";
                break;
            case 4:
                message = "no one loves you";
                break;
            case 5:
                message = "this is easy";
                break;
            case 6:
                message = "loser";
                break;
            case 7:
                message = "random message";
                break;
        }
        gameOver = new GameOverAnimation(message, Vector2.zero, gameOverFont, canvas);
    }

    void Update()
    {
        if (PlayahMove.alive)
        {
            if (gameOver != null)
            {
                gameOver.KillTheChars();
                gameOver = null;
            }
        }
        else
        {
            if (gameOver == null)
                GameOverStart();
            gameOver.Update();
        }
	}
}
