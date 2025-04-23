using System.Linq;
using GlobalScripts;
using Maps;
using RKUnityToolkit.UIElements;
using RKUnityToolkit.UnityExtensions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MainMenu
{
    public class ScenarioOptionsUiController : MonoBehaviour
    {
        [SerializeField] private GenericListDisplay scenariosListDisplay;
        [FormerlySerializedAs("customScenarioDisplayElementPrefab")] [SerializeField] private ScenarioListDisplayElementController scenarioDisplayElementPrefab;
        
        void Start()
        {
            scenariosListDisplay.DisplayList(BattleLoader.GetCustomScenarios(), scenarioDisplayElementPrefab);
        }

        public void OnClickStartGameButton()
        {
            var toggles = scenariosListDisplay.GetComponentsInDirectChildren<Toggle>();
            var activeToggle = toggles.FirstOrDefault(t => t.isOn);

            if (activeToggle)
            {
                var selectedMap = activeToggle.GetComponent<ScenarioListDisplayElementController>().Data;
                StartScenario(selectedMap);
            }
        }

        private void StartScenario(GameplayScenario scenario)
        {
            BattleLoader.LoadBattle(scenario);
        }
    }
}
