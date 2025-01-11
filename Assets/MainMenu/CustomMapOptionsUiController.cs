using System;
using System.Collections.Generic;
using System.Linq;
using GlobalScripts;
using Maps;
using RKUnityToolkit.UIElements;
using RKUnityToolkit.UnityExtensions;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class CustomMapOptionsUiController : MonoBehaviour
    {
        [SerializeField] private GenericListDisplay mapsListDisplay;
        [SerializeField] private CustomMapListDisplayElement customMapDisplayElementPrefab;
        
        void Start()
        {
            mapsListDisplay.DisplayList(BattleLoader.GetCustomMaps(), customMapDisplayElementPrefab);
        }

        public void OnClickStartGameButton()
        {
            var toggles = mapsListDisplay.GetComponentsInDirectChildren<Toggle>();
            var activeToggle = toggles.FirstOrDefault(t => t.isOn);

            if (activeToggle)
            {
                var selectedMap = activeToggle.GetComponent<CustomMapListDisplayElement>().Data;
                StartCustomMapGame(selectedMap);
            }
        }

        private void StartCustomMapGame(GameplayMap gameplayMap)
        {
            BattleLoader.LoadBattle(gameplayMap);
        }
    }
}