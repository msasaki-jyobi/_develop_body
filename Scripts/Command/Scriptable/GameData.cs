using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace develop_body
{
    [CreateAssetMenu(fileName = "GameData", menuName = "develop_body / GameData")]
    public class GameData : ScriptableObject
    {
        public List<GameObject> Items = new List<GameObject>();
        public List<string> PlayerStateNames = new List<string>();
    }
}