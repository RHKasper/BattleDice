using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace BattleDataModel
{
    public class Battle
    {        
        public event EventHandler<BattleEvents.StartingTerritoriesAssignedArgs> StartingTerritoriesAssigned;
        public event EventHandler<BattleEvents.StartingReinforcementsAllocatedArgs> StartingReinforcementsAllocated;
        public event EventHandler<BattleEvents.RollingAttackArgs> RollingAttack;
        public event EventHandler<BattleEvents.AttackSucceededArgs> AttackSucceeded;
        public event EventHandler<BattleEvents.AttackFailedArgs> AttackFailed;
        public event EventHandler<BattleEvents.PlayerEliminatedArgs> PlayerEliminated;
        public event EventHandler<BattleEvents.AttackFinishedArgs> AttackFinished;
        public event EventHandler<BattleEvents.GameEndedArgs> GameEnded;
        public event EventHandler<BattleEvents.ApplyingReinforcementsArgs> ApplyingReinforcements;
        public event EventHandler<BattleEvents.AppliedReinforcementDieArgs> AppliedReinforcementDie;
        public event EventHandler<BattleEvents.AppliedReinforcementsArgs> AppliedReinforcements;
        public event EventHandler<BattleEvents.TurnEndedArgs> TurnEnded;
        
        private readonly List<Player> _players;
        private int _activePlayerIndex = 0;
        
        public Map Map { get; }
        internal Random Rng { get; }
        public Player ActivePlayer => _players[_activePlayerIndex];
        public IReadOnlyList<Player> Players => _players;


        public Battle(Map map, List<Player> players, int randomSeed)
        {
            _players = players.ToList();
            Map = map;
            Rng = new Random(randomSeed);

            Debug.Assert(_players.TrueForAll(p => _players[p.PlayerIndex] == p));
        }

        /// <summary>
        /// Randomly assigns each <see cref="MapNode"/> of <see cref="Map"/> to <see cref="_players"/>, ensuring they're
        /// spread as evenly as possible, and sets each node's <see cref="MapNode.NumDice"/> to 1
        /// </summary>
        public void RandomlyAssignTerritories()
        {
            // todo: optimize all this to avoid so much memory allocation and inefficient CPU usage
            
            List<MapNode> shuffledNodes = Map.Nodes.Values.OrderBy(_ => Rng.Next()).ToList();
            List<Player> shuffledPlayers = _players.ToList();
            
            for (var i = 0; i < shuffledNodes.Count; i++)
            {
                var playerIndex = i % _players.Count;
                if (playerIndex == 0)
                {
                    // reshuffle
                    shuffledPlayers = shuffledPlayers.OrderBy(_ => Rng.Next()).ToList();
                }

                shuffledNodes[i].OwnerPlayerIndex = shuffledPlayers[playerIndex].PlayerIndex;
            }
            
            StartingTerritoriesAssigned?.Invoke(this, new BattleEvents.StartingTerritoriesAssignedArgs(this));
        }

        public void RandomlyAllocateStartingReinforcements(int startingReinforcements)
        {
            ValidateNumStartingReinforcements(startingReinforcements);
            if (Map.Nodes.Values.Any(n => n.OwnerPlayerIndex == -1))
            {
                throw new Exception("All map territories (nodes) must belong to a player before starting reinforcements are applied");
            }

            Dictionary<int, List<MapNode>> playerNodes = new();

            foreach (Player player in _players)
            {
                playerNodes[player.PlayerIndex] = new();
            }

            foreach (var mapNode in Map.Nodes.Values)
            {
                mapNode.NumDice = 1;
                playerNodes[mapNode.OwnerPlayerIndex].Add(mapNode);
            }

            foreach (Player player in _players)
            {
                var nodes = playerNodes[player.PlayerIndex];
                
                for (int i = 0; i < startingReinforcements; i++)
                {
                    MapNode territoryToReinforce = playerNodes[player.PlayerIndex][Rng.Next(0, nodes.Count)]; 
                    territoryToReinforce.NumDice++;
                    if (territoryToReinforce.NumDice == 8)
                    {
                        nodes.Remove(territoryToReinforce);
                    }
                    // todo: trigger dice changed event?
                }
            }
            
            StartingReinforcementsAllocated?.Invoke(this, new BattleEvents.StartingReinforcementsAllocatedArgs(this));
        }
        
        public void Attack(MapNode attackingTerritory, MapNode defendingTerritory)
        {
            Debug.Log("node " + attackingTerritory.NodeIndex + " attacks node " + defendingTerritory.NodeIndex);

            int attackingPlayerIndex = attackingTerritory.OwnerPlayerIndex;
            int defendingPlayerIndex = defendingTerritory.OwnerPlayerIndex;
            
            List<int> attackingRoll = DiceRoller.RollDice(attackingTerritory.NumDice, Rng);
            List<int> defendingRoll = DiceRoller.RollDice(defendingTerritory.NumDice, Rng);
            int attackRollSum = attackingRoll.Sum();
            int defenseRollSum = defendingRoll.Sum();

            bool attackerWins = attackRollSum > defenseRollSum;
            string resultsString = attackRollSum + " vs " + defenseRollSum + " ([" + string.Join(", ", attackingRoll) + "] vs [" + string.Join(", ", defendingRoll) + "]";

            var rollingAttackEventArgs = new BattleEvents.RollingAttackArgs(
                attackingPlayerIndex, defendingPlayerIndex, 
                attackingTerritory, defendingTerritory, 
                attackingRoll.ToArray(), defendingRoll.ToArray());
            RollingAttack?.Invoke(this, rollingAttackEventArgs);
            
            if (attackerWins)
            {
                Debug.Log("Attacker wins: " + resultsString);
                defendingTerritory.OwnerPlayerIndex = attackingPlayerIndex;
                defendingTerritory.NumDice = attackingTerritory.NumDice - 1;
                attackingTerritory.NumDice = 1;
                
                var eventArgs = new BattleEvents.AttackSucceededArgs(attackingTerritory, defendingTerritory);
                AttackSucceeded?.Invoke(this, eventArgs);
            
                if (Map.GetNumTerritoriesOwnedByPlayer(defendingPlayerIndex) == 0)
                {
                    EliminatePlayer(defendingPlayerIndex, attackingPlayerIndex);
                }
            }
            else
            {
                Debug.Log("Defender wins: " + resultsString);
                attackingTerritory.NumDice = 1;
                var attackFailedArgs = new BattleEvents.AttackFailedArgs(attackingPlayerIndex,
                    defendingPlayerIndex, attackingTerritory, defendingTerritory);
                AttackFailed?.Invoke(this, attackFailedArgs);
            }

            var attackFinishedEventArgs = new BattleEvents.AttackFinishedArgs(
                attackingPlayerIndex, defendingPlayerIndex,
                attackingTerritory, defendingTerritory, attackerWins);
            
            AttackFinished?.Invoke(this, attackFinishedEventArgs);
        }

        public void EndTurn()
        {
            int prevActivePlayerIndex = _activePlayerIndex;

            Reinforce(_activePlayerIndex);
            _activePlayerIndex = GetIndexOfNextNonEliminatedPlayer(_activePlayerIndex);
            TurnEnded?.Invoke(this, new(prevActivePlayerIndex, _activePlayerIndex));
        }

        public Player GetPlayer(int playerIndex)
        {
            return _players[playerIndex];
        }

#if DEBUG || UNITY_EDITOR
        public void EliminateNextAiPlayer()
        {
            var playerToEliminate = _players.FirstOrDefault(p => p.Eliminated == false && p.IsAiPlayer);
            if (playerToEliminate != null)
            {
                var playerToOwnTerritories = _players.First(p => !p.IsAiPlayer);
                foreach (MapNode territory in Map.GetTerritoriesOwnedByPlayer(playerToEliminate.PlayerIndex))
                {
                    territory.OwnerPlayerIndex = playerToOwnTerritories.PlayerIndex;
                }
                
                EliminatePlayer(playerToEliminate.PlayerIndex, playerToOwnTerritories.PlayerIndex);
            }
            else
            {
                throw new Exception("No AI Players remain");
            }
        }
#endif
        
        
        private int GetIndexOfNextNonEliminatedPlayer(int startIndex)
        {
            int indexToCheck = (startIndex + 1) % _players.Count;

            while (indexToCheck != startIndex)
            {
                if (!_players[indexToCheck].Eliminated)
                {
                    return indexToCheck;
                }
                
                indexToCheck = (indexToCheck + 1) % _players.Count;
            }

            throw new Exception("No other active players found");
        }
        
        /// <summary>
        /// Called at the end of a turn, this method randomly distributes a number of reinforcement dice equal to
        /// the size of the largest contiguous number of nodes owned by the given <see cref="playerIndex"/>
        /// </summary>
        private void Reinforce(int playerIndex)
        {
            HashSet<MapNode> largestContiguousTerritory = Map.GetLargestContiguousGroupOfTerritories(playerIndex);
            int reinforcementsCount = largestContiguousTerritory.Count;
            List<MapNode> ownedNodes = Map.GetTerritoriesOwnedByPlayer(playerIndex);
            List<MapNode> ownedNodesWithRoomForReinforcements = ownedNodes.Where(n => n.NumDice < DataModelConstants.MaxDiceInTerritory).ToList();

            ApplyingReinforcements?.Invoke(this, new BattleEvents.ApplyingReinforcementsArgs(playerIndex, reinforcementsCount));
            
            List<MapNode> orderedReinforcements = new();
            
            for (int i = 0; i < reinforcementsCount; i++)
            {
                if (ownedNodesWithRoomForReinforcements.Count == 0)
                {
                    HandleNoRoomForReinforcements(reinforcementsCount - i);
                    break;    
                }
                
                int randomTerritoryIndex = Rng.Next(0, ownedNodesWithRoomForReinforcements.Count);
                MapNode randomTerritory = ownedNodesWithRoomForReinforcements[randomTerritoryIndex];
                orderedReinforcements.Add(randomTerritory);
                randomTerritory.NumDice++;
                AppliedReinforcementDie?.Invoke(this, new BattleEvents.AppliedReinforcementDieArgs(randomTerritory, randomTerritory.NumDice, reinforcementsCount - i - 1));
                
                if (randomTerritory.NumDice == DataModelConstants.MaxDiceInTerritory)
                {
                    ownedNodesWithRoomForReinforcements.RemoveAt(randomTerritoryIndex);
                }
            }
            
            AppliedReinforcements?.Invoke(this, new BattleEvents.AppliedReinforcementsArgs(_activePlayerIndex, orderedReinforcements));
        }

        private void HandleNoRoomForReinforcements(int extraReinforcementsAmount)
        {
            Debug.Log("No room for " + extraReinforcementsAmount + " extra reinforcements");
        }
        
        
        private void EliminatePlayer(int eliminatedPlayerIndex, int eliminatingPlayerIndex)
        {
            _players[eliminatedPlayerIndex].Eliminated = true;
            PlayerEliminated?.Invoke(this, new BattleEvents.PlayerEliminatedArgs(eliminatedPlayerIndex, eliminatingPlayerIndex));
            if (_players.Count(p => p.Eliminated == false) == 1)
            {
                EndGame(eliminatingPlayerIndex, eliminatedPlayerIndex);
            }
        }

        private void EndGame(int winningPlayerIndex, int lastOpponentStandingIndex)
        {
            // todo record game over? Might not need to since players' Eliminated variable contains that info
            GameEnded?.Invoke(this, new BattleEvents.GameEndedArgs(winningPlayerIndex, lastOpponentStandingIndex));
        }
        
        private void ValidateNumStartingReinforcements(int startingReinforcements)
        {
            int leastNumTerritories = _players.Min(p => Map.GetNumTerritoriesOwnedByPlayer(p.PlayerIndex));
            int leastRoomForReinforcements = leastNumTerritories * (DataModelConstants.MaxDiceInTerritory - 1);

            if (startingReinforcements > leastRoomForReinforcements)
            {
                throw new Exception("Not enough room for starting reinforcements. " + leastNumTerritories + " only have room for " + leastRoomForReinforcements + " reinforcements, but " + startingReinforcements + " are being assigned");
            }
        }
    }
}