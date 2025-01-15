using System.Threading.Tasks;
using BattleDataModel;
using BattleTest.UI.RollDisplayPanel;
using GlobalScripts;
using UnityEngine;

namespace BattleRunner.UI.RollDisplayPanel
{
    public class AttackRollsPanelController : MonoBehaviour
    {
        [SerializeField] private BattleRunnerController battleRunnerController; 
        [SerializeField] private RollDisplayPanelController attackerRollDisplayPanel;
        [SerializeField] private RollDisplayPanelController defenderRollDisplayPanel;

        private void Awake()
        {
            battleRunnerController.BattleInitialized += OnBattleInitialized;
        }

        public void ShowBlank()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private void OnBattleInitialized()
        {
            battleRunnerController.Battle.RollingAttack += OnRollingAttack;
        }

        private void OnRollingAttack(object sender, BattleEvents.RollingAttackArgs e)
        {
            UserCueSequencer.EnqueueCueWithNoDelay(ShowBlank, "Show attack roll display");
            UserCueSequencer.EnqueueCueWithDelayAfter(gameObject, async () => await attackerRollDisplayPanel.ShowDiceRoll(e.AttackRoll, e.AttackingPlayerId), "show attacker roll");
            UserCueSequencer.EnqueueCueWithDelayAfter(gameObject, async () => await defenderRollDisplayPanel.ShowDiceRoll(e.DefenseRoll, e.DefendingPlayerId), "show defender roll");
            UserCueSequencer.Wait(.3f);
            UserCueSequencer.EnqueueCueWithDelayAfter(gameObject, async () =>
            {
                Hide();
                await Task.Yield();
            }, "hide attack roll displays");
        }
    }
}
