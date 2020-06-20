using UnityEngine;

namespace Assets._Main.Scripts.ScriptableObjectsGenerator.UI
{
    [CreateAssetMenu(fileName = "RingElement", menuName = "RingMenu/Element", order = 1)]
    public class RingElement : ScriptableObject
    {
        public string Name;

        public Sprite Icon;

        public Ring NextRing; //probably dont need that


    }
}
