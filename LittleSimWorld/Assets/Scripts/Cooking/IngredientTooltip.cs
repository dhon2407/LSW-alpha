using InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cooking.Recipe
{
    public class IngredientTooltip : MonoBehaviour
    {
        [SerializeField] private Image icon = null;
        [SerializeField] private new TextMeshProUGUI name = null;

        private readonly Color locked = new Color(200f / 255f, 200f / 255f, 200f / 255f, 128f / 255f);
        private readonly Color textLocked = new Color(1f, 0f, 0f, 200f / 255f);
        private ItemCode itemCode;

        public ItemCode ItemCode => itemCode;

        public void SetItem(Item item)
        {
            icon.sprite = item.Data.icon;
            name.text = item.Data.name;
            itemCode = item.Code;
        }

        public void GreyOut(bool active)
        {
            icon.color = active ? locked : Color.white;
            name.color = active ? textLocked : Color.black;
        }
    }
}