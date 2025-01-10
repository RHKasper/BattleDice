using System;
using MapMaker;
using RKUnityToolkit.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class CustomMapListDisplayElement : GenericListDisplay.ListItemController<CustomMap>
    {
        [SerializeField] private TextMeshProUGUI mapNameText;
        [SerializeField] private Toggle toggle;
        
        private void Start()
        {
            toggle.group = GetComponentInParent<ToggleGroup>();
        }
        
        protected override void OnDataSet(CustomMap data)
        {
            mapNameText.text = data.gameObject.name;
        }
    }
}
