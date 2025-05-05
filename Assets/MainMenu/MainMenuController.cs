using System;
using MainMenu.MapsScreen;
using UnityEngine;
using UnityEngine.Serialization;

namespace MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private MapsScreenController mapsScreenController;
        [SerializeField] private MapsScreenController scenariosScreenController;

        private void Awake()
        {
            scenariosScreenController.gameObject.SetActive(false);
            mapsScreenController.gameObject.SetActive(false);
        }
    }
}
