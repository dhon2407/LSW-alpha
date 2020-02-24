using UnityEngine;

namespace GameFile
{
    public class NoDataPreview : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Button backButton = null;

        public void SetBackButtonAction(UnityEngine.Events.UnityAction action)
        {
            backButton.onClick.AddListener(action);
        }
    }
}