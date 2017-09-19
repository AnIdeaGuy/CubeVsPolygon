using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarAnimation : ImageAnimation {

    public HealthBarAnimation(GameObject image, int count, Vector2 position, Transform canvas) : base(image, count, position, canvas)
    { }

    public override void Animation(ImageIcon icon)
    {
        float y = Mathf.Sin(time * 4 + icon.index * .4f) * 6;
        icon.SetPosition(0, y);
    }
}
