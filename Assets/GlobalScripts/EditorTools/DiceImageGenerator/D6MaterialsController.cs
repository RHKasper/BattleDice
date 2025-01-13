using UnityEngine;

namespace GlobalScripts.EditorTools.DiceImageGenerator
{
    public class D6MaterialsController : MonoBehaviour
    {
        [SerializeField] private MeshRenderer dieBody;
        [SerializeField] private MeshRenderer[] pips;

        public void ApplyBodyMaterial(Material material)
        {
            dieBody.material = material;
        }

        public void ApplyPipMaterial(Material material)
        {
            foreach (MeshRenderer pip in pips)
            {
                pip.material = material;
            }
        }
    }
}
