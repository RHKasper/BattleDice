using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainMenu
{
    public class TestMapOptionsUiController : MonoBehaviour
    {
        public void LoadTestMapScene()
        {
            SceneManager.LoadScene("BattleTest");
        }
    }
}
