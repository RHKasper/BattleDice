using System;
using System.Collections.Generic;
using System.Linq;
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
            if (scenariosInOrder.Any(s => s.MapName == scenario.MapName))
            {
                var index = scenariosInOrder.IndexOf(scenario);
                return index == 0 || HasBeenBeaten(scenariosInOrder[index - 1]);
            }
            else
            {
                throw new Exception("Unknown scenario: " + scenario.MapName);
            }

            return true;
        }

        public static bool HasBeenBeaten(GameplayScenario scenario)
        {
            bool hasBeenBeaten = PlayerPrefs.GetInt(GetScenarioPlayerPrefsKey(scenario)) == 1;
            Debug.Log("Scenario: " + scenario.MapName + " has been beaten: " + hasBeenBeaten);
            return hasBeenBeaten;
        }

        public static void TrackScenarioBeaten(GameplayScenario scenario)
        {
            Debug.Log("Tracking scenario beaten: " + scenario.MapName);
            PlayerPrefs.SetInt(GetScenarioPlayerPrefsKey(scenario), 1);
        }
        
        private static string GetScenarioPlayerPrefsKey(GameplayScenario scenario) => scenario.MapName;
    }
}