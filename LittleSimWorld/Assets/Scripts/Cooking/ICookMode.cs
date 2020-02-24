namespace Cooking.Recipe
{
    public interface ICookMode
    {
        bool Active { get; }
        bool ToggleCooking();
        void Open();
        void Close();
        void Refresh();
    }
}