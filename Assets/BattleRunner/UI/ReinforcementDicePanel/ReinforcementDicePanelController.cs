using System.Collections.Generic;
using BattleRunner.UI.RollDisplayPanel;
using GlobalScripts;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRunner.UI.ReinforcementDicePanel
{
    public class ReinforcementDicePanelController : MonoBehaviour
    {
        [SerializeField] private RectTransform diceImagesParent;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private Image dieImagePrefab;

        private List<Image> _images = new();
        private int _minDiceAcross = 4;
        
        public void ShowReinforcementDice(int diceCount, int playerIndex)
        {
            Sprite dieSprite = Resources.Load<Sprite>(Constants.GetThreeQuartersDieSpritesPathFromResources(playerIndex));
            int maxIndex = Mathf.Max(diceCount, _images.Count);
            float diceImagesRectArea = diceImagesParent.rect.height * diceImagesParent.rect.width;
            float cellSize = Mathf.Min(Mathf.Sqrt(diceImagesRectArea / maxIndex), diceImagesParent.rect.width / _minDiceAcross);
            gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);

            for (int i = 0; i < maxIndex; i++)
            {
                if (i >= _images.Count)
                {
                    _images.Add(Instantiate(dieImagePrefab, diceImagesParent));
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
