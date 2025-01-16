using System.Collections.Generic;
using BattleRunner.UI.RollDisplayPanel;
using GlobalScripts;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRunner.UI.ReinforcementDicePanel
{
    public class ReinforcementDicePanelController : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image dieImagePrefab;

        private List<Image> _images = new();
        
        public void ShowReinforcementDice(int diceCount, int playerIndex)
        {
            Sprite dieSprite = Resources.Load<Sprite>(Constants.GetThreeQuartersDieSpritesPathFromResources(playerIndex));
            int maxIndex = Mathf.Max(diceCount, _images.Count);

            for (int i = 0; i < maxIndex; i++)
            {
                if (i >= _images.Count)
                {
                    _images.Add(Instantiate(dieImagePrefab, transform));
                    _images[i].rectTransform.sizeDelta = new Vector2(rectTransform.rect.size.y, rectTransform.rect.size.y);
                }
                
                if (i < diceCount)
                {
                    _images[i].gameObject.SetActive(true);
                    _images[i].sprite = dieSprite;
                }
                else
                {
                    _images[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
