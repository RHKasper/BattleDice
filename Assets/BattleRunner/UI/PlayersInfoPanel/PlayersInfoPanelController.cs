using System.Collections.Generic;
using BattleDataModel;
using GlobalScripts;
using UnityEngine;

namespace BattleRunner.UI.PlayersInfoPanel
{
    public class PlayersInfoPanelController : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private PlayerInfoBoxController playerInfoBoxPrefab;
        
        [Header("Scene Object References")]
        [SerializeField] private RectTransform playerInfoBoxesParent;
        [SerializeField] private BattleRunnerController battleRunnerController;

        private Dictionary<int, PlayerInfoBoxController> _playerInfoBoxes;

        private void Awake()
        {
            battleRunnerController.BattleInitialized += OnBattleInitialized;
        }

        private void OnBattleInitialized()
        {
            Initialize();
            
            battleRunnerController.Battle.AttackFinished += OnAttackFinished;
            battleRunnerController.Battle.PlayerEliminated += OnPlayerEliminated;
            battleRunnerController.Battle.GameEnded += OnGameEnded;
            battleRunnerController.Battle.TurnEnded += OnTurnEnded;
        }

        private void Initialize()
        {
            _playerInfoBoxes = new Dictionary<int, PlayerInfoBoxController>();
            
            // Instantiate new boxes
            foreach (Player player in battleRunnerController.Battle.Players)
            {
                var box = Instantiate(playerInfoBoxPrefab, playerInfoBoxesParent);
                box.Initialize(player.PlayerIndex, battleRunnerController.GameplayMap, battleRunnerController.Battle);
                _playerInfoBoxes.Add(player.PlayerIndex, box);
            }
        }
        
        private void OnAttackFinished(object sender, BattleEvents.AttackFinishedArgs args)
        {
            UserCueSequencer.EnqueueCueWithDelayAfter(() =>
            {
                _playerInfoBoxes[args.AttackingPlayerId].RefreshData();
                _playerInfoBoxes[args.DefendingPlayerId].RefreshData();
            }, "Show attack results in Player Status Panels");
        }
        
        private void OnPlayerEliminated(object sender, BattleEvents.PlayerEliminatedArgs e)
        {
            UserCueSequencer.EnqueueCueWithDelayAfter(() => _playerInfoBoxes[e.EliminatedPlayerIndex].RefreshData(), "Show player elimination");
        }

        private void OnGameEnded(object sender, BattleEvents.GameEndedArgs e)
        {
            UserCueSequencer.EnqueueCueWithDelayAfter(() => _playerInfoBoxes[e.WinningPlayerIndex].RefreshData(), "Show winner visuals");
        }
        
        private void OnTurnEnded(object sender, BattleEvents.TurnEndedArgs e)
        {
            UserCueSequencer.EnqueueCueWithDelayAfter(() =>
            {
                _playerInfoBoxes[e.PrevActivePlayerIndex].RefreshData();
                _playerInfoBoxes[e.NewActivePlayerIndex].RefreshData();
            }, "Show active player changed");
        }
    }
}
