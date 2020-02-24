using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSW.Tooltip
{
    [AddComponentMenu(menuName:"Tooltip/Simple Tooltip (SMALL)")]
    public class SimpleSmallTooltip : Tooltip<SimpleSmallTooltip.Data>
    {
        [SerializeField] private TextMeshProUGUI textMesh = null;
        [SerializeField] private float horizontalPadding = 3f;
        [SerializeField] private float verticalPadding = 3f;
        [SerializeField] private float maxWidth = 100f;
        
        private LayoutElement _layoutElement;

        public void Start()
        {
            _layoutElement = GetComponent<LayoutElement>();
            _layoutElement.minWidth = maxWidth;
        }

        public override void Show()
        {
            StartCoroutine(DelayShow());
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void SetData(Data data)
        {
            textMesh.text = data.text;
        }
        
        private IEnumerator DelayShow()
        {
            var actualWidth = Mathf.Clamp(textMesh.preferredWidth, 0, maxWidth);
            _layoutElement.minWidth = horizontalPadding + actualWidth;
            
            yield return new WaitForEndOfFrame();

            var actualHeight = Mathf.CeilToInt(textMesh.preferredHeight);
            _layoutElement.minHeight = verticalPadding + actualHeight;
            
            yield return null;
            
            base.Show();
        }

        public struct Data
        {
            public string text;
        }
    }
}