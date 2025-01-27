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
    public class CustomScenarioOptionsUiController : MonoBehaviour
    {
        [SerializeField] private GenericListDisplay scenariosListDisplay;
        [SerializeField] private CustomScenarioListDisplayElementController customScenarioDisplayElementPrefab;
        
        void Start()
        {
            scenariosListDisplay.DisplayList(BattleLoader.GetCustomScenarios(), customScenarioDisplayElementPrefab);
        }

        public void OnClickStartGameButton()
        {
            var toggles = scenariosListDisplay.GetComponentsInDirectChildren<Toggle>();
            var activeToggle = toggles.FirstOrDefault(t => t.isOn);

            if (activeToggle)
            {
                var selectedMap = activeToggle.GetComponent<CustomScenarioListDisplayElementController>().Data;
                StartScenario(selectedMap);
            }
        }

        private void StartScenario(GameplayScenario scenario)
        {
            BattleLoader.LoadBattle(scenario);
        }
    }
}
