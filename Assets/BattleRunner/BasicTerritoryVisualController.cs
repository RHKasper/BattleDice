using System;
using System.Collections.Generic;
using GlobalScripts;
using GraphicExtensions;
using RKUnityToolkit.ColorExtensions;
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
        [SerializeField] private Image diceImage;
        
        [Header("UI States")]
        [SerializeField] private GameObject defaultState;
        [SerializeField] private GameObject selectable;
        [SerializeField] private GameObject selectHover;
        [SerializeField] private GameObject selected;
        [SerializeField] private GameObject attackable;
        [SerializeField] private GameObject attackableHover;
        [SerializeField] private GameObject contiguousTerritoriesHighlight;
        
        [Header("Color Tint Graphics")]
        [SerializeField] List<Graphic> colorTintGraphics;
        [SerializeField] List<Graphic> darkColorTintGraphics;
        
        
        
        protected override void OnInitialize()
        {
            SetState(State.Normal);
            tempText.SetText(Territory.NodeIndex.ToString());
        }

        public override void ShowNumDice(int numDice, int? ownerPlayerIndex = null)
        {
            diceImage.sprite = Resources.Load<Sprite>(Constants.GetDieStackSpritesPathFromResources(ownerPlayerIndex ?? Territory.OwnerPlayerIndex, numDice));
        }

        protected override void UpdateGameData()
        {
            tempText.SetText(Territory.NodeIndex.ToString());
            ShowNumDice(Territory.NumDice);

            foreach (Graphic graphic in colorTintGraphics)
            {
                graphic.color = Constants.Colors[Territory.OwnerPlayerIndex].WithAlpha(graphic.color.a);
            }

            foreach (Graphic graphic in darkColorTintGraphics)
            {
                graphic.color = (Constants.Colors[Territory.OwnerPlayerIndex] * .66f).WithAlpha(graphic.color.a);
            }
        }

        protected override void SetState(State state)
        {
            // if setting to anything but normal, reset to normal first
            if (state != State.Normal)
            {
                SetState(State.Normal);
            }

            defaultState.SetActive(false);
            selected.SetActive(false);
            selectHover.SetActive(false);
            selectable.SetActive(false);
            attackable.SetActive(false);
            attackableHover.SetActive(false);
            contiguousTerritoriesHighlight.SetActive(false);
            
            switch (state)
            {
                case State.Normal:
                    defaultState.SetActive(true);
                    break;
                case State.HoverSelectable:
                case State.HoverDeselectable:
                    selectHover.SetActive(true);
                    break;
                case State.HoverAttackable:
                case State.Defending:
                    attackableHover.SetActive(true);
                    break;
                case State.Selectable:
                    selectable.SetActive(true);
                    break;
                case State.Selected:
                case State.Attacking:
                    selected.SetActive(true);
                    break;
                case State.Attackable:
                    attackable.SetActive(true);
                    break;
                case State.HighlightedToShowLargestContiguousGroupOfTerritories:
                    contiguousTerritoriesHighlight.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}