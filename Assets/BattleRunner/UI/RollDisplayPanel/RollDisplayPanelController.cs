using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericsExtensions;
using GlobalScripts;
using ReusableUi;
using TMPro;
using UnityEngine;

namespace BattleRunner.UI.RollDisplayPanel
{
    public class RollDisplayPanelController : MonoBehaviour
    {
        [SerializeField] private DieRollUiController[] dieRollUiControllers;
        [SerializeField] private TwoDigitNumberDisplay resultsDisplay;
        [SerializeField] private AudioSource audioSource;

        private readonly Dictionary<int, Sprite[]> _playerIndexToDieFaceSprites = new();

        private float PipChangeTimeIntervalMs => UserCueSequencer.DefaultCueDelayMs * .1f;
        private float NumberShowDelayMs => UserCueSequencer.DefaultCueDelayMs * 2.5f;
        
        public async Task ShowDiceRoll(int[] diceRoll, int playerIndex)
        {
            SetRollStartState(playerIndex, diceRoll.Length);
            _ = PlayDieRollSounds(diceRoll.Length, GetDieRollDurationMs(diceRoll.Length) + NumberShowDelayMs);
            await AnimateDiceRoll(diceRoll, playerIndex);
            await DisplayDieRollResults(diceRoll, playerIndex);
        }

        private async Task DisplayDieRollResults(int[] diceRoll, int playerIndex)
        {
            for (int i = 0; i < diceRoll.Length; i++)
            {
                var sprite = _playerIndexToDieFaceSprites[playerIndex][diceRoll[i] - 1];
                dieRollUiControllers[i].ShowPips(sprite);
            }
            
            await WebGlUtil.WebGlSafeDelay(NumberShowDelayMs);
            
            resultsDisplay.ShowNumber(diceRoll.Sum());
            resultsDisplay.gameObject.SetActive(true);
        }

        private async Task AnimateDiceRoll(int[] diceRoll, int playerIndex)
        {
            float endTime = Time.time + GetDieRollDurationMs(diceRoll.Length) * .001f;
            while (Time.time < endTime)
            {
                for (int i = 0; i < diceRoll.Length; i++)
                {
                    dieRollUiControllers[i].ShowPips(_playerIndexToDieFaceSprites[playerIndex].GetRandom());
                }
                
                await WebGlUtil.WebGlSafeDelay(PipChangeTimeIntervalMs);
            }
        }

        private void SetRollStartState(int playerIndex, int numDice)
        {
            gameObject.SetActive(true);
            resultsDisplay.gameObject.SetActive(false);

            if (!_playerIndexToDieFaceSprites.ContainsKey(playerIndex))
            {
                _playerIndexToDieFaceSprites[playerIndex] = new[]
                {
                    Resources.Load<Sprite>(Constants.GetDieFaceSpritesPathFromResources(playerIndex, 1)),
                    Resources.Load<Sprite>(Constants.GetDieFaceSpritesPathFromResources(playerIndex, 2)),
                    Resources.Load<Sprite>(Constants.GetDieFaceSpritesPathFromResources(playerIndex, 3)),
                    Resources.Load<Sprite>(Constants.GetDieFaceSpritesPathFromResources(playerIndex, 4)),
                    Resources.Load<Sprite>(Constants.GetDieFaceSpritesPathFromResources(playerIndex, 5)),
                    Resources.Load<Sprite>(Constants.GetDieFaceSpritesPathFromResources(playerIndex, 6)),
                };
            }
            
            for (int i = 0; i < dieRollUiControllers.Length; i++)
            {
                dieRollUiControllers[i].gameObject.SetActive(i < numDice);
            }
        }
        
        private async Task PlayDieRollSounds(int numDiceRolled, float desiredDurationMs)
        {
            Debug.Log("desired duration: " + desiredDurationMs + "ms | clip length: " + audioSource.clip.length * 1000 + " ms");
            for (int i = 0; i < numDiceRolled; i++)
            {
                audioSource.Play();
                await WebGlUtil.WebGlSafeDelay(audioSource.clip.length * 1000);
            }
        }

        private float GetDieRollDurationMs(int numDice) => audioSource.clip.length * 1000 * numDice - NumberShowDelayMs;
    }
}
