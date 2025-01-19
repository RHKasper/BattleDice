using BattleDataModel;
using Maps;

namespace BattleRunner
{
    public static class BattleRunnerExtensions
    {
        public static TerritoryVisualControllerBase GetTerritoryVisualController(this GameplayMap gameplayMap, MapNode territory)
        {
            return gameplayMap.GetTerritoryGameObject(territory).GetComponent<TerritoryVisualControllerBase>();
        }
    }
}