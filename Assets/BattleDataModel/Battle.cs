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
        public event EventHandler<BattleEvents.TerritoryCapturedArgs> TerritoryCaptured;
        public event EventHandler<BattleEvents.AttackFinishedArgs> AttackFinished;
        public event EventHandler<BattleEvents.PlayerEliminatedArgs> PlayerEliminated;
        public event EventHandler<BattleEvents.GameEndedArgs> GameEnded;
        public event EventHandler<BattleEvents.ReinforcementsAppliedArgs> ReinforcementsApplied;
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
                    playerNodes[player.PlayerIndex][Rng.Next(0, nodes.Count)].NumDice++;
                    // todo: trigger dice changed event?
                }
            }
            
            StartingReinforcementsAllocated?.Invoke(this, new BattleEvents.StartingReinforcementsAllocatedArgs(this));
        }

        public void Attack(MapNode attackingSpace, MapNode defendingSpace)
        {
            Debug.Log("node " + attackingSpace.NodeIndex + " attacks node " + defendingSpace.NodeIndex);

            List<int> attackingRoll = DiceRoller.RollDice(attackingSpace.NumDice, Rng);
            List<int> defendingRoll = DiceRoller.RollDice(defendingSpace.NumDice, Rng);
            int attackRollSum = attackingRoll.Sum();
            int defenseRollSum = defendingRoll.Sum();

            bool attackerWins = attackRollSum > defenseRollSum;
            string resultsString = attackRollSum + " vs " + defenseRollSum + " ([" + string.Join(", ", attackingRoll) + "] vs [" + string.Join(", ", defendingRoll) + "]";
            
            var attackFinishedEventArgs = new BattleEvents.AttackFinishedArgs(attackingSpace.OwnerPlayerIndex, defendingSpace.OwnerPlayerIndex, attackingSpace, defendingSpace, attackerWins);
            
            if (attackerWins)
            {
                Debug.Log("Attacker wins: " + resultsString);
                defendingSpace.NumDice = attackingSpace.NumDice - 1;
                attackingSpace.NumDice = 1;
                CaptureTerritory(defendingSpace, attackingSpace.OwnerPlayerIndex);
            }
            else
            {
                Debug.Log("Defender wins: " + resultsString);
                attackingSpace.NumDice = 1;
            }

            AttackFinished?.Invoke(this, attackFinishedEventArgs);
        }

        public void EndTurn()
        {
            int prevActivePlayerIndex = _activePlayerIndex;

            Reinforce(_activePlayerIndex);
            _activePlayerIndex = GetIndexOfNextNonEliminatedPlayer(_activePlayerIndex);
            TurnEnded?.Invoke(this, new(prevActivePlayerIndex, _activePlayerIndex));
        }

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
            List<MapNode> ownedNodesWithRoomForReinforcements = ownedNodes.Where(n => n.NumDice < Constants.MaxDiceInTerritory).ToList();

            Debug.Log("Adding " + reinforcementsCount + " reinforcements for player " + playerIndex);
            
            for (int i = 0; i < reinforcementsCount; i++)
            {
                if (ownedNodesWithRoomForReinforcements.Count == 0)
                {
                    HandleNoRoomForReinforcements(reinforcementsCount - i);
                    break;    
                }
                
                int randomTerritoryIndex = Rng.Next(0, ownedNodesWithRoomForReinforcements.Count);
                MapNode randomTerritory = ownedNodesWithRoomForReinforcements[randomTerritoryIndex];
                randomTerritory.NumDice++;
                
                if (randomTerritory.NumDice == Constants.MaxDiceInTerritory)
                {
                    ownedNodesWithRoomForReinforcements.RemoveAt(randomTerritoryIndex);
                }
            }
            
            ReinforcementsApplied?.Invoke(this, new BattleEvents.ReinforcementsAppliedArgs(_activePlayerIndex));
        }

        private void HandleNoRoomForReinforcements(int extraReinforcementsAmount)
        {
            Debug.Log("No room for " + extraReinforcementsAmount + " extra reinforcements");
        }

        private void CaptureTerritory(MapNode territory, int capturingPlayerIndex)
        {
            int previousOwnerPlayerIndex = territory.OwnerPlayerIndex;
            var eventArgs = new BattleEvents.TerritoryCapturedArgs(territory, territory.OwnerPlayerIndex);
            territory.OwnerPlayerIndex = capturingPlayerIndex;
            
            TerritoryCaptured?.Invoke(this, eventArgs);
            
            if (Map.GetNumTerritoriesOwnedByPlayer(previousOwnerPlayerIndex) == 0)
            {
                EliminatePlayer(previousOwnerPlayerIndex, capturingPlayerIndex);
                if (_players.Count(p => p.Eliminated == false) == 1)
                {
                    EndGame(capturingPlayerIndex, previousOwnerPlayerIndex);
                }
            }
        }

        private void EliminatePlayer(int eliminatedPlayerIndex, int eliminatingPlayerIndex)
        {
            _players[eliminatedPlayerIndex].Eliminated = true;
            PlayerEliminated?.Invoke(this, new BattleEvents.PlayerEliminatedArgs(eliminatedPlayerIndex, eliminatingPlayerIndex));
        }

        private void EndGame(int winningPlayerIndex, int lastOpponentStandingIndex)
        {
            // todo record game over? Might not need to since players' Eliminated variable contains that info
            GameEnded?.Invoke(this, new BattleEvents.GameEndedArgs(winningPlayerIndex, lastOpponentStandingIndex));
        }
    }
}