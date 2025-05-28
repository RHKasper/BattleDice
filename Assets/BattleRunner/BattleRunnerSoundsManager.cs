using UnityEngine;

namespace BattleRunner
{
    public class BattleRunnerSoundsManager : MonoBehaviour
    {
        [SerializeField] private AudioSource dieRoll;
        [SerializeField] private AudioSource attackSucceeded;
        [SerializeField] private AudioSource attackFailed;
        [SerializeField] private AudioSource select;
        [SerializeField] private AudioSource deselect;
        [SerializeField] private AudioSource enemyEliminatedSound;

        public void PlayDieRollSound() => dieRoll.Play();
        public void PlayAttackSucceededSound() => attackSucceeded.Play();
        public void PlayAttackFailedSound() => attackFailed.Play();
        public void PlaySelectSound() => select.Play();
        public void PlayDeselectSound() => deselect.Play();
        public void PlayEnemyEliminatedSound() => enemyEliminatedSound.Play();

        public float GetDieRollLengthSeconds() => dieRoll.clip.length;
        public float GetAttackSucceededLengthSeconds() => attackSucceeded.clip.length;
        public float GetAttackFailedLengthSeconds() => attackFailed.clip.length;
        public float GetEnemyEliminatedLengthSeconds() => enemyEliminatedSound.clip.length;
    }
}