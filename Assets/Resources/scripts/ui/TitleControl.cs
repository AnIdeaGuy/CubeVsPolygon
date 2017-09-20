using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleControl : MonoBehaviour
{
    public GameObject title;
    public GameObject background;
    public Transform canvas;

    public GameObject BlankCharacterMenu;

    private TextAnimation playOp;
    private TextAnimation optionsOp;
    private TextAnimation exitOp;
    
    private GameObject titleInstance;
    private GameObject backgroundInstance;
    private float time = 0;
    private Vector2 originalSize;

    private int selectionIndex = 0;
    private const int MENU_COUNT = 3;
    private bool hitStart = false;
    private int prevSelected = 0;

	void Start ()
    {
        backgroundInstance = Instantiate(background, canvas);
        titleInstance = Instantiate(title, canvas);
        originalSize = titleInstance.transform.localScale;
        RectTransform canvasRect = canvas.transform.gameObject.GetComponent<RectTransform>();
        playOp = new TextAnimation("PLAY", new Vector2(-canvasRect.sizeDelta.x / 2 + 120, canvasRect.anchoredPosition.y - 100), BlankCharacterMenu, canvas);
        optionsOp = new TextAnimation("OPTIONS", new Vector2(-canvasRect.sizeDelta.x / 2 + 120, canvasRect.anchoredPosition.y - 164), BlankCharacterMenu, canvas);
        exitOp = new TextAnimation("EXIT", new Vector2(-canvasRect.sizeDelta.x / 2 + 120, canvasRect.anchoredPosition.y - 228), BlankCharacterMenu, canvas);
    }
	
	void Update ()
    {
        HandleInput();
        HandleMenu();
        HandleImages();
        time += Time.deltaTime;
	}

    private void HandleInput()
    {
        prevSelected = selectionIndex;
        if (Input.GetButtonDown("Up"))
            selectionIndex--;

        if (Input.GetButtonDown("Duck"))
            selectionIndex++;

        if (Input.GetButtonDown("Start"))
            hitStart = true;

        if (selectionIndex < 0)
            selectionIndex = MENU_COUNT - 1;

        if (selectionIndex >= MENU_COUNT)
            selectionIndex = 0;
    }

    private void HandleMenu()
    {
        playOp.Update();
        optionsOp.Update();
        exitOp.Update();

        bool sameSelected = prevSelected == selectionIndex;
        
        switch (selectionIndex)
        {
             case 0: // Play
             if (!sameSelected)
             {
             playOp.KillTheChars();
                playOp = MakeBounceText("PLAY", 100);
             }
             if (hitStart)
                 SceneManager.LoadScene("MainScene");
             break;
             case 1: // Options
             if (!sameSelected)
             {
                 optionsOp.KillTheChars();
                 optionsOp = MakeBounceText("OPTIONS", 164);
             }
             break;
             case 2: // Exit
             if (!sameSelected)
             {
                 exitOp.KillTheChars();
                 exitOp = MakeBounceText("EXIT", 228);
             }
             if (hitStart)
                 Application.Quit();
             break;
        }
    }

    private BounceAnimation MakeBounceText(string str, float y)
    {
        RectTransform canvasRect = canvas.transform.gameObject.GetComponent<RectTransform>();
        return new BounceAnimation(str, new Vector2(-canvasRect.sizeDelta.x / 2 + 120, canvasRect.anchoredPosition.y - y), BlankCharacterMenu, canvas);
    }

    private void HandleImages()
    {
        RectTransform canvasRect = canvas.transform.gameObject.GetComponent<RectTransform>();
        Vector2 anchorPoint = canvasRect.anchoredPosition;
        anchorPoint.x += 60;
        anchorPoint.y += 60;
        Vector2 positionTitle = new Vector2();
        positionTitle.x = Mathf.Cos(time * 1.8f) * 50;
        positionTitle.y = Mathf.Sin(time * 2.5f) * 20;
        titleInstance.transform.position = anchorPoint + positionTitle;
        titleInstance.transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(time * 2.2f) * 8);
        float size = 1 + Mathf.Sin(time * 3.3f) * .025f;
        titleInstance.transform.localScale = originalSize * size;

        backgroundInstance.GetComponent<RectTransform>().sizeDelta = canvasRect.sizeDelta;
        backgroundInstance.transform.position = canvasRect.anchoredPosition;
    }
}
