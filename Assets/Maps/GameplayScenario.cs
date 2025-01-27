using BattleDataModel;
using UnityEngine;

namespace Maps
{
    public class GameplayScenario : GameplayMap
    {
        public int GetPlayerCount()
        {
            int highestPlayerIndex = -1;
            foreach (GameplayMapNodeDefinition nodeDefinition in GetNodeDefinitionsInOrder())
            {
                highestPlayerIndex = Mathf.Max(highestPlayerIndex, nodeDefinition.GetComponent<NodeStartStateDefinition>().ownerPlayerIndex);
            }

            return highestPlayerIndex + 1;
        }
        
        protected override MapNode CreateMapNode(GameplayMapNodeDefinition definition, int index)
        {
            var startStateDefinition = definition.GetComponent<NodeStartStateDefinition>();
            return new MapNode(index, startStateDefinition.ownerPlayerIndex, startStateDefinition.numDice);
        }
    }
}