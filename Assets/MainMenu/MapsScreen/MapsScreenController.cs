using System.Collections;
using System.Linq;
using GlobalScripts;
using Maps;
using RKUnityToolkit.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.MapsScreen
{
    public class MapsScreenController : MonoBehaviour
    {
        [SerializeField] private GenericListDisplay mapsListDisplay;
        [SerializeField] private MapListDisplayElementController mapDisplayElementPrefab;
        [SerializeField] private ScrollRect mapsListScrollRect;
        [SerializeField] private TextMeshProUGUI selectedMapTitleText;
        [SerializeField] private TextMeshProUGUI selectedMapDescriptionText;
        [SerializeField] private Image selectedMapPreviewImage;

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

        public void OnMapToggleActivated(MapListDisplayElementController mapToggle)
        {
            selectedMapTitleText.SetText(mapToggle.Data.MapName);
            selectedMapDescriptionText.SetText(mapToggle.Data.MapDescription);
            selectedMapPreviewImage.sprite = mapToggle.Data.MapPreviewImage;
        }

        private void StartCustomMapGame(GameplayMap gameplayMap)
        {
            BattleLoader.LoadBattle(gameplayMap);
        }
    }
}