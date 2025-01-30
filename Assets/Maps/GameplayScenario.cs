using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using BattleDataModel;
using BattleDataModel.AiPlayerStrategies;
using UnityEngine;

namespace Maps
{
    public class GameplayScenario : GameplayMap
    {
        [SerializedDictionary("Player ID, Strategy")] 
        [SerializeField] private SerializedDictionary<int, AiStrat> aiStrategies;

        private void OnValidate()
        {
            List<Player> players = GetPlayers();

            Debug.Assert(players.Count(p => !aiStrategies.ContainsKey(p.PlayerIndex)) == 1, "There must be exactly one non-ai player. Please add AI strategies in the " + nameof(GameplayScenario) + " component");

            for (var index = 0; index < players.Count; index++)
            {
                var player = players[index];
                Debug.Assert(player.PlayerIndex == index, "Player Index is incorrect for player " + player.PlayerIndex + ". Should be " + index);
            }
        }

        public List<Player> GetPlayers()
        {
            List<Player> players = new();
            
            foreach (GameplayMapNodeDefinition nodeDefinition in GetNodeDefinitionsInOrder())
            {
                int playerIndex = nodeDefinition.GetComponent<NodeStartStateDefinition>().ownerPlayerIndex;
                if (players.All(p => p.PlayerIndex != playerIndex))
                {
                    AiStrat? aiStrat = aiStrategies.ContainsKey(playerIndex) ? aiStrategies[playerIndex] : null;
                    players.Add(aiStrat == null ? new Player(playerIndex) : new Player(playerIndex, AiStrategyHelpers.GetAiStrategyObject(aiStrat.Value)));
                }
            }

            return players.OrderBy(p => p.PlayerIndex).ToList();
        }
        
        protected override MapNode CreateMapNode(GameplayMapNodeDefinition definition, int index)
        {
            var startStateDefinition = definition.GetComponent<NodeStartStateDefinition>();
            return new MapNode(index, startStateDefinition.ownerPlayerIndex, startStateDefinition.numDice);
        }
    }
}