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

        public async Task ShowDiceRoll(int[] diceRoll, int playerIndex)
        {
            gameObject.SetActive(true);
            resultsDisplay.gameObject.SetActive(false);
            
            var dieFaceSprites = new[]
            {
                Resources.Load<Sprite>(Constants.GetDieFaceSpritesPathFromResources(playerIndex, 1)),
                Resources.Load<Sprite>(Constants.GetDieFaceSpritesPathFromResources(playerIndex, 2)),
                Resources.Load<Sprite>(Constants.GetDieFaceSpritesPathFromResources(playerIndex, 3)),
                Resources.Load<Sprite>(Constants.GetDieFaceSpritesPathFromResources(playerIndex, 4)),
                Resources.Load<Sprite>(Constants.GetDieFaceSpritesPathFromResources(playerIndex, 5)),
                Resources.Load<Sprite>(Constants.GetDieFaceSpritesPathFromResources(playerIndex, 6)),
            };
            
            for (int i = 0; i < dieRollUiControllers.Length; i++)
            {
                dieRollUiControllers[i].gameObject.SetActive(i < diceRoll.Length);
            }
            
            float endTime = Time.time + .001f * UserCueSequencer.DefaultCueDelayMs * 5;
            float pipChangeTimeIntervalMs = UserCueSequencer.DefaultCueDelayMs / 10.0f;
            
            while (Time.time < endTime)
            {
                for (int i = 0; i < diceRoll.Length; i++)
                {
                    dieRollUiControllers[i].ShowPips(dieFaceSprites.GetRandom());
                }

                await WebGlUtil.WebGlSafeDelay(pipChangeTimeIntervalMs);
            }
            
            for (int i = 0; i < diceRoll.Length; i++)
            {
                var sprite = dieFaceSprites[diceRoll[i] - 1];
                dieRollUiControllers[i].ShowPips(sprite);
            }

            await WebGlUtil.WebGlSafeDelay(UserCueSequencer.DefaultCueDelayMs * 2.5f);
            resultsDisplay.ShowNumber(diceRoll.Sum());
            resultsDisplay.gameObject.SetActive(true);
        }
    }
}
