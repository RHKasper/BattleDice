using UnityEngine;

namespace BattleRunner
{
    public class BattleRunnerSoundsManager : MonoBehaviour
    {
        [SerializeField] private AudioSource attackSucceeded;
        [SerializeField] private AudioSource attackFailed;

        public void PlayAttackSucceededSound() => attackSucceeded.Play();
        public void PlayAttackFailedSound() => attackFailed.Play();
    }
}