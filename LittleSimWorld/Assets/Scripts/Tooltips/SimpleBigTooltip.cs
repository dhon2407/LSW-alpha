using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSW.Tooltip
{
    [AddComponentMenu(menuName:"Tooltip/Simple Tooltip (BIG)")]
    public class SimpleBigTooltip : Tooltip<SimpleBigTooltip.Data>
    {
        [SerializeField] private TextMeshProUGUI textMesh = null;
        [SerializeField] private float horizontalPadding = 10f;
        
        private LayoutElement _layoutElement;

        public void Start()
        {
            _layoutElement = GetComponent<LayoutElement>();
            UpdateLayout();
        }

        public override void Show()
        {
            UpdateLayout();
            base.Show();
        }

        public override void SetData(Data data)
        {
            textMesh.text = data.text;
        }
        
        private void UpdateLayout()
        {
            _layoutElement.minWidth = horizontalPadding + textMesh.preferredWidth;
        }
        
        public struct Data
        {
            public string text;
        }
    }
}