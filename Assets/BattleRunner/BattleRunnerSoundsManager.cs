using UnityEngine;

namespace BattleRunner
{
    public class BattleRunnerSoundsManager : MonoBehaviour
    {
        [Header("Dice")]
        [SerializeField] private AudioSource dieRoll;
        [SerializeField] private AudioSource reinforcementDie;
        
        [Header("Attacks")]
        [SerializeField] private AudioSource attackSucceeded;
        [SerializeField] private AudioSource attackFailed;
        
        [Header("Selection")]
        [SerializeField] private AudioSource select;
        [SerializeField] private AudioSource deselect;
        
        [Header("Game Events")]
        [SerializeField] private AudioSource playerTurnStartSound;
        [SerializeField] private AudioSource enemyEliminatedSound;
        [SerializeField] private AudioSource victorySound;
        [SerializeField] private AudioSource defeatSound;

        public void PlayDieRollSound() => dieRoll.Play();
        public void PlayReinforcementDieSound() => reinforcementDie.Play();
        public void PlayAttackSucceededSound() => attackSucceeded.Play();
        public void PlayAttackFailedSound() => attackFailed.Play();
        public void PlaySelectSound() => select.Play();
        public void PlayDeselectSound() => deselect.Play();
        public void PlayPlayerTurnStartSound() => playerTurnStartSound.Play();
        public void PlayEnemyEliminatedSound() => enemyEliminatedSound.Play();
        public void PlayVictorySound() => victorySound.Play();
        public void PlayDefeatSound() => defeatSound.Play();

        public float GetDieRollLengthSeconds() => dieRoll.clip.length;
        public float GetAttackSucceededLengthSeconds() => attackSucceeded.clip.length;
        public float GetAttackFailedLengthSeconds() => attackFailed.clip.length;
        public float GetEnemyEliminatedLengthSeconds() => enemyEliminatedSound.clip.length;
    }
}