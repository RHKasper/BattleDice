using UnityEngine;
using UnityEngine.EventSystems;

namespace BattleRunner
{
    public class BasicTerritoryVisualController : TerritoryVisualControllerBase
    {
        public override void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("OnPointerExit");
        }

        public override void OnSelect()
        {
        }

        public override void OnDeselect()
        {
        }

        protected override void OnInitialize()
        {
        }

        protected override void OnPointerEnterWhenSelectable(PointerEventData eventData)
        {
        }

        protected override void OnPointerEnterWhenDeselectable(PointerEventData eventData)
        {
        }

        protected override void OnPointerEnterWhenAttackable(PointerEventData eventData)
        {
        }

        protected override void OnPointerEnterWhenUninteractable(PointerEventData eventData)
        {
            
        }
    }
}