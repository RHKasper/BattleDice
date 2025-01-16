using System.Threading.Tasks;
using BattleDataModel;
using GlobalScripts;
using UnityEngine;

namespace BattleRunner.UI.RollDisplayPanel
{
    public class AttackRollsPanelController : MonoBehaviour
    {
        [SerializeField] private RollDisplayPanelController attackerRollDisplayPanel;
        [SerializeField] private RollDisplayPanelController defenderRollDisplayPanel;

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

        public async Task RunAttackRoll(int[] attackRoll, int attackingPlayerId)
        {
            await attackerRollDisplayPanel.ShowDiceRoll(attackRoll, attackingPlayerId);
        }

        public async Task RunDefenseRoll(int[] defenseRoll, int defendingPlayerId)
        {
            await defenderRollDisplayPanel.ShowDiceRoll(defenseRoll, defendingPlayerId);
        }
    }
}
