using System;
using BattleDataModel;
using BattleTest.Scripts;
using UnityEngine;

namespace BattleTest.UI.RollDisplayPanel
{
    public class AttackRollsPanelController : MonoBehaviour
    {
        [SerializeField] private BattleTester battleTester; 
        [SerializeField] private RollDisplayPanelController attackerRollDisplayPanel;
        [SerializeField] private RollDisplayPanelController defenderRollDisplayPanel;

        private void Awake()
        {
            battleTester.BattleInitialized += OnBattleInitialized;
        }

        private void OnBattleInitialized()
        {
            battleTester.Battle.AttackFinished += OnAttackFinished;
        }

        private void OnAttackFinished(object sender, BattleEvents.AttackFinishedArgs e)
        {
            UserCueSequencer.EnqueueCueWithDelayAfter(gameObject, async () => await attackerRollDisplayPanel.ShowDiceRoll(e.AttackRoll));
        }
    }
}
