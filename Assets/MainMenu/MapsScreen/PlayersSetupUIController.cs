using System;
using GlobalScripts;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.MapsScreen
{
    public class PlayersSetupUIController : MonoBehaviour
    {
        [SerializeField] private Button addPlayerButton;
        [SerializeField] private Button removePlayerButton;
        [SerializeField] private Image numPlayersImage;
        [SerializeField] private PlayerSetupRowController[] playerSetupRows;

        public int NumPlayers { get; private set; } = 2;

        private void Awake()
        {
            ShowNumber(NumPlayers);
        }

        public void OnClickAddPlayer()
        {
            NumPlayers++;
            ShowNumber(NumPlayers);
            // validate player count change
            // change player count
            // activate/deactive buttons
            
        }

        public void OnClickRemovePlayer()
        {
            NumPlayers--;
            ShowNumber(NumPlayers);
            // validate player count change
            // change player count
            // activate/deactive buttons
        }

        private void ShowNumber(int number)
        {
            numPlayersImage.sprite = NumberSpritesSo.Instance.GetSprite(number);
            for (int i = 0; i < playerSetupRows.Length; i++)
            {
                playerSetupRows[i].SetActiveWithTweening(i < number);
            }
        }
    }
}
