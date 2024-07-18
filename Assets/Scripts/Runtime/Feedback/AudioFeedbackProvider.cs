using UnityEngine;

namespace Runtime.Feedback
{
    public class AudioFeedbackProvider : MonoBehaviour
    {
        public static AudioFeedbackProvider Instance { get; private set; }

        [field: SerializeField] public AudioLibrary AudioLibrary { get; private set; }

        [SerializeField] private AudioSource oneShotAudioSource;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlayClip(AudioClip clip)
        {
            oneShotAudioSource.PlayOneShot(clip);
        }
    }
}