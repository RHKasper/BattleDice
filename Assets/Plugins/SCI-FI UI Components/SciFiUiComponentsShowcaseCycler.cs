using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plugins.SCI_FI_UI_Components
{
    public class SciFiUiComponentsShowcaseCycler : MonoBehaviour
    {
        [SerializeField] private List<GameObject> objects;
        
        private int _index = 0;

        private void Start()
        {
            _index = objects.IndexOf(objects.First(o => o.activeSelf));
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.anyKeyDown)
            {
                objects[_index].SetActive(false);
                _index = (_index + 1) % objects.Count;
                objects[_index].SetActive(true);;
            }
        }
    }
}
