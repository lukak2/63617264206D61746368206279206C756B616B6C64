using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UserInterface
{
    public class GameStartButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text text;

        public event Action OnClick;

        public void Initailize(string buttonText, Action onClick)
        {
            text.text = buttonText;
            OnClick += onClick;
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked()
        {
            OnClick?.Invoke();
        }
    }
}