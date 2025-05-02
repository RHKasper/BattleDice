using System;
using System.Collections.Generic;
using GlobalScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.MapsScreen
{
    public class PlayerSetupRowController : MonoBehaviour
    {
        [SerializeField] private Image numberImage;
        [SerializeField] private Image numberBackground;
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private int playerIndex;

        private void Awake()
        {
            numberImage.sprite = NumberSpritesSo.Instance.GetSprite(playerIndex + 1);
            numberBackground.color = Constants.Colors[playerIndex];
            dropdown.options = new List<TMP_Dropdown.OptionData>
            {
                new("Strategy 1"),
                new("Strategy 2")
            };
            dropdown.value = 0;
        }
    }
}
