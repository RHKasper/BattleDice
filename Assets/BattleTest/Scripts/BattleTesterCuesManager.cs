using BattleDataModel;
using UnityEngine;

namespace BattleTest.Scripts
{
    /// <summary>
    /// Responsible for managing all player cues for a battle
    /// </summary>
    public class BattleTesterCuesManager
    {
        private readonly BattleTester _battleTester;

        public BattleTesterCuesManager(BattleTester battleTester)
        {
            _battleTester = battleTester;
        }
        
        public void OnStartingTerritoriesAssigned(object sender, BattleEvents.StartingTerritoriesAssignedArgs e)
        {
            foreach (MapNode mapNode in e.Battle.Map.Nodes.Values)
            {
                UserCueSequencer.EnqueueCueWithDelayAfter("Show Node " + mapNode.NodeIndex + " starting owner", _battleTester.GetMapNodeVisual(mapNode).ShowOwnership);
            }
        }

        public void OnStartingReinforcementsAllocated(object sender, BattleEvents.StartingReinforcementsAllocatedArgs e)
        {
            foreach (MapNode mapNode in e.Battle.Map.Nodes.Values)
            {
                UserCueSequencer.EnqueueCueWithDelayAfter("Show Node " + mapNode.NodeIndex + " starting dice", _battleTester.GetMapNodeVisual(mapNode).ShowNumDice);
            }
        }

        public void OnApplyingReinforcements(object sender, BattleEvents.ApplyingReinforcementsArgs e)
        {
            UserCueSequencer.EnqueueCueWithDelayAfter("Logging", () => Debug.Log("Adding " + e.NumReinforcements + " reinforcements for player " + e.PlayerIndex));
        }

        public void OnAppliedReinforcementDie(object sender, BattleEvents.AppliedReinforcementDieArgs e)
        {
            UserCueSequencer.EnqueueCueWithDelayAfter("Show Node " + e.Territory.NodeIndex + " die added", _battleTester.GetMapNodeVisual(e.Territory).ShowNumDice);
        }
        
        public void OnAppliedReinforcements(object sender, BattleEvents.AppliedReinforcementsArgs e)
        {
            UserCueSequencer.EnqueueCueWithDelayAfter("Logging", () => Debug.Log("Applied reinforcements for " + e.PlayerIndex));
        }

        public void OnAttackSucceeded(object sender, BattleEvents.AttackSucceededArgs e)
        {
            UserCueSequencer.EnqueueCueWithDelayAfter("Showing attack success", () =>
            {
                _battleTester.GetMapNodeVisual(e.AttackingTerritory).ShowOwnershipAndNumDice();
                _battleTester.GetMapNodeVisual(e.CapturedTerritory).ShowOwnershipAndNumDice();
            });
        }
        
        public void OnAttackFailed(object sender, BattleEvents.AttackFailedArgs e)
        {
            UserCueSequencer.EnqueueCueWithDelayAfter("Showing attack failure", _battleTester.GetMapNodeVisual(e.AttackingTerritory).ShowNumDice);
        }
    }
}