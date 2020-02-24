namespace Utilities.Testing {
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Objects.Utilities;
    using PlayerStats;
    using UnityEngine;
    using UnityEngine.UI;

    public class InGameObjectUpgradeTesterObjectUI : MonoBehaviour {
        public TMPro.TextMeshProUGUI label;
        public Image image;
        public TMPro.TMP_Dropdown dropdown;

        UpgradeValidator validator;

        public void SetTarget(UpgradeValidator validator) {
            label.text = validator.ValidatorType.ToString();
            image.sprite = validator.CurrentObject.GetComponent<SpriteRenderer>().sprite;
            this.validator = validator;

            dropdown.ClearOptions();
            int i = 0;
            foreach (var item in validator.ObjectSettings) {
                var text = validator.ValidatorType.ToString() + " " + i++;
                var optionData = new TMPro.TMP_Dropdown.OptionData(text);
                dropdown.onValueChanged.AddListener(UpdateSelection);
                dropdown.options.Add(optionData);
            }
        }

        void UpdateSelection(int selectionID) {
            validator.ApplyUpgrade(validator.ObjectSettings[selectionID].Prefab);
            image.sprite = validator.CurrentObject.GetComponent<SpriteRenderer>().sprite;
        }

    }
}