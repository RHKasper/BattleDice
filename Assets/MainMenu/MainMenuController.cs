using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private MainMenuTweeningManager tweeningManager;
        [SerializeField] private MapsPanelController mapsPanelController;

        private void Awake()
        {
            tweeningManager.gameObject.SetActive(true);
            mapsPanelController.gameObject.SetActive(false);
        }
    }
}
