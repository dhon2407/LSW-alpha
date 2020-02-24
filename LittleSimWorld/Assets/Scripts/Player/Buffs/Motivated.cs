using PlayerStats.Buffs.Core;
using Sirenix.OdinInspector;
using UI.Notifications;
using UnityEngine;

namespace PlayerStats.Buffs
{
    [CreateAssetMenu(fileName = "Motivated", menuName = "Buffs/Motivated", order = 0)]
    public class Motivated : Buff<Motivated>
    {
        [BoxGroup("Effect Details")]
        [LabelText("Increase money gain after working by (%)")]
        [SerializeField]
        private float percentIncrease = 15f;
        public override void TakeEffect()
        { }

        public override void TakeEffect(BuffArgs args)
        {
            if (args.GetType() == typeof(StatIncreaseArgs))
                UpdateStat((StatIncreaseArgs) args);
        }

        private void UpdateStat(StatIncreaseArgs args)
        {
            args.IncreaseValue(percentIncrease);
            Notify.Show($"You received additional money for a great job!", Icon);
        }

        public override void End()
        { }
    }
}