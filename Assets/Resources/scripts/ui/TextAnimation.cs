using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAnimation
{
    /// <summary>
    /// The list of GameObject characters in the string.
    /// </summary>
    private List<Character> characterList = new List<Character>();
    /// <summary>
    /// The string that started it all.
    /// </summary>
    public string text = "";

    public float time = 0;

    public float width;

    /// <summary>
    /// Creates the text animation object.
    /// </summary>
    /// <param name="_text">The string that turns into the list of GameObject.</param>
    /// <param name="blankCharacter">The GameObject character that determines the font.</param>
    /// <param name="canvas">The reference to the canvas transaform to place the characterss.</param>
    /// 
    public TextAnimation(string _text, Vector2 _position, GameObject blankCharacter, Transform canvas)
    {
        text = _text;
        char[] chars = text.ToCharArray();
        width = 0;
        float x = 0;
        foreach (char c in chars)
        {
            GameObject obj = Object.Instantiate(blankCharacter, canvas);
            obj.GetComponent<Text>().text = "" + c;
            RectTransform rect = obj.GetComponent<RectTransform>();
            RectTransform canvasRect = canvas.gameObject.GetComponent<RectTransform>();
            Character chara = new Character(c, obj, characterList.Count - 1, new Vector2(canvasRect.anchoredPosition.x + _position.x, canvasRect.anchoredPosition.y + _position.y));
            characterList.Add(chara);
            width += chara.GetWidth();
        }
        foreach (Character c in characterList)
        {
            Vector3 newPosition = new Vector3(c.originalPosition.x -width / 2 + x, c.originalPosition.y, 0);
            c.gameObject.transform.position = newPosition;
            c.originalPosition = newPosition;
            x += c.GetWidth();
        }
    }

    public void Update()
    {
        foreach (Character c in characterList)
            Animation(c);
        time += Time.deltaTime;
    }

    /// <summary>
    /// Override this to make a text animation. This is called for each character in the string.
    /// </summary>
    /// <param name="character">The GameObject for the current character.</param>
    /// <param name="index"></param>
    virtual public void Animation(Character character)
    {

    }

    /// <summary>
    /// Destroys the instances of every character.
    /// </summary>
    public void KillTheChars()
    {
        foreach (Character c in characterList)
            Object.Destroy(c.gameObject);
        characterList.Clear();
    }
}

public class Character
{
    public GameObject gameObject;
    public int index;
    public Vector2 originalPosition;
    public char character;
    public Character(char _character, GameObject _gameobject, int _index, Vector2 _originalPosition)
    {
        character = _character;
        gameObject = _gameobject;
        index = _index;
        originalPosition = _originalPosition;
    }

    public void SetPosition(float x, float y)
    {
        gameObject.transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, 0);
    }

    /// <summary>
    /// Gets the width of the character.
    /// </summary>
    /// <returns>The kickass float of the character width.</returns>
    public float GetWidth()
    {
        Text textComponent = gameObject.GetComponent<Text>();
        CharacterInfo info;
        textComponent.font.GetCharacterInfo(character, out info, textComponent.fontSize);
        return info.advance;
    }
}