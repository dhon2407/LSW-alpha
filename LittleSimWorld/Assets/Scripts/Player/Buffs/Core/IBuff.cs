using UnityEngine;
using UnityEngine.Events;

namespace PlayerStats.Buffs.Core
{
    public interface IBuff
    {
        string Name { get; }
        string Description { get; }
        Sprite Icon { get; }
        float Duration { get; }
        float MaxDuration { get; }
        float Remaining { get; }
        
        bool IsDebuff { get; }
        bool IsPermanent { get; }

        void Start();
        void TakeEffect();
        void TakeEffect(BuffArgs args);
        void End();
        void Cancel();
        void Stack(IBuff self);
    }
}