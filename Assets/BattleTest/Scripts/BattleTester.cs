using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BattleDataModel;
using TMPro;
using UnityEngine;

namespace BattleTest.Scripts
{
    public class BattleTester : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool quickInit = true;
        
        [Header("Prefab References")]
        [SerializeField] private MapNodeVisualController mapNodeVisualPrefab;
        [SerializeField] private MapEdgeVisualController mapEdgeVisualPrefab;
        
        [Header("Scene Object References")]
        [SerializeField] private Transform edgesParent;
        [SerializeField] private Transform nodesParent;
        [SerializeField] private TextMeshProUGUI activePlayerText;

        private readonly Dictionary<MapNode, MapNodeVisualController> _instantiatedMapNodeVisuals = new();

        private Battle _battle;

        private async void Start()
        {
            if (quickInit)
            {
                await InitializeBattle(3);
                RandomlyAssignTerritories();
                RandomlyAllocateStartingReinforcements();
            }
        }

        private void Update()
        {
            activePlayerText.text = "Active Player: " + _battle.ActivePlayer.PlayerID;
        }

        public async void InitializeBattleWith3Players()
        {
            await InitializeBattle(3);
        }

        public void RandomlyAssignTerritories() => _battle.RandomlyAssignTerritories();

        public void RandomlyAllocateStartingReinforcements() => _battle.RandomlyAllocateStartingReinforcements(3);

        public void OnClickEndTurn()
        {
            _battle.EndTurn();
        }

        private async Task InitializeBattle(int playerCount)
        {
            var players = new List<Player>();
            for (int i = 0; i < playerCount; i++)
            {
                players.Add(new Player(i));    
            }
            
            var map = MapGenUtil.GenerateSimpleMapAsLine(7);
            _battle = new Battle(map, players, 190);
            
            await GenerateMapVisuals(map);
        }
        
        private async Task GenerateMapVisuals(Map map)
        {
            foreach (var mapNode in map.Nodes.Values)
            {
                var mapNodeVisual = Instantiate(mapNodeVisualPrefab, nodesParent);
                mapNodeVisual.Initialize(mapNode);
                _instantiatedMapNodeVisuals.Add(mapNode, mapNodeVisual);
            }

            await Task.Delay(100);
            
            foreach (var mapNode in map.Nodes.Values)
            {
                foreach (MapNode adjacentNode in mapNode.AdjacentNodes)
                {
                    Instantiate(mapEdgeVisualPrefab, edgesParent).Initialize(_instantiatedMapNodeVisuals[mapNode], _instantiatedMapNodeVisuals[adjacentNode]);
                }
            }
        }
    }
}
