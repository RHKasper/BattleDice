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
                UserCueSequencer.EnqueueCueWithDelayAfter(_battleTester.GetMapNodeVisual(mapNode).ShowOwnership);
            }
        }

        public void OnStartingReinforcementsAllocated(object sender, BattleEvents.StartingReinforcementsAllocatedArgs e)
        {
            foreach (MapNode mapNode in e.Battle.Map.Nodes.Values)
            {
                UserCueSequencer.EnqueueCueWithDelayAfter(_battleTester.GetMapNodeVisual(mapNode).ShowNumDice);
            }
        }

        public void OnApplyingReinforcements(object sender, BattleEvents.ApplyingReinforcementsArgs e)
        {
            UserCueSequencer.EnqueueCueWithDelayAfter(() => Debug.Log("Adding " + e.NumReinforcements + " reinforcements for player " + e.PlayerIndex));
        }

        public void OnAppliedReinforcementDie(object sender, BattleEvents.AppliedReinforcementDieArgs e)
        {
            UserCueSequencer.EnqueueCueWithDelayAfter(_battleTester.GetMapNodeVisual(e.Territory).ShowNumDice);
        }
        
        public void OnAppliedReinforcements(object sender, BattleEvents.AppliedReinforcementsArgs e)
        {
            UserCueSequencer.EnqueueCueWithDelayAfter(() => Debug.Log("Applied reinforcements for " + e.PlayerIndex));
        }
        
        public void OnTerritoryCaptured(object sender, BattleEvents.TerritoryCapturedArgs e)
        {
            _battleTester.GetMapNodeVisual(e.CapturedTerritory).ShowOwnershipAndNumDice();
            
            // todo: update player status UI
        }

        public void OnAttackFinished(object sender, BattleEvents.AttackFinishedArgs e)
        {
            _battleTester.GetMapNodeVisual(e.AttackingTerritory).ShowNumDice();
        }
    }
}