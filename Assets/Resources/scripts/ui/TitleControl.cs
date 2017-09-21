using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleControl : MonoBehaviour
{
    public GameObject title;
    public GameObject background;
    public GameObject menuCursor;
    public Transform canvas;

    public GameObject BlankCharacterMenu;

    private TextAnimation playOp;
    private TextAnimation optionsOp;
    private TextAnimation quitOp;
    
    private GameObject titleInstance;
    private GameObject backgroundInstance;
    private GameObject cursorInstance;

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
        cursorInstance = Instantiate(menuCursor, canvas);

        playOp = new TextAnimation("PLAY", new Vector2(-canvasRect.anchoredPosition.x + 120, canvasRect.anchoredPosition.y - 100), BlankCharacterMenu, canvas);
        optionsOp = new TextAnimation("OPTIONS", new Vector2(-canvasRect.anchoredPosition.x + 120, canvasRect.anchoredPosition.y - 164), BlankCharacterMenu, canvas);
        quitOp = new TextAnimation("EXIT", new Vector2(-canvasRect.anchoredPosition.x + 120, canvasRect.anchoredPosition.y - 228), BlankCharacterMenu, canvas);

        cursorInstance.GetComponent<RectTransform>().sizeDelta = new Vector2(optionsOp.width * 1.5f , 70);
        cursorInstance.GetComponent<Image>().color = new Color(1, 1, 1, .5f);
        cursorInstance.transform.position = new Vector2(120, canvasRect.sizeDelta.y - 135);
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
        {
            gameObject.GetComponent<AudioSource>().Play();
            selectionIndex--;
        }

        if (Input.GetButtonDown("Duck"))
        {
            gameObject.GetComponent<AudioSource>().Play();
            selectionIndex++;
        }

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
        quitOp.Update();

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
                 quitOp.KillTheChars();
                 quitOp = MakeBounceText("EXIT", 228);
             }
             if (hitStart)
                 Application.Quit();
             break;
        }
    }

    private void MakeCursor(float y)
    {
        RectTransform canvasRect = canvas.transform.gameObject.GetComponent<RectTransform>();
        Destroy(cursorInstance);
        cursorInstance = Instantiate(menuCursor, canvas);
        cursorInstance.GetComponent<RectTransform>().sizeDelta = new Vector2(optionsOp.width * 1.5f, 70);
        cursorInstance.transform.position = new Vector2(120, canvasRect.sizeDelta.y - y - 35);
        cursorInstance.GetComponent<Image>().color = new Color(1, 1, 1, .5f);
    }

    private BounceAnimation MakeBounceText(string str, float y)
    {
        MakeCursor(y);
        RectTransform canvasRect = canvas.transform.gameObject.GetComponent<RectTransform>();
        return new BounceAnimation(str, new Vector2(-canvasRect.anchoredPosition.x + 120, canvasRect.anchoredPosition.y - y), BlankCharacterMenu, canvas);
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
