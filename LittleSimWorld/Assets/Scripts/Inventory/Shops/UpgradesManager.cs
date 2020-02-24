namespace Objects {
    using System.Collections.Generic;
    using System.Linq;
    using Objects.Utilities;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public static class UpgradesManager {
        static List<UpgradeValidator> validators;
        static UpgradesManager() {
            InitializeValidators();
            SceneManager.activeSceneChanged += (a, b) => { };
        }
        static void InitializeValidators() {
            try {
                validators = Object.FindObjectsOfType<UpgradeValidator>().ToList();
            }
            catch { }
        }

        public static UpgradeValidator GetValidator(UpgradeType type) {
            var validator = validators.Find(x => x.ValidatorType == type);
            if (!validator) { throw new UnityException($"No validator of type {type} found."); }
            return validator;
        }

    }

}