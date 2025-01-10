using System.Collections.Generic;
using System.Linq;
using MapMaker;
using RKUnityToolkit.UIElements;
using UnityEngine;

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

        private List<CustomMap> LoadCustomMaps()
        {
            return Resources.LoadAll<CustomMap>(CustomMap.CustomMapsResourcesFolder).ToList();
        }
    }
}
