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
        private const float DieFaceChangeIntervalMS = UserCueSequencer.DefaultCueDelayMs / 10.0f;
        
        [SerializeField] private DieRollUiController[] dieRollUiControllers;
        [SerializeField] private Sprite[] dieFaceSprites;
        [SerializeField] private TextMeshProUGUI resultsText;

        private void Awake()
        {
            Debug.Assert(dieFaceSprites.Length == 6);
        }

        public async Task ShowDiceRoll(int[] diceRoll)
        {
            for (int i = 0; i < dieRollUiControllers.Length; i++)
            {
                dieRollUiControllers[i].gameObject.SetActive(i < diceRoll.Length);
            }

            float rollDuration = UserCueSequencer.DefaultCueDelayMs;
            float startTime = Time.time;

            while (Time.time < startTime + rollDuration)
            {
                for (int i = 0; i < diceRoll.Length; i++)
                {
                    dieRollUiControllers[i].ShowPips(dieFaceSprites.GetRandom());
                }

                await Task.Delay(TimeSpan.FromMilliseconds(DieFaceChangeIntervalMS));
            }
            
            for (int i = 0; i < diceRoll.Length; i++)
            {
                dieRollUiControllers[i].ShowPips(dieFaceSprites[diceRoll[i-1]]);
            }

            await Task.Delay(TimeSpan.FromMilliseconds(rollDuration));
            resultsText.text = diceRoll.Sum().ToString();
        }
    }
}
