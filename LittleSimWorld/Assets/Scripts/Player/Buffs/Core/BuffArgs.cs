using System;
using UnityEngine;

namespace PlayerStats.Buffs.Core
{
    public class BuffArgs
    {
        public static readonly BuffArgs Empty = new BuffArgs();
    }

    public class StatIncreaseArgs : BuffArgs
    {
        public float Amount { get; private set; }

        public StatIncreaseArgs(float amount)
        {
            Amount = Mathf.Abs(amount);
        }
        
        public void IncreaseValue(float percentage)
        {
            Amount += Amount * (percentage / 100f);
        }
    }
    
    public class StatDecreaseArgs : BuffArgs
    {
        public float Amount { get; private set; }
        private readonly float _sign;

        public StatDecreaseArgs(float amount)
        {
            _sign = Mathf.Sign(amount);
            Amount = Mathf.Abs(amount);
        }

        public void DecreaseValue(float percentage)
        {
            Amount -= Amount * (percentage / 100f);
            Amount *= _sign;
        }
    }
}