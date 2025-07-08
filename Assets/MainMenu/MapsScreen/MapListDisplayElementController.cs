using GlobalScripts;
using Maps;
using RKUnityToolkit.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainMenu.MapsScreen
{
    public class MapListDisplayElementController : GenericListDisplay.ListItemController<GameplayMap>, ISelectHandler
    {
        [SerializeField] private TextMeshProUGUI mapNameText;
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image lockImage;

        private MapsScreenController _owner;
        public bool IsToggledOn => toggle.isOn;

        private void Start()
        {
            toggle.group = GetComponentInParent<ToggleGroup>();
        }

        public void OnToggleValueChanged(bool value)
        {
            if (value)
            {
                _owner.OnMapToggleActivated(this);
            }
        }

        public void ToggleOn()
        {
            toggle.isOn = true;
        }
        
        protected override void OnDataSet(GameplayMap data)
        {
            var instance = ScenarioSequenceSo.Instance.ScenariosInOrder;
            mapNameText.text = data.MapName;
            lockImage.enabled = data is GameplayScenario scenario && !ScenarioSequenceSo.Instance.IsUnlocked(scenario);
            toggle.interactable = !lockImage.enabled;
        }

        public override void Init(object initData)
        {
            _owner = (MapsScreenController)initData;
        }

        public void OnSelect(BaseEventData eventData)
        {
            toggle.isOn = true;
        }
    }
}
