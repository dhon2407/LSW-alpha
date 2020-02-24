using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace LSW.Helpers
{
    public static class EnumFunction
    {
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }
    }

    public static class UIEffects
    {
        public static void Shake(this Button button, float shakeDuration, float intensity)
        {
            Transform uiTransform = button.transform;
            var originalPosition = uiTransform.localPosition;

            StartShake(uiTransform,
                shakeDuration,
                intensity,
                originalPosition).
                    Start();
        }
        
        public static void Shake(this RectTransform rectTransform, float shakeDuration, float intensity)
        {
            Transform uiTransform = rectTransform.transform;
            var originalPosition = uiTransform.localPosition;

            StartShake(uiTransform,
                    shakeDuration,
                    intensity,
                    originalPosition).
                Start();
        }

        private static IEnumerator<float> StartShake(Transform transform, float shakeDuration, float intensity, Vector3 originalPosition)
        {
            while (shakeDuration > 0)
            {
                shakeDuration -= Time.deltaTime;
                transform.localPosition = originalPosition + (Random.insideUnitSphere * intensity);
                yield return 0f;
            }

            transform.localPosition = originalPosition;
        }
    }
    
    public static class PlaceholderFactoryExtensions
    {
        public static TValue Create<TValue>(this PlaceholderFactory<TValue> factory, Transform parent)
            where TValue : Component
        {
            var value = factory.Create();

            if (parent != null)
            {
                value.transform.SetParent(parent, false);
            }

            return value;
        }

        public static TValue Create<TParam1, TValue>(this PlaceholderFactory<TParam1, TValue> factory, TParam1 p1, Transform parent)
            where TValue : Component
        {
            var value = factory.Create(p1);

            if (parent != null)
            {
                value.transform.SetParent(parent, false);
            }

            return value;
        }

        // .. etc.
    }

    public static class GameObjectPosition
    {
        public static Vector2 Offset(this Vector3 position, float xOffset, float yOffset)
        {
            var newPosition = position;
            newPosition.x += xOffset;
            newPosition.y += yOffset;

            return newPosition;
        }
        
        public static Vector3 Offset(this Vector3 position, float xOffset, float yOffset, float zOffset)
        {
            var newPosition = position;
            newPosition.x += xOffset;
            newPosition.y += yOffset;
            newPosition.z += zOffset;

            return newPosition;
        }
        public static Vector3 Offset(this Vector3 position, Vector2 offset)
        {
            var newPosition = position;
            newPosition.x += offset.x;
            newPosition.y += offset.y;

            return newPosition;
        }
    }
}
