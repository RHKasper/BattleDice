using Maps;
using RKUnityToolkit.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MainMenu
{
    public class CustomScenarioListDisplayElementController : GenericListDisplay.ListItemController<GameplayScenario>
    {
        [SerializeField] private TextMeshProUGUI scenarioNameText;
        [SerializeField] private Toggle toggle;
        
        private void Start()
        {
            toggle.group = GetComponentInParent<ToggleGroup>();
        }

        protected override void OnDataSet(GameplayScenario data)
        {
            scenarioNameText.text = data.gameObject.name;
        }
    }
}
