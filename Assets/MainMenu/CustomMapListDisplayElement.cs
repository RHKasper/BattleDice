using MapMaker;
using RKUnityToolkit.UIElements;
using TMPro;
using UnityEngine;

namespace MainMenu
{
    public class CustomMapListDisplayElement : GenericListDisplay.ListItemController<CustomMap>
    {
        [SerializeField] private TextMeshProUGUI mapNameText;
        
        protected override void OnDataSet(CustomMap data)
        {
            mapNameText.text = data.gameObject.name;
        }
    }
}
