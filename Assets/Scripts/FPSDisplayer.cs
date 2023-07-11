using Unity.VisualScripting;
using UnityEngine;

namespace DefaultNamespace
{
    // display fps counter on GUI
    public class FPSDisplayer : MonoBehaviour
    {
        private int _framesCount;
        private float _framesTime;
        private float _fps;

        private void Update()
        {
            _framesCount++;
            _framesTime += Time.unscaledDeltaTime;
            if (_framesTime > 1)
            {
                _fps = _framesCount / _framesTime;
                _framesCount = 0;
                _framesTime = 0;
            }
        }

        private void OnGUI()
        {
            //GUI.Label(new Rect(0, 0, 200, 200), "FPS: " + _fps);
            // like that but scale according to screen size
            GUI.Label(new Rect(0, 0, Screen.width * 0.1f, Screen.height * 0.1f), "FPS: " + _fps);
        }
    }
}