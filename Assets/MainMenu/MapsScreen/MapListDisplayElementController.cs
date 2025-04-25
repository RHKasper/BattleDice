using System;
using GlobalScripts;
using Maps;
using RKUnityToolkit.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class MapListDisplayElementController : GenericListDisplay.ListItemController<GameplayMap>
    {
        [SerializeField] private TextMeshProUGUI mapNameText;
        [SerializeField] private Toggle toggle;
        
        private void Start()
        {
            toggle.group = GetComponentInParent<ToggleGroup>();
        }
        
        protected override void OnDataSet(GameplayMap data)
        {
            mapNameText.text = data.gameObject.name;
        }
    }
}
