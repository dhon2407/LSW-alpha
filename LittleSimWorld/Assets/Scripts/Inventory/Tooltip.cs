using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace InventorySystem
{
    public class Tooltip : MonoBehaviour
    {
        private static Tooltip instance;
            
        [SerializeField]
        private UIPopUp PopUpScript = null;
        [SerializeField]
        new private TextMeshProUGUI name = null;
        [SerializeField]
        private TextMeshProUGUI description = null;

        public float MinWidth = 0.1f;
        public float MaxWidth = 0.92f;
        public float MinHeight = 0.2f;
        public float MaxHeight = 1;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            ItemToolTip.Initialize(this);
        }

        private void LateUpdate()
        {
            transform.position = new Vector2(Mathf.Clamp(Input.mousePosition.x, Screen.width * MinWidth, Screen.width * MaxWidth),
                Mathf.Clamp(Input.mousePosition.y, Screen.height * MinHeight, Screen.height * MaxHeight));
        }

        public void ShowTooltip(Item item)
        {
            ShowTooltip(item.name, item.Data.Description);
        }

        public void ShowTooltip(string name, string description)
        {
            transform.position = new Vector2(Mathf.Clamp(Input.mousePosition.x, Screen.width * MinWidth, Screen.width * MaxWidth),
                Mathf.Clamp(Input.mousePosition.y, Screen.height * MinHeight, Screen.height * MaxHeight));

            this.name.text = name;
            this.description.text = description;

            PopUpScript.Open();
        }

        public void HideTooltip()
        {
            PopUpScript.Close();
        }
    }

    public static class ItemToolTip
    {
        private static Tooltip tooltip = null;

        public static bool Ready => tooltip != null;

        public static void Initialize(Tooltip currentTooltip)
        {
            tooltip = currentTooltip;
        }

        public static void Show(string name, string description)
        {
            if (Ready)
                tooltip.ShowTooltip(name, description);
        }

        public static void Hide()
        {
            if (Ready)
                tooltip.HideTooltip();
        }
    }
}