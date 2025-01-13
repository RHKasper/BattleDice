using System;
using GraphicExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BattleRunner
{
    public class BasicTerritoryVisualController : TerritoryVisualControllerBase
    {
        [SerializeField] private TextMeshProUGUI tempText;
        [SerializeField] private Image selectionHighlight;
        [SerializeField] private Image attackableHighlight;
        
        protected override void OnInitialize()
        {
            SetState(State.Normal);
            tempText.SetText("Initialized");
        }

        public override void UpdateInfo()
        {
            tempText.SetText("Owner: " + Territory.OwnerPlayerIndex + "\nDice: " + Territory.NumDice);
        }

        public override void SetState(State state)
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
                    break;
                case State.HoverSelectable:
                    selectionHighlight.gameObject.SetActive(true);
                    selectionHighlight.SetAlpha(.5f);
                    break;
                case State.HoverDeselectable:
                    selectionHighlight.gameObject.SetActive(true);
                    selectionHighlight.SetAlpha(.5f);
                    break;
                case State.HoverAttackable:
                    attackableHighlight.gameObject.SetActive(true);
                    attackableHighlight.SetAlpha(1);
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}