using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BattleDataModel;
using BattleTest.MapVisuals;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace BattleTest.Scripts
{
    public class BattleTester : MonoBehaviour
    {
        public event Action BattleInitialized; 
        
        [Header("Settings")]
        [SerializeField] private bool quickInit = true;
        [SerializeField] private int playerCount = 3;
        [SerializeField] private int startingReinforcements = 3;
        
        [Header("Prefab References")]
        [SerializeField] private MapNodeVisualController mapNodeVisualPrefab;
        [SerializeField] private MapEdgeVisualController mapEdgeVisualPrefab;
        
        [Header("Scene Object References")]
        [SerializeField] private Transform edgesParent;
        [SerializeField] private Transform nodesParent;
        [SerializeField] private TextMeshProUGUI activePlayerText;

        private readonly Dictionary<MapNode, MapNodeVisualController> _instantiatedMapNodeVisuals = new();
        private MapNodeVisualController _selectedNode;
        
        internal Battle Battle { get; private set; }
        public MapNodeVisualController SelectedNode
        {
            get => _selectedNode;
            set
            {
                if (_selectedNode)
                {
                    _selectedNode.OnDeselected();
                }
                
                _selectedNode = value;

                if (_selectedNode)
                {
                    _selectedNode.OnSelected();
                }
            }
        }

        private async void Start()
        {
            if (quickInit)
            {
                await InitializeBattle(playerCount);
                BattleInitialized?.Invoke();
                
                Battle.RandomlyAssignTerritories();
                Battle.RandomlyAllocateStartingReinforcements(startingReinforcements);
            }
        }

        private void Update()
        {
            if (Battle != null)
            {
                activePlayerText.text = "Active Player: " + Battle.ActivePlayer.PlayerID;
            }
        }

        public void OnClickReshuffle()
        {
            Battle.RandomlyAssignTerritories();
            Battle.RandomlyAllocateStartingReinforcements(startingReinforcements);
        }
        
        public void OnClickEndTurn()
        {
            Battle.EndTurn();
        }

        private async Task InitializeBattle(int playerCount)
        {
            var players = new List<Player>();
            for (int i = 0; i < playerCount; i++)
            {
                players.Add(new Player(i));    
            }
            
            //var map = MapGenUtil.GenerateSimpleMapAsLine(20);
            var map = MapGenUtil.GenerateCircleMap(25);
            Battle = new Battle(map, players, 190);
            
            await GenerateMapVisuals(map);
        }
        
        private async Task GenerateMapVisuals(Map map)
        {
            await GenerateNodeVisualsInConcentricCircles(map);
            await Task.Delay(100);
            
            foreach (var mapNode in map.Nodes.Values)
            {
                foreach (MapNode adjacentNode in mapNode.AdjacentNodes)
                {
                    Instantiate(mapEdgeVisualPrefab, edgesParent).Initialize(_instantiatedMapNodeVisuals[mapNode], _instantiatedMapNodeVisuals[adjacentNode]);
                }
            }
        }

        private async Task GenerateNodeVisualsInConcentricCircles(Map map)
        {
            var rootNode = map.Nodes.Values.First();
            HashSet<MapNode> discoveredNodes = new HashSet<MapNode>();
            Queue<MapNode> frontierNodesPastCurrentDepth = new Queue<MapNode>();
            Queue<MapNode> frontierNodesAtCurrentDepth = new Queue<MapNode>();
            int depth = 0;
            
            frontierNodesAtCurrentDepth.Enqueue(rootNode);

            while (frontierNodesPastCurrentDepth.Any() || frontierNodesAtCurrentDepth.Any())
            {
                int numNodesAtCurrentDepth = frontierNodesAtCurrentDepth.Count;
                int numNodesProcessedAtCurrentDepth = 0;
                
                while (frontierNodesAtCurrentDepth.Any())
                {
                    var currentNode = frontierNodesAtCurrentDepth.Dequeue();                    
                    var currentMapNodeVisual = InstantiateMapNodeVisual(currentNode);

                    float radius = depth * 1.1f * mapNodeVisualPrefab.rectTransform.sizeDelta.x;
                    float irrationalOffset = .01f * (float)Math.E * depth;
                    float angleInRadians = irrationalOffset + 2 * Mathf.PI * (numNodesProcessedAtCurrentDepth - (depth-1)) / numNodesAtCurrentDepth;
                    currentMapNodeVisual.rectTransform.anchoredPosition = radius * new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

                    // manage traversal
                    discoveredNodes.Add(currentNode);
                    foreach (MapNode adjacentNode in currentNode.AdjacentNodes)
                    {
                        if (!discoveredNodes.Contains(adjacentNode) &&
                            !frontierNodesAtCurrentDepth.Contains(adjacentNode) &&
                            !frontierNodesPastCurrentDepth.Contains(adjacentNode))
                        {
                            frontierNodesPastCurrentDepth.Enqueue(adjacentNode);
                        }
                    }
                    
                    numNodesProcessedAtCurrentDepth++;
                }

                frontierNodesAtCurrentDepth = frontierNodesPastCurrentDepth;
                frontierNodesPastCurrentDepth = new Queue<MapNode>();
                depth++;
            }

            // foreach (var mapNode in map.Nodes.Values)
            // {
            //     GenerateMapNodeVisual(mapNode);
            // }   
        }

        private MapNodeVisualController InstantiateMapNodeVisual(MapNode mapNode)
        {
            var mapNodeVisual = Instantiate(mapNodeVisualPrefab, nodesParent);
            mapNodeVisual.Initialize(mapNode, this);
            _instantiatedMapNodeVisuals.Add(mapNode, mapNodeVisual);
            return mapNodeVisual;
        }
    }
}
