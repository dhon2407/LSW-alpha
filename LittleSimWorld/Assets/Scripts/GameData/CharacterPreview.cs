using CharacterData;
using UnityEngine;
using UnityEngine.UI;

namespace GameFile
{
    public class CharacterPreview : MonoBehaviour
    {
        public Image Body, Hair, Head, Shirt, Pants;
        public Image Hand_Left, Hand_Right;


        public void SetHair(CharacterSpriteSet set)
        {
            if (set != null)
                Hair.sprite = set.Bot;
        }
        public void SetBody(CharacterSpriteSet set)
        {
            if (set != null)
                Body.sprite = set.Bot;
        }
        public void SetHead(CharacterSpriteSet set)
        {
            if (set != null)
                Head.sprite = set.Bot;
        }
        public void SetShirt(CharacterSpriteSet set)
        {
            if (set != null)
                Shirt.sprite = set.Bot;
        }
        public void SetPants(CharacterSpriteSet set)
        {
            if (set != null)
            {
                Pants.sprite = set.Bot;
                Pants.gameObject.SetActive(set.Bot != null);
            }
        }
        public void SetHands(CharacterSpriteSet set)
        {
            if (set != null)
            {
                Hand_Left.sprite = set.Bot;
                Hand_Right.sprite = set.Bot;
            }
        }
    }
}