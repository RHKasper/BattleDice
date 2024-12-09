using System;
using System.Collections.Generic;
using BattleDataModel;
using BattleTest.Scripts;
using UnityEngine;

namespace BattleTest.PlayersStatusPanel
{
    public class PlayersStatusPanelController : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private PlayerStatusBoxController playerStatusBoxPrefab;
        
        [Header("Scene Object References")]
        [SerializeField] private RectTransform playerStatusBoxesParent;
        [SerializeField] private BattleTester battleTester;

        private Dictionary<int, PlayerStatusBoxController> _playerStatusBoxes;

        private void Start()
        {
            battleTester.BattleInitialized += OnBattleInitialized;
        }

        private void OnBattleInitialized()
        {
            battleTester.Battle.StartingTerritoriesAssigned += OnStartingTerritoriesAssigned;
            battleTester.Battle.StartingReinforcementsAllocated += OnStartingReinforcementsAllocated;
            battleTester.Battle.TerritoryCaptured += OnTerritoryCaptured;
        }

        private void OnDestroy()
        {
            battleTester.Battle.StartingTerritoriesAssigned -= OnStartingTerritoriesAssigned;
            battleTester.Battle.StartingReinforcementsAllocated -= OnStartingReinforcementsAllocated;
            battleTester.Battle.TerritoryCaptured -= OnTerritoryCaptured;
        }

        private void OnStartingTerritoriesAssigned(object sender, BattleEvents.StartingTerritoriesAssignedArgs e)
        {
            // clear old boxes if necessary
            if (_playerStatusBoxes != null)
            {
                foreach (PlayerStatusBoxController box in _playerStatusBoxes.Values)
                {
                    Destroy(box.gameObject);
                }
            }
            
            _playerStatusBoxes = new Dictionary<int, PlayerStatusBoxController>();
            
            // Instantiate new boxes
            foreach (Player player in e.Battle.Players)
            {
                var box = Instantiate(playerStatusBoxPrefab, playerStatusBoxesParent);
                _playerStatusBoxes.Add(player.PlayerID, box);
            }
        }
        
        private void OnStartingReinforcementsAllocated(object sender, BattleEvents.StartingReinforcementsAllocatedArgs e)
        {
            foreach (Player player in e.Battle.Players)
            {
                _playerStatusBoxes[player.PlayerID].SetData(player.PlayerID, e.Battle.Map);    
            }
        }
        
        private void OnTerritoryCaptured(object sender, BattleEvents.TerritoryCapturedArgs args)
        {
            _playerStatusBoxes[args.PreviousOwnerPlayerId].SetData(args.PreviousOwnerPlayerId, battleTester.Battle.Map);
            _playerStatusBoxes[args.CapturedTerritory.OwnerPlayerId].SetData(args.CapturedTerritory.OwnerPlayerId, battleTester.Battle.Map);
        }
    }
}
