using PlayerStats.Buffs.Core;
using UI.Notifications;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerStats.Buffs
{
    [CreateAssetMenu(fileName = "Insomnia", menuName = "Buffs/Insomnia")]
    public class Insomnia : Buff<Insomnia>
    {
        public override void Start()
        {
            base.Start();
            Notify.Show("I can't sleep.", Icon);
        }

        public override void TakeEffect()
        {
            Debug.Log($"{Name} taking effect.");
        }

        public override void TakeEffect(BuffArgs args)
        { }

        public override void End()
        {
            Debug.Log($"{Name} ended.");
        }
    }
}