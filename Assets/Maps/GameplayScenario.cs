using BattleDataModel;

namespace Maps
{
    public class GameplayScenario : GameplayMap
    {
        protected override MapNode CreateMapNode(GameplayMapNodeDefinition definition, int index)
        {
            var startStateDefinition = definition.GetComponent<NodeStartStateDefinition>();
            return new MapNode(index, startStateDefinition.ownerPlayerId, startStateDefinition.numDice);
        }
    }
}