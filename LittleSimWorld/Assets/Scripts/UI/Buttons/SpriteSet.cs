using UnityEngine;

namespace UI.Buttons
{
    [CreateAssetMenu(fileName = "Button Sprites", menuName = "UI/Buttons/Sprite Set", order = 0)]
    public class SpriteSet : ScriptableObject
    {
        public Sprite keyTop;
        public Sprite keyShadow;
    }
}