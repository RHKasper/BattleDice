using System;
using System.Collections.Generic;
using System.Linq;
using MapMaker;
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
            mapsListDisplay.DisplayList(LoadCustomMaps(), customMapDisplayElementPrefab);
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

        private List<CustomMap> LoadCustomMaps()
        {
            return Resources.LoadAll<CustomMap>(CustomMap.CustomMapsResourcesFolder).ToList();
        }

        private void StartCustomMapGame(CustomMap customMap)
        {
            throw new NotImplementedException($"Todo: Start game from Custom Map: \"{customMap.gameObject.name}\"");
        }
    }
}