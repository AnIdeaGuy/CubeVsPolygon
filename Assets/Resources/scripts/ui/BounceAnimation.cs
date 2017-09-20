using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAnimation : TextAnimation
{
    public BounceAnimation(string _text, Vector2 _position, GameObject _blankCharacter, Transform _canvas) : base(_text, _position, _blankCharacter, _canvas)
    {
    }

    override public void Animation(Character character)
    {
        float time2 = time - .1f * character.index;
        float progress = Mathf.Max(1.2f - time2, 0);
        float y = -Mathf.Sin(time2 * 6) * progress * 8;
        character.SetPosition(0, y);
    }
}
