using System;
using System.Collections;
using System.Collections.Generic;
using PlayerStats.Buffs.Core;
using UnityEngine;

namespace UI.Buffs
{
    public class BuffsDisplayHandler : MonoBehaviour
    {
        [SerializeField] private BuffItem buffItem = null;
        
        private List<BuffItem> _buffs = new List<BuffItem>();

        private void Awake()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            while (!PlayerBuff.Ready)
                yield return null;

            PlayerBuff.OnBuffsChanged.AddListener(OnBuffsChanged);
        }

        private void OnBuffsChanged()
        {
            var currentBuffs = PlayerBuff.GetAll();
            
            for (int i = 0; i < currentBuffs.Count; i++)
            {
                if (_buffs.Count < i + 1)
                    _buffs.Add(Instantiate(buffItem, transform).Initialize(currentBuffs[i]));
                else
                    _buffs[i].Initialize(currentBuffs[i]);
            }

            if (_buffs.Count > currentBuffs.Count)
                for (int i = currentBuffs.Count; i < _buffs.Count; i++)
                    _buffs[i].Disable();
        }
    }
}