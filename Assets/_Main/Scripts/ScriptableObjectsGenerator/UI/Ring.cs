using UnityEngine;

namespace Assets._Main.Scripts.ScriptableObjectsGenerator.UI
{
    [CreateAssetMenu(fileName = "Ring", menuName = "RingMenu/Ring", order = 1)]
    public class Ring : ScriptableObject
    {

        public RingElement[] Elements;

    }
}
