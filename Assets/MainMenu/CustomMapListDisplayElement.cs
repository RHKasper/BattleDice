using System;
using GlobalScripts;
using RKUnityToolkit.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class CustomMapListDisplayElement : GenericListDisplay.ListItemController<GameplayMap>
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
