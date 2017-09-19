using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimation
{
    /// <summary>
    /// The list of GameObject characters in the string.
    /// </summary>
    private List<ImageIcon> imageList = new List<ImageIcon>();
    /// <summary>
    /// The string that started it all.
    /// </summary>
    public string text = "";

    public float time = 0;

    /// <summary>
    /// Creates the text animation object.
    /// </summary>
    /// <param name="_text">The string that turns into the list of GameObject.</param>
    /// <param name="image">The GameObject character that determines the font.</param>
    /// <param name="canvas">The reference to the canvas transaform to place the characterss.</param>
    /// 
    public ImageAnimation(GameObject image, int duplicateNum, Vector2 _position, Transform canvas)
    {
        float totalX = 0;
        float x = 0;
        for (int i = 0; i < duplicateNum; i++)
        {
            GameObject obj = Object.Instantiate(image, canvas);
            RectTransform rect = obj.GetComponent<RectTransform>();
            RectTransform canvasRect = canvas.gameObject.GetComponent<RectTransform>();
            ImageIcon img = new ImageIcon(obj, imageList.Count - 1, new Vector2(canvasRect.anchoredPosition.x + _position.x, canvasRect.anchoredPosition.y + _position.y));
            imageList.Add(img);
            totalX += img.GetWidth() + 2;
        }
        if (imageList.Count > 0)
            x = imageList[0].GetWidth() / 2;
        foreach (ImageIcon icon in imageList)
        {
            Vector3 newPosition = new Vector3(icon.originalPosition.x - totalX / 2 + x, icon.originalPosition.y, 0);
            icon.image.transform.position = newPosition;
            icon.originalPosition = newPosition;
            x += icon.GetWidth() + 2;
        }
    }

    public void Update()
    {
        foreach (ImageIcon icon in imageList)
            Animation(icon);
        time += Time.deltaTime;
    }

    /// <summary>
    /// Override this to make a text animation. This is called for each character in the string.
    /// </summary>
    /// <param name="icon">The GameObject for the current image.</param>
    virtual public void Animation(ImageIcon icon)
    {

    }

    /// <summary>
    /// Destroys the instances of every character.
    /// </summary>
    public void KillTheImages()
    {
        foreach (ImageIcon icon in imageList)
            Object.Destroy(icon.image);
        imageList.Clear();
    }
}

public class ImageIcon
{
    public GameObject image;
    public int index;
    public Vector2 originalPosition;
    public ImageIcon(GameObject _image, int _index, Vector2 _originalPosition)
    {
        image = _image;
        index = _index;
        originalPosition = _originalPosition;
    }

    public void SetPosition(float x, float y)
    {
        image.transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, 0);
    }

    /// <summary>
    /// Gets the width of the character.
    /// </summary>
    /// <returns>The kickass float of the character width.</returns>
    public float GetWidth()
    {
        return image.GetComponent<RectTransform>().sizeDelta.x;
    }
}