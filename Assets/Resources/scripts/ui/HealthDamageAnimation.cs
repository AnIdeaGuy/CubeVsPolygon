using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDamageAnimation : ImageAnimation {

    public HealthDamageAnimation(GameObject image, int count, Vector2 position, Transform canvas) : base(image, count, position, canvas)
    { }

    public override void Animation(ImageIcon icon)
    {
        float x = Random.Range(-2.0f, 2.0f);
        float y = Random.Range(-2.0f, 2.0f);
        icon.SetPosition(x, y);
    }
}
