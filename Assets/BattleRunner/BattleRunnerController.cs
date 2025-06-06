using System;
using BattleDataModel;
using BattleRunner.UI;
using BattleRunner.UI.ReinforcementDicePanel;
using BattleRunner.UI.RollDisplayPanel;
using GlobalScripts;
using Maps;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BattleRunner
{
    public class BattleRunnerController : MonoBehaviour
    {
        public event Action BattleInitialized;
        public event Action BattleStarted;
        public event Action SelectedTerritoryChanged;
        
        [SerializeField] private Canvas mapRoot;
        [SerializeField] private GraphicRaycaster mapCanvasGraphicRaycaster;
        [SerializeField] private GameObject gameOverUi;
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button endTurnButton;
        [SerializeField] private AttackRollsPanelController attackRollsPanel;
        [SerializeField] private ReinforcementDicePanelController reinforcementDicePanel;
        [SerializeField] private BattleRunnerSoundsManager soundsManager;
        
        private bool _battleStarted;
        private bool _battleEnded;
        
        public bool IsHumanPlayersTurn => !Battle.ActivePlayer.IsAiPlayer;
        public GameplayMap GameplayMap {get; private set;}
        public Battle Battle {get; private set;}
        public TerritoryVisualControllerBase SelectedTerritory {get; private set;}
        
        private void Start()
        {
            // Set initial UI states
            startGameButton.gameObject.SetActive(true);
            endTurnButton.gameObject.SetActive(false);
            attackRollsPanel.gameObject.SetActive(false);
            reinforcementDicePanel.gameObject.SetActive(false);
            gameOverUi.gameObject.SetActive(false);
            
            // todo: maybe remove this?
            BattleLoader.EnsureInitialized();
            
            // Instantiate selected map
            GameplayMap = Instantiate(BattleLoader.SelectedMapPrefab, mapRoot.transform);
            GameplayMap.RectTransform.anchoredPosition = Vector2.zero;
            
            // Construct data model battle
            Battle = BattleLoader.ConstructBattle(GameplayMap);
            Battle.RollingAttack += OnRollingAttack;
            Battle.PlayerEliminated += OnPlayerEliminated;
            Battle.ApplyingReinforcements += OnApplyingReinforcements;
            Battle.AppliedReinforcementDie += OnAppliedReinforcementDie;
            Battle.TurnEnded += OnTurnEnded;
            Battle.GameEnded += OnGameEnded;
            
            // Link nodes to node visuals
            var order = GameplayMap.GetNodeDefinitionsInOrder();
            for (var i = 0; i < order.Length; i++)
            {
                var nodeDefinition = order[i];
                nodeDefinition.GetComponent<TerritoryVisualControllerBase>().Initialize(this, Battle.Map.Territories[i]);
                Destroy(nodeDefinition);
            }
            
            // Assign territories and initial reinforcements
            if (GameplayMap is not GameplayScenario)
            {
                Battle.RandomlyAssignTerritories();
                Battle.RandomlyAllocateStartingDice(BattleLoader.StartingDicePercentage);
            }

            BattleInitialized?.Invoke();
            
            // for testing, auto start. In the future, player will start
            OnClickStartGame();
        }
        
        private void Update()
        {
            mapCanvasGraphicRaycaster.enabled = !UserCueSequencer.CurrentlyProcessingCues && _battleStarted && !_battleEnded;
            endTurnButton.interactable = !Battle.ActivePlayer.IsAiPlayer;
            
            if (Input.GetKeyDown(KeyCode.Escape) && !UserCueSequencer.CurrentlyProcessingCues)
            {
                DeselectTerritory();   
            }

            if (!UserCueSequencer.CurrentlyProcessingCues && Battle.ActivePlayer.IsAiPlayer)// && Input.GetKeyDown(KeyCode.N))
            {
                UserCueSequencer.EnqueueCueWithNoDelay(() =>
                {
                    Battle.ActivePlayer.AiStrategy!.PlayNextMove(Battle, Battle.ActivePlayer);
                }, "Play Next AI Move");
            }
        }

        public void SelectTerritory(TerritoryVisualControllerBase territory)
        {
            if (SelectedTerritory != null)
            {
                DeselectTerritory(true);
            }
            
            SelectedTerritory = territory;
            territory.UpdateState();
            soundsManager.PlaySelectSound();
            SelectedTerritoryChanged?.Invoke();
        }
        
        public void DeselectTerritory(bool dontPlaySound = false)
        {
            if (SelectedTerritory)
            {
                SelectedTerritory.UpdateState();
                SelectedTerritory = null;
                if (!dontPlaySound)
                {
                    soundsManager.PlayDeselectSound();
                }
                SelectedTerritoryChanged?.Invoke();
            }
        }
        
        public void ExecuteAttack(TerritoryVisualControllerBase attackingTerritory, TerritoryVisualControllerBase targetTerritory)
        {
            SetAllTerritoriesToNormalState();
            Battle.Attack(attackingTerritory.Territory, targetTerritory.Territory);
        }
        
        public void OnClickStartGame()
        {
            startGameButton.gameObject.SetActive(false);
            endTurnButton.gameObject.SetActive(true);
            _battleStarted = true;
            
            BattleStarted?.Invoke();
        }
        
        public void OnClickEndTurn()
        {
            Battle.EndTurn();
        }
        
        public void OnClickQuitToMenu()
        {
            // todo: add confirmation modal
            SceneManager.LoadScene("MainMenu");
            UserCueSequencer.ClearQueuedCues();
        }

#if DEBUG || UNITY_EDITOR
        public void OnClickEliminateNextOpponent()
        {
            Battle.EliminateNextAiPlayer();

            foreach (MapNode territory in Battle.Map.Territories.Values)
            {
                GameplayMap.GetTerritoryVisualController(territory).UpdateState();
            }
        }
#endif
        
        private void SetAllTerritoriesToNormalState()
        {
            foreach (MapNode territory in Battle.Map.Territories.Values)
            {
                var visualController = GameplayMap.GetTerritoryGameObject(territory).GetComponent<TerritoryVisualControllerBase>();
                visualController.OverrideState(TerritoryVisualControllerBase.State.Normal);
            }
        }
        
        private void OnPlayerEliminated(object sender, BattleEvents.PlayerEliminatedArgs e)
        {
            UserCueSequencer.EnqueueCueWithNoDelay(() =>
            {
                if (Battle.Players[e.EliminatedPlayerIndex].IsLocalHumanPlayer)
                {
                    soundsManager.PlayDefeatSound();
                }
                else
                {
                    soundsManager.PlayEnemyEliminatedSound();
                }
            }, "Play player elimination sounds (if necessary)");
        }
        
        private void OnRollingAttack(object sender, BattleEvents.RollingAttackArgs e)
        {
            var attackingTerritoryVisualController = GameplayMap.GetTerritoryVisualController(e.AttackingTerritory);
            var defendingTerritoryVisualController = GameplayMap.GetTerritoryVisualController(e.DefendingTerritory);
            
            UserCueSequencer.EnqueueCueWithNoDelay(() =>
            {
                attackingTerritoryVisualController.Attacking = true;
                defendingTerritoryVisualController.BeingAttacked = true;
                attackingTerritoryVisualController.UpdateState(false);
                defendingTerritoryVisualController.UpdateState(false);    
            }, "Show attacking and defending states");
            
            UserCueSequencer.EnqueueCueWithNoDelay(attackRollsPanel.ShowBlank, "Show attack roll display");
            UserCueSequencer.EnqueueCueWithDelayAfter(gameObject, async () => await attackRollsPanel.RunAttackRoll(e.AttackRoll, e.AttackingPlayerId), "show attacker roll");
            UserCueSequencer.Wait(UserCueSequencer.DefaultCueDelayMs * 3 * .001f);
            UserCueSequencer.EnqueueCueWithDelayAfter(gameObject, async () => await attackRollsPanel.RunDefenseRoll(e.DefenseRoll, e.DefendingPlayerId), "show defender roll");
            UserCueSequencer.Wait(UserCueSequencer.DefaultCueDelayMs * 3 * .001f);
            UserCueSequencer.EnqueueCueWithNoDelay(attackRollsPanel.Hide, "Hide attack roll display");
            
            UserCueSequencer.EnqueueCueWithNoDelay(() =>
            {
                DeselectTerritory(true);
                attackRollsPanel.Hide();
                
                attackingTerritoryVisualController.Attacking = false;
                attackingTerritoryVisualController.UpdateState();
                
                defendingTerritoryVisualController.BeingAttacked = false;
                defendingTerritoryVisualController.UpdateState();

                if (defendingTerritoryVisualController.Territory.OwnerPlayerIndex == e.AttackingPlayerId)
                {
                    if (Battle.Map.GetNumTerritoriesOwnedByPlayer(e.DefendingPlayerId) > 0)
                    {
                        soundsManager.PlayAttackSucceededSound();
                        if (Battle.ActivePlayer.IsAiPlayer)
                        {
                            UserCueSequencer.Wait(soundsManager.GetAttackSucceededLengthSeconds());
                        }
                    }
                }
                else
                {
                    soundsManager.PlayAttackFailedSound();
                    if (Battle.ActivePlayer.IsAiPlayer)
                    {
                        UserCueSequencer.Wait(soundsManager.GetAttackFailedLengthSeconds());
                    }
                }
                
            }, "Show Attack Results");
        }
        
        private void OnApplyingReinforcements(object sender, BattleEvents.ApplyingReinforcementsArgs e)
        {
            UserCueSequencer.EnqueueCueWithDelayAfter(() =>
            {
                reinforcementDicePanel.gameObject.SetActive(true);
                reinforcementDicePanel.ShowReinforcementDice(e.NumReinforcements, e.PlayerIndex);
            }, nameof(BattleRunnerController) + "." + nameof(OnApplyingReinforcements));
        }
        
        private void OnAppliedReinforcementDie(object sender, BattleEvents.AppliedReinforcementDieArgs e)
        {
            UserCueSequencer.EnqueueCueWithDelayAfter(() =>
            {
                var territoryVisualController = GameplayMap.GetTerritoryGameObject(e.Territory).GetComponent<TerritoryVisualControllerBase>();
                territoryVisualController.ShowNumDice(e.CurrentNumDice);
                soundsManager.PlayReinforcementDieSound();
                reinforcementDicePanel.ShowReinforcementDice(e.NumReinforcementsLeftUnapplied, e.Territory.OwnerPlayerIndex, true);
            }, nameof(BattleRunnerController) + "." + nameof(OnAppliedReinforcementDie));
        }

        private void OnTurnEnded(object sender, BattleEvents.TurnEndedArgs e)
        {
            UserCueSequencer.EnqueueCueWithNoDelay(() => reinforcementDicePanel.gameObject.SetActive(false), "Hide reinforcements panel");
            UserCueSequencer.EnqueueCueWithDelayBefore(() =>
            {
                if (Battle.Players[e.NewActivePlayerIndex].IsLocalHumanPlayer)
                {
                    soundsManager.PlayPlayerTurnStartSound();
                }
            }, "play local human player turn start sound");
        }
        
        private void OnGameEnded(object sender, BattleEvents.GameEndedArgs e)
        {
            UserCueSequencer.EnqueueCueWithDelayBefore(() =>
            {
                _battleEnded = true;
                endTurnButton.gameObject.SetActive(false);
                Debug.Log("Game won by " + e.WinningPlayerIndex);
                gameOverUi.SetActive(true);
                gameOverUi.GetComponentInChildren<TextMeshProUGUI>().SetText("Game won by " + e.WinningPlayerIndex);

                if (Battle.Players[e.WinningPlayerIndex].IsLocalHumanPlayer)
                {
                    soundsManager.PlayVictorySound();
                }
            }, "Handle Game End");
            
        }
    }
}
