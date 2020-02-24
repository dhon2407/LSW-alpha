using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStats.Buffs.Core
{
    public class BuffCreator
    {
        private const string ResourceFolder = "Buffs/";
        private readonly Dictionary<Type, string> _availableBuffs = new Dictionary<Type, string>
        {
            {typeof(Insomnia), $"{ResourceFolder}{nameof(Insomnia)}"},
            {typeof(DeepSleep), $"{ResourceFolder}{nameof(DeepSleep)}"},
            {typeof(Freedom), $"{ResourceFolder}{nameof(Freedom)}"},
            {typeof(Motivated), $"{ResourceFolder}{nameof(Motivated)}"},
            {typeof(Productive), $"{ResourceFolder}{nameof(Productive)}"},
            {typeof(Suppression), $"{ResourceFolder}{nameof(Suppression)}"},
        };
        
        
        
        public IBuff Create<T>() where T : ScriptableObject, IBuff
        {
            if (_availableBuffs.ContainsKey(typeof(T)))
                return Resources.Load(_availableBuffs[typeof(T)]) as IBuff;

            Debug.LogWarning($"No buff type of {typeof(T)} defined.");
            return null;
        }
    }
}