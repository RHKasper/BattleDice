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

        private void Awake()
        {
            battleTester.BattleInitialized += OnBattleInitialized;
        }

        private void OnBattleInitialized()
        {
            battleTester.Battle.StartingTerritoriesAssigned += OnStartingTerritoriesAssigned;
            battleTester.Battle.StartingReinforcementsAllocated += OnStartingReinforcementsAllocated;
            battleTester.Battle.AttackFinished += OnAttackFinished;
            battleTester.Battle.PlayerEliminated += OnPlayerEliminated;
            battleTester.Battle.GameEnded += OnGameEnded;
            battleTester.Battle.AppliedReinforcements += OnAppliedReinforcements;
            battleTester.Battle.TurnEnded += OnTurnEnded;
        }

        private void OnDestroy()
        {
            battleTester.Battle.StartingTerritoriesAssigned -= OnStartingTerritoriesAssigned;
            battleTester.Battle.StartingReinforcementsAllocated -= OnStartingReinforcementsAllocated;
            battleTester.Battle.AttackFinished -= OnAttackFinished;
            battleTester.Battle.PlayerEliminated -= OnPlayerEliminated;
            battleTester.Battle.GameEnded -= OnGameEnded;
            battleTester.Battle.AppliedReinforcements -= OnAppliedReinforcements;
            battleTester.Battle.TurnEnded -= OnTurnEnded;
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
                _playerStatusBoxes.Add(player.PlayerIndex, box);
            }
        }
        
        private void OnStartingReinforcementsAllocated(object sender, BattleEvents.StartingReinforcementsAllocatedArgs e)
        {
            foreach (Player player in e.Battle.Players)
            {
                _playerStatusBoxes[player.PlayerIndex].SetData(player.PlayerIndex, e.Battle.Map);
                _playerStatusBoxes[player.PlayerIndex].SetHighlightActive(player.PlayerIndex == e.Battle.ActivePlayer.PlayerIndex);
                _playerStatusBoxes[player.PlayerIndex].SetEliminatedVisualsActive(false);
                _playerStatusBoxes[player.PlayerIndex].SetWinnerVisualsActive(false);
                
            }
        }
        
        private void OnAttackFinished(object sender, BattleEvents.AttackFinishedArgs args)
        {
            _playerStatusBoxes[args.AttackingPlayerId].SetData(args.AttackingPlayerId, battleTester.Battle.Map);
            _playerStatusBoxes[args.DefendingPlayerId].SetData(args.DefendingPlayerId, battleTester.Battle.Map);
        }
        
        private void OnPlayerEliminated(object sender, BattleEvents.PlayerEliminatedArgs e)
        {
            _playerStatusBoxes[e.EliminatedPlayerIndex].SetEliminatedVisualsActive(true);
        }

        private void OnGameEnded(object sender, BattleEvents.GameEndedArgs e)
        {
            _playerStatusBoxes[e.WinningPlayerIndex].SetWinnerVisualsActive(true);
        }
        
        private void OnAppliedReinforcements(object sender, BattleEvents.AppliedReinforcementsArgs e)
        {
            _playerStatusBoxes[e.PlayerIndex].SetData(e.PlayerIndex, battleTester.Battle.Map);
        }
        
        private void OnTurnEnded(object sender, BattleEvents.TurnEndedArgs e)
        {
            _playerStatusBoxes[e.PrevActivePlayerIndex].SetHighlightActive(false);
            _playerStatusBoxes[e.NewActivePlayerIndex].SetHighlightActive(true);
        }
    }
}
