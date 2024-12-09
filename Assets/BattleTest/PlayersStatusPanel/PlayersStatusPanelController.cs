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
            battleTester.Battle.AttackFinished += OnAttackFinished;
            battleTester.Battle.ReinforcementsApplied += OnReinforcementsApplied;
        }

        private void OnDestroy()
        {
            battleTester.Battle.StartingTerritoriesAssigned -= OnStartingTerritoriesAssigned;
            battleTester.Battle.StartingReinforcementsAllocated -= OnStartingReinforcementsAllocated;
            battleTester.Battle.AttackFinished -= OnAttackFinished;
            battleTester.Battle.ReinforcementsApplied -= OnReinforcementsApplied;
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
        
        private void OnAttackFinished(object sender, BattleEvents.AttackFinishedArgs args)
        {
            _playerStatusBoxes[args.AttackingPlayerId].SetData(args.AttackingPlayerId, battleTester.Battle.Map);
            _playerStatusBoxes[args.DefendingPlayerId].SetData(args.DefendingPlayerId, battleTester.Battle.Map);
        }
        
        private void OnReinforcementsApplied(object sender, BattleEvents.ReinforcementsAppliedArgs e)
        {
            _playerStatusBoxes[e.PlayerId].SetData(e.PlayerId, battleTester.Battle.Map);
        }
    }
}
