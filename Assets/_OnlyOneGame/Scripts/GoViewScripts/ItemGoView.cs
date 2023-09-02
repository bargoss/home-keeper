using TMPro;
using UnityEngine;

namespace _OnlyOneGame.Scripts.GoViewScripts
{
    public class ItemGoView : MonoBehaviour
    {
        public TextMeshPro m_Text;

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }
        
        public void Restore(string text)
        {
            m_Text.text = text;
        }
    }
}