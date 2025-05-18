using UnityEngine;

namespace GlobalScripts.EditorTools.DiceImageGenerator
{
    public class D6MaterialsController : MonoBehaviour
    {
        [SerializeField] private MeshRenderer dieBody;
        [SerializeField] private MeshRenderer[] pips;

        public void ApplyBodyColor(Color color)
        {
            dieBody.material.color = color;
        }

        public void ApplyPipColor(Color color)
        {
            foreach (MeshRenderer pip in pips)
            {
                pip.material.color = color;
            }
        }
    }
}
