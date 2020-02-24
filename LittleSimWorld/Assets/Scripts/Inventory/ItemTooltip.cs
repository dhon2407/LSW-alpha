using LSW.Tooltip;
using TMPro;
using UnityEngine;

namespace InventorySystem
{
    [AddComponentMenu(menuName:"Tooltip/Item Tooltip")]
    public class ItemTooltip : Tooltip<ItemTooltip.Data>
    {
        [Header("Labels")]
        [SerializeField] private TextMeshProUGUI itemName = null;
        [SerializeField] private TextMeshProUGUI description = null;
        
        public override void SetData(Data data)
        {
            itemName.text = data.name;
            description.text = data.description;
        }
        
        public struct Data
        {
            public string name;
            public string description;
        }
    }
}