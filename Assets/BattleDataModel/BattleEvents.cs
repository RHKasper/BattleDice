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
    }
}