using System;
using System.Collections.Generic;

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
        
        public class AttackSucceededArgs : EventArgs
        {
            public MapNode AttackingTerritory;
            public MapNode CapturedTerritory;

            public AttackSucceededArgs(MapNode attackingTerritory, MapNode capturedTerritory)
            {
                AttackingTerritory = attackingTerritory;
                CapturedTerritory = capturedTerritory;
            }
        }
        
        public class RollingAttackArgs : EventArgs
        {
            public int AttackingPlayerId;
            public int DefendingPlayerId;
            public MapNode AttackingTerritory;
            public MapNode DefendingTerritory;
            public int[] AttackRoll;
            public int[] DefenseRoll;

            public RollingAttackArgs(int attackingPlayerId, int defendingPlayerId, MapNode attackingTerritory, MapNode defendingTerritory, int[] attackRoll, int[] defenseRoll)
            {
                AttackingPlayerId = attackingPlayerId;
                DefendingPlayerId = defendingPlayerId;
                AttackingTerritory = attackingTerritory;
                DefendingTerritory = defendingTerritory;
                AttackRoll = attackRoll;
                DefenseRoll = defenseRoll;
            }
        }
        
        public class AttackFailedArgs : EventArgs
        {
            public int AttackingPlayerId;
            public int DefendingPlayerId;
            public MapNode AttackingTerritory;
            public MapNode DefendingTerritory;

            public AttackFailedArgs(int attackingPlayerId, int defendingPlayerId, MapNode attackingTerritory, MapNode defendingTerritory)
            {
                AttackingPlayerId = attackingPlayerId;
                DefendingPlayerId = defendingPlayerId;
                AttackingTerritory = attackingTerritory;
                DefendingTerritory = defendingTerritory;
            }
        }
        
        public class AttackFinishedArgs : EventArgs
        {
            public int AttackingPlayerId;
            public int DefendingPlayerId;
            public MapNode AttackingTerritory;
            public MapNode DefendingTerritory;
            public bool AttackerWon;

            public AttackFinishedArgs(int attackingPlayerId, int defendingPlayerId, MapNode attackingTerritory, MapNode defendingTerritory, bool attackerWon)
            {
                AttackingPlayerId = attackingPlayerId;
                DefendingPlayerId = defendingPlayerId;
                AttackingTerritory = attackingTerritory;
                DefendingTerritory = defendingTerritory;
                AttackerWon = attackerWon;
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
        
        public class ApplyingReinforcementsArgs : EventArgs
        {
            public readonly int PlayerIndex;
            public readonly int NumReinforcements;

            public ApplyingReinforcementsArgs(int playerIndex, int numReinforcements)
            {
                PlayerIndex = playerIndex;
                NumReinforcements = numReinforcements;
            }
        }
        
        public class AppliedReinforcementDieArgs : EventArgs
        {
            public readonly MapNode Territory;

            public AppliedReinforcementDieArgs(MapNode territory)
            {
                Territory = territory;
            }
        }
        
        public class AppliedReinforcementsArgs : EventArgs
        {
            public readonly int PlayerIndex;
            public readonly IEnumerable<MapNode> ReinforcedTerritoriesInOrder;

            public AppliedReinforcementsArgs(int playerIndex, IEnumerable<MapNode> reinforcedTerritoriesInOrder)
            {
                PlayerIndex = playerIndex;
                ReinforcedTerritoriesInOrder = reinforcedTerritoriesInOrder;
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