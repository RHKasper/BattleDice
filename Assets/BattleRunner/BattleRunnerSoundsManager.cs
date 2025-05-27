using UnityEngine;

namespace BattleRunner
{
    public class BattleRunnerSoundsManager : MonoBehaviour
    {
        [SerializeField] private AudioSource attackSucceeded;
        [SerializeField] private AudioSource attackFailed;
        [SerializeField] private AudioSource select;
        [SerializeField] private AudioSource deselect;

        public void PlayAttackSucceededSound() => attackSucceeded.Play();
        public void PlayAttackFailedSound() => attackFailed.Play();
        public void PlaySelectSound() => select.Play();
        public void PlayDeselectSound() => deselect.Play();
    }
}