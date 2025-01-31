using System;
using System.Collections.Generic;
using System.Linq;
using BattleDataModel;
using GlobalScripts;
using Maps;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace BattleRunner.UI.PlayersInfoPanel
{
    public class PlayerInfoBoxController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ProceduralImage border;
        [SerializeField] private Image background;
        [SerializeField] private Image dieImage;
        [SerializeField] private GameObject winnerVisuals;
        [SerializeField] private TextMeshProUGUI text;

        private int _playerIndex;
        private GameplayMap _gameplayMap;
        private BattleRunnerController _battleRunnerController;
        private bool _pointerOver;
        private bool _largestRegionHighlighted;

        private bool LargestRegionShouldBeHighlighted => _pointerOver && !UserCueSequencer.CurrentlyProcessingCues && _battleRunnerController.IsHumanPlayersTurn;

        private void Update()
        {
            UpdateLargestRegionHighlights();
        }

        public void Initialize(int playerIndex, GameplayMap gameplayMap, BattleRunnerController battleRunnerController)
        {
            _playerIndex = playerIndex;
            _gameplayMap = gameplayMap;
            _battleRunnerController = battleRunnerController;
            RefreshData();
        }

        public void RefreshData()
        {
            var territories = _battleRunnerController.Battle.Map.GetTerritories(_playerIndex);
            int reinforcementCount = _battleRunnerController.Battle.Map.GetLargestContiguousGroupOfTerritories(_playerIndex).Count;
            bool isActivePlayer = _battleRunnerController.Battle.ActivePlayer.PlayerIndex == _playerIndex;
            
            dieImage.sprite = Resources.Load<Sprite>(Constants.GetThreeQuartersDieSpritesPathFromResources(_playerIndex));
            border.color = isActivePlayer ? Color.white : Constants.Colors[_playerIndex];
            background.color = isActivePlayer ? Constants.Colors[_playerIndex] : Color.white;
            text.text = territories.Sum(t => t.NumDice) + " | " + territories.Count + " | " + reinforcementCount;

            gameObject.SetActive(!_battleRunnerController.Battle.GetPlayer(_playerIndex).Eliminated);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _pointerOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _pointerOver = false;
        }
        
        private void UpdateLargestRegionHighlights()
        {
            if (LargestRegionShouldBeHighlighted && !_largestRegionHighlighted)
            {
                _largestRegionHighlighted = true;
                HashSet<MapNode> territories = _battleRunnerController.Battle.Map.GetLargestContiguousGroupOfTerritories(_playerIndex);
                foreach (MapNode territory in territories)
                {
                    var visualController = _gameplayMap.GetTerritoryVisualController(territory);
                    visualController.HighlightedToShowLargestContiguousGroupOfTerritories = true;
                    visualController.UpdateState();
                }
            }
            else if (!LargestRegionShouldBeHighlighted && _largestRegionHighlighted)
            {
                _largestRegionHighlighted = false;

                foreach (MapNode territory in _battleRunnerController.Battle.Map.Territories.Values)
                {
                    var visualController = _gameplayMap.GetTerritoryVisualController(territory);
                    visualController.HighlightedToShowLargestContiguousGroupOfTerritories = false;
                    visualController.UpdateState();
                }
            }
        }
    }
}
