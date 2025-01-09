using System;
using System.Linq;
using System.Threading.Tasks;
using BattleTest.Scripts;
using GenericsExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BattleTest.UI.RollDisplayPanel
{
    public class RollDisplayPanelController : MonoBehaviour
    {
        [SerializeField] private DieRollUiController[] dieRollUiControllers;
        [SerializeField] private Sprite[] dieFaceSprites;
        [SerializeField] private TextMeshProUGUI resultsText;

        private void Awake()
        {
            Debug.Assert(dieFaceSprites.Length == 6);
        }

        public async Task ShowDiceRoll(int[] diceRoll)
        {
            Debug.Log("Starting Dice Roll - " + Time.time);
            float startTime = Time.time;
            int dieChanges = 0;
            
            for (int i = 0; i < dieRollUiControllers.Length; i++)
            {
                dieRollUiControllers[i].gameObject.SetActive(i < diceRoll.Length);
            }
            
            float endTime = Time.time + .001f * UserCueSequencer.DefaultCueDelayMs * 3;
            float pipChangeTimeInterval = UserCueSequencer.DefaultCueDelayMs / 10.0f;
            
            while (Time.time < endTime)
            {
                for (int i = 0; i < diceRoll.Length; i++)
                {
                    dieRollUiControllers[i].ShowPips(dieFaceSprites.GetRandom());
                }

                dieChanges++;
                await Task.Delay(TimeSpan.FromMilliseconds(pipChangeTimeInterval));
            }
            
            for (int i = 0; i < diceRoll.Length; i++)
            {
                var sprite = dieFaceSprites[diceRoll[i] - 1];
                dieRollUiControllers[i].ShowPips(sprite);
            }

            await Task.Delay(TimeSpan.FromMilliseconds(UserCueSequencer.DefaultCueDelayMs));
            resultsText.text = diceRoll.Sum().ToString();
            
            Debug.Log("Ending ShowDiceRoll - " + Time.time + " (elapsed: " + (Time.time - startTime) + ") Die Changes: " + dieChanges);
        }
    }
}
