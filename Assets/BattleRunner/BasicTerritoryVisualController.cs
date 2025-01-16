using System;
using ColorExtensions;
using GlobalScripts;
using GraphicExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BattleRunner
{
    public class BasicTerritoryVisualController : TerritoryVisualControllerBase
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI tempText;
        [SerializeField] private Image ownerPlayerImage;
        [SerializeField] private Image diceImage;
        
        [Header("UI Highlights")]
        [SerializeField] private Image selectionHighlight;
        [SerializeField] private Image attackableHighlight;
        [SerializeField] private Image contiguousTerritoriesHighlight;
        
        protected override void OnInitialize()
        {
            SetState(State.Normal);
            tempText.SetText("Initialized");
        }

        protected override void UpdateInfo()
        {
            Debug.Log("BasicTerritoryVisualController.UpdateInfo");
            tempText.SetText("Owner: " + Territory.OwnerPlayerIndex + "\nDice: " + Territory.NumDice);
            ownerPlayerImage.color = Constants.Colors[Territory.OwnerPlayerIndex];
            diceImage.sprite = Resources.Load<Sprite>(Constants.GetDieStackSpritesPathFromResources(Territory.OwnerPlayerIndex, Territory.NumDice));
        }

        protected override void SetState(State state)
        {
            // if setting to anything but normal, reset to normal first
            if (state != State.Normal)
            {
                SetState(State.Normal);
            }
            
            switch (state)
            {
                case State.Normal:
                    selectionHighlight.gameObject.SetActive(false);
                    attackableHighlight.gameObject.SetActive(false);
                    contiguousTerritoriesHighlight.gameObject.SetActive(false);
                    break;
                case State.HoverSelectable:
                    selectionHighlight.gameObject.SetActive(true);
                    selectionHighlight.SetAlpha(.75f);
                    break;
                case State.HoverDeselectable:
                    selectionHighlight.gameObject.SetActive(true);
                    selectionHighlight.SetAlpha(.5f);
                    break;
                case State.HoverAttackable:
                    attackableHighlight.gameObject.SetActive(true);
                    attackableHighlight.SetAlpha(1);
                    break;
                case State.Selectable:
                    selectionHighlight.gameObject.SetActive(true);
                    selectionHighlight.SetAlpha(.5f);
                    break;
                case State.Selected:
                    selectionHighlight.gameObject.SetActive(true);
                    selectionHighlight.SetAlpha(1);
                    break;
                case State.Attackable:
                    attackableHighlight.gameObject.SetActive(true);
                    attackableHighlight.SetAlpha(.5f);
                    break;
                case State.Attacking:
                    break;
                case State.Defending:
                    break;
                case State.HighlightedToShowLargestContiguousGroupOfTerritories:
                    contiguousTerritoriesHighlight.gameObject.SetActive(true);
                    contiguousTerritoriesHighlight.color = Constants.Colors[Territory.OwnerPlayerIndex].WithAlpha(.75f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}