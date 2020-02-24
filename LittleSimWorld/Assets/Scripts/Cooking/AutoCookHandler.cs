using UnityEngine;

namespace Cooking.Recipe
{
    public class AutoCookHandler : MonoBehaviour, ICookMode
    {
        [SerializeField] private UIPopUp popUp = null;

        private bool _isOpen = false;
        private IRecipeLoader _recipeLoader;

        public bool Active => _isOpen;

        public bool ToggleCooking()
        {
            _isOpen = !_isOpen;

            if (_isOpen)
                Open();
            else
                Close();

            return _isOpen;
        }

        public void Refresh()
        {
            _recipeLoader.FetchRecipes(CookingEntity.AutoActionText);
        }

        public void Open()
        {
            popUp.Open();
            _isOpen = true;
        }

        public void Close()
        {
            popUp.Close();
            _isOpen = false;
        }

        public int SlotRequiredLevel(int numberOfIngredients)
        {
            return 0;
        }

        private void Awake()
        {
            _recipeLoader = GetComponentInChildren<IRecipeLoader>();
        }
    }
}