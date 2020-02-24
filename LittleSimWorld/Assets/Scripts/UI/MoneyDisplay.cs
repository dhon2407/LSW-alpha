using System;
using System.Collections;
using PlayerStats;
using TMPro;
using UnityEngine;

namespace UI
{
    public class MoneyDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI money = null;
        [SerializeField] private TextMeshProUGUI moneyChange = null;

        private Vector2 _changeInitialPosition;
        private Vector2 _targetPosition;

        private float _moneyValue;
        private float _lastChangeValue;

        private void Awake()
        {
            StartCoroutine(Initialize());
            _changeInitialPosition = moneyChange.transform.localPosition;
            _targetPosition = money.transform.localPosition;
        }

        private IEnumerator Initialize()
        {
            while (!Stats.Ready || !Stats.Initialized)
                yield return null;

            _moneyValue = Stats.Money;
            money.text = $"£{_moneyValue:0}";

            Stats.OnMoneyChange.AddListener(UpdateMoney);
        }

        private void UpdateMoney(float value)
        {
            if (Math.Abs(value - _moneyValue) < 0.01) return;
            
            StopAllCoroutines();
            StartCoroutine(UpdateChange(_moneyValue, value));

            _moneyValue = value;
        }

        private IEnumerator UpdateChange(float currentValue, float newValue)
        {
            moneyChange.gameObject.SetActive(true);
            moneyChange.transform.localPosition = _changeInitialPosition;
            
            var changeValue = newValue - currentValue;

            _lastChangeValue += changeValue;

            if (_lastChangeValue.Equals(0f))
            {
                moneyChange.gameObject.SetActive(false);
                yield break;
            }

            moneyChange.fontSize = money.fontSize;
            moneyChange.text = (_lastChangeValue > 0 ? "+" : "-") + Mathf.Abs(_lastChangeValue).ToString("0.0");
            moneyChange.color = _lastChangeValue > 0 ? Color.white : Color.red;
            
            yield return new WaitForSeconds(2f);

            var timeLapse = 0f;
            var timeToMove = 0.5f;
            
            while (timeLapse < timeToMove)
            {
                timeLapse += GameTime.Time.deltaTime;
                moneyChange.transform.localPosition =
                    Vector2.Lerp(_changeInitialPosition, _targetPosition, timeLapse / timeToMove);
                yield return null;
            }

            moneyChange.transform.localPosition = _targetPosition;
            moneyChange.gameObject.SetActive(false);
            StartCoroutine(AnimateMoneyUpdate(currentValue, _moneyValue));
            _lastChangeValue = 0;
        }

        private IEnumerator AnimateMoneyUpdate(float currentValue, float targetValue)
        {
            var timeLapse = 0f;
            var countAnimationDuration = 0.5f;

            while (timeLapse < countAnimationDuration)
            {
                timeLapse += GameTime.Time.deltaTime;
                money.text = $"£{Mathf.Lerp(currentValue, targetValue, timeLapse/countAnimationDuration):0}";
                yield return null;
            }
            
            money.text = $"£{targetValue:0}";
        }
    }
}