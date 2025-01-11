using UnityEngine.EventSystems;

namespace BattleRunner
{
    public class BasicTerritoryVisualController : TerritoryVisualControllerBase
    {
        public override void OnPointerExit(PointerEventData eventData)
        {
            //throw new System.NotImplementedException();
        }

        public override void OnSelect()
        {
            throw new System.NotImplementedException();
        }

        public override void OnDeselect()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnInitialize()
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnPointerEnterWhenSelectable(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnPointerEnterWhenDeselectable(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnPointerEnterWhenAttackable(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnPointerEnterWhenUninteractable(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}