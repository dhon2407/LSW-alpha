using Cooking.Recipe;
using UnityEngine;

namespace Cooking
{
    public class ManualCookHandler : MonoBehaviour, ICookMode
    {
        [SerializeField] private CookList cookingList = null;
        
        [Space]
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
            _recipeLoader.FetchRecipes(CookingEntity.ManualActionText);
            cookingList.ClearList();
            cookingList.SetName(CookingEntity.DeviceName);
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

            cookingList.ReturnIngredients();
        }

        private void Awake()
        {
            _recipeLoader = GetComponentInChildren<IRecipeLoader>();
        }
    }
}