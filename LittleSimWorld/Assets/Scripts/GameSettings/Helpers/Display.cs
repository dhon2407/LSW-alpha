using UnityEngine;

namespace GameSettings.Helpers
{
    public static class Display
    {
        public static bool Same(this Resolution current, Resolution other)
        {
            return (current.height == other.height) && (current.width == other.width);
        }

        public static string NiceString(this FullScreenMode mode)
        {
            switch (mode)
            {
                case FullScreenMode.ExclusiveFullScreen:
                    return "Fullscreen";
                case FullScreenMode.FullScreenWindow:
                    return "Borderless Window";
                case FullScreenMode.MaximizedWindow:
                    return "Maximize Window";
                case FullScreenMode.Windowed:
                    return "Windowed";
                default:
                    return "Unknown screenmode.";
            }
        }
    }
}
