using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To be used by the main camera to display messages and UI things
/// </summary>
public class DisplayControl : MonoBehaviour
{
    public GameObject gameOverFont;
    public GameObject healthIcon;
    public Transform canvas;
    private TextAnimation gameOver = null;
    public ImageAnimation healthbar= null;
    private bool firstRun = true;
    static public DisplayControl displayControl;
    private bool damaging = false;
    private RectTransform canvasRect;
        
    public void GameStart()
    {
        RestartHealth();
    }

    public void RestartHealth()
    {
        if (healthbar != null)
            healthbar.KillTheImages();
        healthbar = new HealthBarAnimation(healthIcon, (int) Mathf.Floor(PlayahMove.hp), new Vector2(0, - canvasRect.sizeDelta.y / 2 + 40), canvas);
    }

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
                message = "actually try, k?";
                break;
            case 4:
                message = "no one loves you";
                break;
            case 5:
                message = "your face is bad";
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

    public void StartDamageAnimation()
    {
        healthbar.KillTheImages();
        healthbar = new HealthDamageAnimation(healthIcon, (int) Mathf.Floor(PlayahMove.hp), new Vector2(0, -canvasRect.sizeDelta.y / 2 + 40), canvas);
        damaging = true;
    }

    void Start()
    {
        displayControl = this;
        canvasRect = canvas.gameObject.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (firstRun)
        {
            GameStart();
            firstRun = false;
        }
        if (PlayahMove.alive)
        {
            if (gameOver != null)
            {
                gameOver.KillTheChars();
                gameOver = null;
            }

            if (healthbar != null)
            {
                if (damaging && healthbar.time > .5f)
                    RestartHealth();
                healthbar.Update();
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
