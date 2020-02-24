using PlayerStats.Buffs.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PlayerStats.Buffs
{
    [CreateAssetMenu(fileName = "Freedom", menuName = "Buffs/Freedom", order = 0)]
    public class Freedom : Buff<Freedom>
    {
        [BoxGroup("Effect Details")]
        [LabelText("Increase mood gain by (%)")]
        [SerializeField]
        private float percentIncrease = 25f;

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
        }

        public override void End()
        { }
    }
}