using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverAnimation : TextAnimation
{
    public GameOverAnimation(string _text, Vector2 _position, GameObject _blankCharacter, Transform _canvas) : base(_text, _position, _blankCharacter, _canvas)
    {
    }

    override public void Animation(Character character)
    {
        float time2 = time - .1f * character.index;
        float progress = Mathf.Max(1.5f - time2, 0);
        float x = -Mathf.Cos(time2 * 4) * progress * 500;
        float y = -Mathf.Sin(time2 * 4) * progress * 500;
        character.SetPosition(x, y);
    }
}
