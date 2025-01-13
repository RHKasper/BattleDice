using UnityEngine;

namespace GlobalScripts.EditorTools.DiceImageGenerator
{
    public class D6MaterialsController : MonoBehaviour
    {
        [SerializeField] private MeshRenderer dieBody;
        [SerializeField] private MeshRenderer[] pips;

        public void ApplyBodyMaterial(Material sharedMaterial)
        {
            dieBody.sharedMaterial = sharedMaterial;
        }

        public void ApplyPipMaterial(Material sharedMaterial)
        {
            foreach (MeshRenderer pip in pips)
            {
                pip.sharedMaterial = sharedMaterial;
            }
        }
    }
}
