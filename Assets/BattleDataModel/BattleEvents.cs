using System;

namespace BattleDataModel
{
    public static class BattleEvents
    {
        public class StartingTerritoriesAssignedArgs : EventArgs
        {
            public Battle Battle;

            public StartingTerritoriesAssignedArgs(Battle battle)
            {
                Battle = battle;
            }
        }
        
        public class StartingReinforcementsAllocatedArgs : EventArgs
        {
            public Battle Battle;

            public StartingReinforcementsAllocatedArgs(Battle battle)
            {
                Battle = battle;
            }
        }
        
        public class TerritoryCapturedArgs : EventArgs
        {
            public MapNode CapturedTerritory;
            public int PreviousOwnerPlayerId;

            public TerritoryCapturedArgs(MapNode capturedTerritory, int previousOwnerPlayerId)
            {
                CapturedTerritory = capturedTerritory;
                PreviousOwnerPlayerId = previousOwnerPlayerId;
            }
        }
        
        public class AttackFinishedArgs : EventArgs
        {
            public int AttackingPlayerId;
            public int DefendingPlayerId;
            public MapNode AttackingTerritory;
            public MapNode DefendingTerritory;
            public bool AttackSuccessful;

            public AttackFinishedArgs(int attackingPlayerId, int defendingPlayerId, MapNode attackingTerritory, MapNode defendingTerritory, bool attackSuccessful)
            {
                AttackingPlayerId = attackingPlayerId;
                DefendingPlayerId = defendingPlayerId;
                AttackingTerritory = attackingTerritory;
                DefendingTerritory = defendingTerritory;
                AttackSuccessful = attackSuccessful;
            }
        }
        
        // todo: add info here to nicely animate reinforcements
        public class ReinforcementsAppliedArgs : EventArgs
        {
            public int PlayerId;

            public ReinforcementsAppliedArgs(int playerId)
            {
                PlayerId = playerId;
            }
        }
    }
}