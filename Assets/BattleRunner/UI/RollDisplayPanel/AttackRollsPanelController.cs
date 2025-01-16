using System.Threading.Tasks;
using BattleDataModel;
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
            attackerRollDisplayPanel.gameObject.SetActive(false);
            defenderRollDisplayPanel.gameObject.SetActive(false);
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
            Debug.Log("OnRollingAttack");
            UserCueSequencer.EnqueueCueWithNoDelay(ShowBlank, "Show attack roll display");
            UserCueSequencer.EnqueueCueWithDelayAfter(gameObject, async () => await attackerRollDisplayPanel.ShowDiceRoll(e.AttackRoll, e.AttackingPlayerId), "show attacker roll");
            UserCueSequencer.EnqueueCueWithDelayAfter(gameObject, async () => await defenderRollDisplayPanel.ShowDiceRoll(e.DefenseRoll, e.DefendingPlayerId), "show defender roll");
            // todo: cue up the territory ownership change over
            UserCueSequencer.Wait(.5f);
            UserCueSequencer.EnqueueCueWithDelayAfter(gameObject, async () =>
            {
                Hide();
                await Task.Yield();
            }, "hide attack roll displays");
        }
    }
}
