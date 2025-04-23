using System;
using System.Collections.Generic;
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
    public class MapOptionsUiController : MonoBehaviour
    {
        [SerializeField] private GenericListDisplay mapsListDisplay;
        [FormerlySerializedAs("customMapDisplayElementPrefab")] [SerializeField] private MapListDisplayElementController mapDisplayElementPrefab;
        
        void Start()
        {
            mapsListDisplay.DisplayList(BattleLoader.GetCustomMaps(), mapDisplayElementPrefab);
        }

        public void OnClickStartGameButton()
        {
            var toggles = mapsListDisplay.GetComponentsInDirectChildren<Toggle>();
            var activeToggle = toggles.FirstOrDefault(t => t.isOn);

            if (activeToggle)
            {
                var selectedMap = activeToggle.GetComponent<MapListDisplayElementController>().Data;
                StartCustomMapGame(selectedMap);
            }
        }

        private void StartCustomMapGame(GameplayMap gameplayMap)
        {
            BattleLoader.LoadBattle(gameplayMap);
        }
    }
}