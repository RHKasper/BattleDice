using System;
using MainMenu.MapsScreen;
using UnityEngine;
using UnityEngine.Serialization;

namespace MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private MainMenuTweeningManager tweeningManager;
        [SerializeField] private MapsScreenController mapsScreenController;

        private void Awake()
        {
            tweeningManager.gameObject.SetActive(true);
            mapsScreenController.gameObject.SetActive(false);
        }
    }
}
