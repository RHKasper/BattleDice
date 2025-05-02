using System;
using System.Collections.Generic;
using BattleDataModel;
using BattleDataModel.AiPlayerStrategies;
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
            NumPlayers = Mathf.Clamp(NumPlayers + 1, Constants.MinPlayers, Constants.MaxPlayers);
            ShowNumber(NumPlayers);
            SetPlayerCountButtonsInteractability();
        }

        public void OnClickRemovePlayer()
        {
            NumPlayers = Mathf.Clamp(NumPlayers - 1, Constants.MinPlayers, Constants.MaxPlayers);
            ShowNumber(NumPlayers);
            SetPlayerCountButtonsInteractability();
        }

        public List<Player> GetPlayers()
        {
            var players = new List<Player> { BattleLoader.GetHumanPlayer() };
            
            for (int i = 1; i < NumPlayers; i++)
            {
                AiStrat strat = playerSetupRows[i].GetAiStrategy();
                players.Add(new Player(i, AiStrategyHelpers.GetAiStrategyObject(strat)));    
            }

            return players;
        }

        private void SetPlayerCountButtonsInteractability()
        {
            addPlayerButton.interactable = NumPlayers < Constants.MaxPlayers;
            removePlayerButton.interactable = NumPlayers > Constants.MinPlayers;
        }
        
        private void ShowNumber(int number)
        {
            numPlayersImage.sprite = NumberSpritesSo.Instance.GetSprite(number);
            for (int i = 0; i < playerSetupRows.Length; i++)
            {
                _ = playerSetupRows[i].SetActiveWithTweening(i < number);
            }
        }
    }
}
