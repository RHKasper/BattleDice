using System.Collections;
using System.Linq;
using GlobalScripts;
using Maps;
using RKUnityToolkit.UIElements;
using RKUnityToolkit.UnityExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Toggle = UnityEngine.UI.Toggle;

namespace MainMenu.MapsScreen
{
    public class MapsScreenController : MonoBehaviour
    {
        [SerializeField] private GenericListDisplay mapsListDisplay;
        [SerializeField] private MapListDisplayElementController mapDisplayElementPrefab;
        [SerializeField] private ScrollRect mapsListScrollRect;
        [SerializeField] private TextMeshProUGUI selectedMapTitleText;

        IEnumerator Start()
        {
            mapsListDisplay.DisplayList(BattleLoader.GetCustomMaps(), mapDisplayElementPrefab, this);
            mapsListDisplay.GetActiveListItems<MapListDisplayElementController>().First().ToggleOn();
            yield return null;
            mapsListScrollRect.content.anchoredPosition = Vector2.zero; // fix scroll rect starting scrolled to the right
        }

        public void OnClickStartGameButton()
        {
            MapListDisplayElementController activeToggle = mapsListDisplay.GetActiveListItems<MapListDisplayElementController>().First(e => e.IsToggledOn);
            GameplayMap selectedMap = activeToggle.GetComponent<MapListDisplayElementController>().Data;
            StartCustomMapGame(selectedMap);
        }

        private void StartCustomMapGame(GameplayMap gameplayMap)
        {
            BattleLoader.LoadBattle(gameplayMap);
        }

        public void OnMapToggleActivated(MapListDisplayElementController mapToggle)
        {
            selectedMapTitleText.SetText(mapToggle.Data.MapName);
        }
    }
}