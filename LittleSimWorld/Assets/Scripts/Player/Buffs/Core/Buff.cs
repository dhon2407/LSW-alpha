using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlayerStats.Buffs.Core
{
    public abstract class Buff<T> : ScriptableObject, IBuff
    {
        [OnInspectorGUI("DrawIcon",append:false)]
        [SerializeField] private Sprite icon = null;
        [SerializeField] private string buffName = "New buff";
        [SerializeField] private string description = "Buff";
        [SerializeField] private bool isDebuff = false;
        
        [BoxGroup("Effect duration")]
        [SerializeField] private float duration = 5f;
        [BoxGroup("Effect duration")]
        [SerializeField] private float maxDuration = 5f;

        [SerializeField] private bool permanent = false;

        public string Name => buffName;
        public string Description => description;
        public Sprite Icon => icon;
        public float Duration => duration;
        public float MaxDuration => maxDuration;
        public bool IsDebuff => isDebuff;
        public bool IsPermanent => permanent;
        public float Remaining => _remainingTime;

        public abstract void TakeEffect();
        public abstract void TakeEffect(BuffArgs args);
        public abstract void End();

        public virtual void Cancel()
        {
            if (IsDebuff || IsPermanent) return;
            
            Timing.KillCoroutines(nameof(StartBuff));
            _remainingTime = 0;
        }

        public virtual void Start()
        {
            Timing.RunCoroutine(StartBuff(), nameof(StartBuff));
        }

        public void Stack(IBuff self)
        {
            if (self is T)
                _remainingTime = Mathf.Clamp(_remainingTime + duration, 0, maxDuration);
        }

        private float _remainingTime = 0;
        
        private IEnumerator<float> StartBuff()
        {
            if (permanent) yield break;
            
            _remainingTime = duration;
            while (_remainingTime > 0)
            {
                _remainingTime = Mathf.Clamp(_remainingTime - GameTime.Time.deltaTime, 0, float.MaxValue);
                yield return Timing.WaitForOneFrame;
            }
            
            End();
        }
        
        private void OnValidate()
        {
            duration = Mathf.Clamp(duration, 0, maxDuration);
            maxDuration = Mathf.Clamp(maxDuration, 0, float.MaxValue);

            if (permanent)
                duration = maxDuration = 0;
        }

        #if UNITY_EDITOR
        private void DrawIcon()
        {
            if (icon == null) return;
            
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label(AssetPreview.GetAssetPreview(icon));
            GUILayout.EndVertical();
            
        }
        #endif
    }
}