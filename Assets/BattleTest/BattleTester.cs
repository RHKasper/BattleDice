using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BattleDataModel;
using UnityEngine;

namespace BattleTest
{
    public class BattleTester : MonoBehaviour
    {
        [SerializeField] private MapNodeVisualController mapNodeVisualPrefab;
        [SerializeField] private MapEdgeVisualController mapEdgeVisualPrefab;
        [SerializeField] private Transform edgesParent;
        [SerializeField] private Transform nodesParent;

        private readonly Dictionary<MapNode, MapNodeVisualController> _instantiatedMapNodeVisuals = new();

        private Battle _battle;
    
        private async void Start()
        {
             //await InitializeBattleWith3Players();
        }

        public async void InitializeBattleWith3Players()
        {
            Player p0 = new Player(0);
            Player p1 = new Player(1);
            Player p2 = new Player(2);

            var players = new List<Player>{p0, p1, p2};
            var map = await InitMap();
            Battle battle = new Battle(map, players);
            
            _battle = battle;
        }

        public void RandomlyAssignTerritories() => _battle.RandomlyAssignTerritories();

        public void RandomlyAllocateStartingReinforcements() => _battle.RandomlyAllocateStartingReinforcements(3);

        private async Task<Map> InitMap()
        {
            var map = MapGenUtil.GenerateSimpleMapAsLine(7);

            foreach (var mapNode in map.Nodes.Values)
            {
                var mapNodeVisual = Instantiate(mapNodeVisualPrefab, nodesParent);
                mapNodeVisual.Initialize(mapNode);
                _instantiatedMapNodeVisuals.Add(mapNode, mapNodeVisual);
            }

            await Task.Delay(100);
            
            foreach (var mapNode in map.Nodes.Values)
            {
                foreach (MapNode adjacentNode in mapNode.GetAdjacentMapNodes())
                {
                    Instantiate(mapEdgeVisualPrefab, edgesParent).Initialize(_instantiatedMapNodeVisuals[mapNode], _instantiatedMapNodeVisuals[adjacentNode]);
                }
            }

            return map;
        }
    }
}
