using BattleDataModel;

namespace BattleTest.Scripts
{
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
                _battleTester.GetMapNodeVisual(mapNode).ShowOwnership();
            }
        }

        public void OnStartingReinforcementsAllocated(object sender, BattleEvents.StartingReinforcementsAllocatedArgs e)
        {
            foreach (MapNode mapNode in e.Battle.Map.Nodes.Values)
            {
                _battleTester.GetMapNodeVisual(mapNode).ShowNumDice();
            }
        }

        public void OnReinforcementsApplied(object sender, BattleEvents.ReinforcementsAppliedArgs e)
        {
            // todo: replace this with sequenced cues for each assigned die
            foreach (MapNode mapNode in _battleTester.Battle.Map.Nodes.Values)
            {
                _battleTester.GetMapNodeVisual(mapNode).ShowNumDice();
            }
            
            //_battleTester.GetMapNodeVisual(e.)
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