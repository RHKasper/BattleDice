using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Maps
{
    [ExecuteAlways]
    public class NodeStartStateDefinition : MonoBehaviour
    {
        [FormerlySerializedAs("ownerPlayerId")] public int ownerPlayerIndex;
        public int numDice;
    }
}