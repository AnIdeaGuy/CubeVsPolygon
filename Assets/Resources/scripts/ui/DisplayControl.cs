using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// To be used by the main camera to display messages and UI things
/// </summary>
public class DisplayControl : MonoBehaviour
{
    static public DisplayControl displayControl;

    public GameObject gameOverFont;
    public GameObject healthIcon;
    public GameObject whiteOut;
    public Transform canvas;

    public ImageAnimation healthbar = null;

    private TextAnimation gameOver = null;

    private bool firstRun = true;
    private bool damaging = false;
    private RectTransform canvasRect;
    private bool firstFrame = true;

    private GameObject whiteOutInstance;
    private float whiteOutAlpha = 0;
    private int whiteOutDir = 0;
    private const float WHITE_OUT_SPEED = 4.0f;
    private float whiteOutFullTimer = 0;
    private const float WHITE_OUT_TIMER_MAX = .1f;

    public bool whiteOutIsFull = false;

    private Color fadeColor;
        
    public void GameStart()
    {
        RestartHealth();
        StretchWhiteToScreen();
    }

    private void StretchWhiteToScreen()
    {
        RectTransform canvasRect = canvas.gameObject.GetComponent<RectTransform>();
        whiteOutInstance.GetComponent<RectTransform>().sizeDelta = canvasRect.sizeDelta;
        whiteOutInstance.transform.position = new Vector3(canvasRect.anchoredPosition.x, canvasRect.anchoredPosition.y, 1);
    }

    public void StartWhiteOut()
    {
        if (whiteOutDir == 0)
        {
            fadeColor = PlayahMove.alive ? Color.white : Color.black;
            whiteOutDir = 1;
            whiteOutAlpha = .1f;
        }
    }

    public void RestartHealth()
    {
        if (healthbar != null)
            healthbar.KillTheImages();
        damaging = false;
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
        whiteOutInstance = Instantiate(whiteOut, canvas);
        canvasRect = canvas.gameObject.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (firstFrame)
        {
            GameStart();
            firstFrame = false;
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

        if (whiteOut != null)
        {
            if (whiteOutAlpha >= 1)
            {
                whiteOutDir = -1;

                if (whiteOutIsFull)
                {
                    if (whiteOutFullTimer > 0)
                        whiteOutFullTimer -= Time.deltaTime;
                    else
                    {
                        whiteOutIsFull = false;
                    }
                }
                else
                {
                    whiteOutFullTimer = WHITE_OUT_TIMER_MAX;
                    whiteOutIsFull = true;
                }
            }
            if (whiteOutAlpha <= 0 && whiteOutDir == -1)
            {
                whiteOutAlpha = 0;
                whiteOutDir = 0;
            }
            if (!whiteOutIsFull)
            {
                whiteOutAlpha += WHITE_OUT_SPEED * whiteOutDir * Time.deltaTime;
            }

            Color newFadeColor = fadeColor;
            newFadeColor.a = whiteOutAlpha;
            whiteOutInstance.GetComponent<Image>().color = newFadeColor;
        }
    }
}
