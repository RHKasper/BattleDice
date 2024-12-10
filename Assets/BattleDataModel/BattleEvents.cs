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
        
        public class PlayerEliminatedArgs : EventArgs
        {
            public int EliminatedPlayerIndex;
            public int EliminatingPlayerIndex;

            public PlayerEliminatedArgs(int eliminatedPlayerIndex, int eliminatingPlayerIndex)
            {
                EliminatedPlayerIndex = eliminatedPlayerIndex;
                EliminatingPlayerIndex = eliminatingPlayerIndex;
            }
        }
        
        public class GameEndedArgs : EventArgs
        {
            public int WinningPlayerIndex;
            public int LastOpponentStandingIndex;

            public GameEndedArgs(int winningPlayerIndex, int lastOpponentStandingIndex)
            {
                WinningPlayerIndex = winningPlayerIndex;
                LastOpponentStandingIndex = lastOpponentStandingIndex;
            }
        }
        
        // todo: add info here to nicely animate reinforcements
        public class ReinforcementsAppliedArgs : EventArgs
        {
            public int PlayerIndex;

            public ReinforcementsAppliedArgs(int playerIndex)
            {
                PlayerIndex = playerIndex;
            }
        }
        
        public class TurnEndedArgs : EventArgs
        {
            public int PrevActivePlayerIndex;
            public int NewActivePlayerIndex;

            public TurnEndedArgs(int prevActivePlayerIndex, int newActivePlayerIndex)
            {
                PrevActivePlayerIndex = prevActivePlayerIndex;
                NewActivePlayerIndex = newActivePlayerIndex;
            }
        }
    }
}