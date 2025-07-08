using System.Collections.Generic;
using Maps;
using RKUnityToolkit.ScriptableObjects;
using UnityEngine;

namespace GlobalScripts
{
    [CreateAssetMenu]
    [SoResourcesPath(ResourcesPath = "ScriptableObjects/ScenarioSequenceSo")]
    public class ScenarioSequenceSo : SoSingleton<ScenarioSequenceSo>
    {
        [SerializeField] private List<GameplayScenario> scenariosInOrder;
        public IReadOnlyList<GameplayScenario> ScenariosInOrder => scenariosInOrder.AsReadOnly();

        public bool IsUnlocked(GameplayScenario scenario)
        {
            if (scenariosInOrder.Contains(scenario))
            {
                var index = scenariosInOrder.IndexOf(scenario);
                return index == 0 || HasBeenBeaten(scenariosInOrder[index - 1]);
            }

            return true;
        }

        public static bool HasBeenBeaten(GameplayScenario scenario)
        {
            return PlayerPrefs.GetInt(GetScenarioPlayerPrefsKey(scenario)) == 1;
        }

        public static void TrackScenarioBeaten(GameplayScenario scenario)
        {
            PlayerPrefs.SetInt(GetScenarioPlayerPrefsKey(scenario), 1);
        }
        
        private static string GetScenarioPlayerPrefsKey(GameplayScenario scenario) => scenario.GetInstanceID().ToString();
    }
}