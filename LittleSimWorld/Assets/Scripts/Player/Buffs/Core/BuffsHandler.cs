using System;
using System.Collections.Generic;
using System.Linq;
using GameTime;
using InventorySystem;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace PlayerStats.Buffs.Core
{
    public class BuffsHandler : MonoBehaviour
    {
        public List<IBuff> Buffs => _buffs;
        public UnityEvent OnBuffChangeEvent { get; } = new UnityEvent();

        public void AddBuff<T>() where T : ScriptableObject, IBuff
        {
            AddNewBuff(_creator.Create<T>());
        }

        public bool HasBuff<T>() where T : IBuff
        {
            return _buffs.OfType<T>().Any();
        }

        public void RemoveBuff<T>() where T : IBuff
        {
            if (!HasBuff<T>()) return;

            var buffToRemove = GetBuff<T>();
            buffToRemove.End();
            _buffs.Remove(buffToRemove);
            
            OnBuffChangeEvent.Invoke();
        }
        
        public IBuff GetBuff<T>() where T: IBuff
        {
            return _buffs.FirstOrDefault(buff => buff.GetType() == typeof(T));
        }
        
        public void AddNewBuff(IBuff newBuff)
        {
            if (HasBuff(newBuff))
            {
                GetBuff(newBuff).Stack(newBuff);
            }
            else
            {
                _buffs.Add(newBuff);
                newBuff.Start();
            }

            OnBuffChangeEvent.Invoke();
        }
        
        private List<IBuff> _buffs = new List<IBuff>();
        private readonly BuffCreator _creator = new BuffCreator();
        
        private void Awake()
        {
            PlayerBuff.Initialize(this);
        }
        
        private void Update()
        {
            CheckBuffs();

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F6))
                AddRandomBuff();
#endif
        }

        public void AddRandomBuff()
        {
            var x = Random.Range(0, 6);
            switch (x)
            {
                case 0:
                    PlayerBuff.Add<Insomnia>();
                    break;
                case 2:
                    PlayerBuff.Add<Freedom>();
                    break;
                case 3:
                    PlayerBuff.Add<Motivated>();
                    break;
                case 4:
                    PlayerBuff.Add<Productive>();
                    break;
                default:
                    PlayerBuff.Add<Suppression>();
                    break;
            }
        }

        private void CheckBuffs()
        {
            var shouldUpdate = false;
            for (int i = 0; i < _buffs.Count; i++)
            {    
                if (_buffs[i].IsPermanent || !(_buffs[i].Remaining <= 0)) continue;

                _buffs[i].End();
                _buffs.RemoveAt(i);
                shouldUpdate = true;
            }

            if (shouldUpdate)
                OnBuffChangeEvent.Invoke();
        }

        private void OnDestroy()
        {
            PlayerBuff.Clear(this);
        }
        
        private bool HasBuff(IBuff newBuff)
        {
            return _buffs.Any(buff => buff.GetType() == newBuff.GetType());
        }

        private IBuff GetBuff(IBuff newBuff)
        {
            return _buffs.FirstOrDefault(buff => buff.GetType() == newBuff.GetType());
        }
    }
    
    public static class PlayerBuff
    {
        private static BuffsHandler _handler;
        
        public static bool Ready => _handler != null;
        public static UnityEvent OnBuffsChanged => _handler.OnBuffChangeEvent;

        public static void Initialize(BuffsHandler handler)
        {
            _handler = handler;
        }
        
        public static void Clear(BuffsHandler handler)
        {
            if (_handler.Equals(handler))
                _handler = null;
        }

        public static void Add(IBuff newBuff)
        {
            _handler.AddNewBuff(newBuff);
        }

        public static void Add<T>() where T : ScriptableObject, IBuff
        {
            _handler.AddBuff<T>();
        }

        public static void Remove<T>() where T : IBuff
        {
            _handler.RemoveBuff<T>();
        }

        public static bool HasBuff<T>() where T : IBuff
        {
            return _handler.HasBuff<T>();
        }

        public static IBuff GetBuff<T>() where T : IBuff
        {
            return _handler.GetBuff<T>();
        }
        
        public static List<IBuff> GetAll()
        {
            return _handler.Buffs;
        }

        public static void GetRandomBuff()
        {
            _handler.AddRandomBuff();
        }
    }
}