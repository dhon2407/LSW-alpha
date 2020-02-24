using System;
using System.Collections;
using System.ComponentModel;
using PlayerStats.Buffs.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Buffs
{
    public class BuffItem : MonoBehaviour
    {
        [SerializeField] private Image icon = null;
        [FormerlySerializedAs("tensSeconds")]
        [SerializeField] private TextMeshProUGUI tensDigit = null;
        [FormerlySerializedAs("onesSeconds")]
        [SerializeField] private TextMeshProUGUI onesDigit = null;
        [SerializeField] private TextMeshProUGUI unit = null;


        public IBuff CurrentBuff => _buff;
        
        public BuffItem Initialize(IBuff buff)
        {
            gameObject.SetActive(true);
            _buff = buff;
            icon.sprite = buff.Icon;
            
            if (_buff.IsPermanent)ClearValues();
            
            return this;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            ClearValues();
            transform.SetAsLastSibling();
            
        }

        public void Cancel()
        {
            _buff.Cancel();
        }
        
        private IBuff _buff;
        private TimeUnit _maxUnit;
        private int _currentValue = 0;
        private int _lastValue = 0;
        private bool _fadingAnimating;

        private void Awake()
        {
            ClearValues();
        }

        private void ClearValues()
        {
            tensDigit.text = string.Empty;
            onesDigit.text = string.Empty;
            unit.text = string.Empty;
            _currentValue = 0;
            
            ActivateFading(false);
        }

        private void Update()
        {
            if (_buff == null || _buff.IsPermanent) return;

            UpdateValue();

            if (_lastValue != _currentValue)
            {
                unit.text = GetUnitText();
                tensDigit.text = (_currentValue / 10).ToString("0");
                onesDigit.text = (_currentValue % 10).ToString("0");

                _lastValue = _currentValue;
            }
            
            ActivateFading((_buff.Remaining / _buff.Duration) < 0.1f);
        }

        private string GetUnitText()
        {
            switch (_maxUnit)
            {
                case TimeUnit.Days: return "d";
                case TimeUnit.Hours: return "h";
                case TimeUnit.Minutes: return "m";
                case TimeUnit.Seconds: return "s";
                
                default:
                    return string.Empty;
            }
        }

        private void UpdateValue()
        {
            _currentValue = TimeSpan.FromSeconds(_buff.Remaining).Days;
            if (_currentValue >= 1)
            {
                _maxUnit = TimeUnit.Days;
                return;
            }

            _currentValue = TimeSpan.FromSeconds(_buff.Remaining).Hours;
            if (_currentValue >= 1)
            {
                _maxUnit = TimeUnit.Hours;
                return;
            }
            
            _currentValue = TimeSpan.FromSeconds(_buff.Remaining).Minutes;
            if (TimeSpan.FromSeconds(_buff.Remaining).Minutes >= 1)
            {
                _maxUnit = TimeUnit.Minutes;
                return;
            }
            
            _currentValue = TimeSpan.FromSeconds(_buff.Remaining).Seconds;
            _maxUnit = TimeUnit.Seconds;
        }
        
        private void ActivateFading(bool enable)
        {
            if (enable == _fadingAnimating) return;

            _fadingAnimating = enable;

            if (_fadingAnimating)
                StartCoroutine(StartFadingEffect());
            else
                StopCoroutine(StartFadingEffect());
        }

        private IEnumerator StartFadingEffect()
        {
            const float fadeOutOpacity = 0.4f;
            const float fadeInOpacity = 1f;
            const float timeStep = 0.5f;

            while (_fadingAnimating)
            {
                float timeLapse = 0;
                while (timeLapse < timeStep)
                {
                    var c = icon.color;
                    icon.color = new Color(c.r, c.g, c.b,
                        Mathf.Lerp(fadeInOpacity, fadeOutOpacity, timeLapse / timeStep));
                    yield return null;
                    timeLapse = Mathf.Clamp(timeLapse + GameTime.Time.deltaTime, 0, timeStep);
                }

                timeLapse = 0;
                while (icon.color.a < fadeInOpacity)
                {
                    var c = icon.color;
                    icon.color = new Color(c.r, c.g, c.b,
                        Mathf.Lerp(fadeOutOpacity, fadeInOpacity, timeLapse / timeStep));
                    yield return null;
                    timeLapse = Mathf.Clamp(timeLapse + GameTime.Time.deltaTime, 0, timeStep);
                }
            }
        }


        private enum TimeUnit
        {
            Days, Hours, Minutes, Seconds    
        }
        
    }
}