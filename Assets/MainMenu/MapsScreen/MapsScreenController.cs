using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BattleDataModel;
using GlobalScripts;
using Maps;
using RKUnityToolkit.Coroutines;
using RKUnityToolkit.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.MapsScreen
{
    public class MapsScreenController : MonoBehaviour
    {
        [SerializeField] private MapSource mapSource;
        [SerializeField] private GenericListDisplay mapsListDisplay;
        [SerializeField] private MapListDisplayElementController mapDisplayElementPrefab;
        [SerializeField] private ScrollRect mapsListScrollRect;
        [SerializeField] private TextMeshProUGUI selectedMapTitleText;
        [SerializeField] private TextMeshProUGUI selectedMapDescriptionText;
        [SerializeField] private Image selectedMapPreviewImage;
        [SerializeField] private PlayersSetupUIController playersSetupUIController;
        [SerializeField] private Slider startingDiceSlider;


        private float StartingDicePercentage => startingDiceSlider.value / startingDiceSlider.maxValue;
        
        void Start()
        {
            mapsListDisplay.DisplayList(GetMaps(), mapDisplayElementPrefab, this);
            mapsListDisplay.GetActiveListItems<MapListDisplayElementController>().First().ToggleOn();
        }

        private void OnEnable()
        {
            StartCoroutine(GenericCoroutines.DoNextFrame(() => mapsListScrollRect.content.anchoredPosition = Vector2.zero)); // fix scroll rect starting scrolled to the right
        }

        public void OnClickStartGameButton()
        {
            MapListDisplayElementController activeToggle = mapsListDisplay.GetActiveListItems<MapListDisplayElementController>().First(e => e.IsToggledOn);
            GameplayMap selectedMap = activeToggle.GetComponent<MapListDisplayElementController>().Data;
            StartBattle(selectedMap);
        }

        public void OnMapToggleActivated(MapListDisplayElementController mapToggle)
        {
            selectedMapTitleText.SetText(mapToggle.Data.MapName);
            selectedMapDescriptionText.SetText(mapToggle.Data.MapDescription);
            selectedMapPreviewImage.sprite = mapToggle.Data.MapPreviewImage;

            if (mapToggle.Data is GameplayScenario scenario)
            {
                playersSetupUIController.SetPlayers(scenario.GetPlayers());
            }
        }

        private void StartBattle(GameplayMap gameplayMap)
        {
            if (gameplayMap is GameplayScenario scenario)
            {
                BattleLoader.LoadCustomBattle(scenario, scenario.GetPlayers());
            }
            else
            {
                Debug.Log(StartingDicePercentage);
                BattleLoader.LoadCustomBattle(gameplayMap, playersSetupUIController.GetPlayers(), StartingDicePercentage);
            }
        }

        private List<GameplayMap> GetMaps()
        {
            return mapSource == MapSource.Maps
                ? BattleLoader.GetCustomMaps()
                : BattleLoader.GetCustomScenarios().Cast<GameplayMap>().ToList();
        }
        
        private enum MapSource
        {
            Maps, Scenarios
        }
    }
}