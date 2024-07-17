using UnityEngine;

namespace Runtime.Utilities
{
    public class SafeAreaContainer : MonoBehaviour
    {
        private static Rect _cachedSafeArea;

        private RectTransform _rectTransform;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void RuntimeInitialize()
        {
            RefreshSafeArea();
        }

        private static void RefreshSafeArea()
        {
            _cachedSafeArea = Screen.safeArea;
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            
            ApplySafeArea();
        }

        #if UNITY_EDITOR
        // Necessary to update the safe area in the editor to consider possible change of aspect-ratio
        // Would benefit from a more efficient way to update only when necessary, for better editor performance
        private void Update()
        {
            RefreshSafeArea();
            ApplySafeArea();
        }
        #endif

        private void ApplySafeArea()
        {
            Rect safeArea = _cachedSafeArea;
            
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;
        }
    }
}
