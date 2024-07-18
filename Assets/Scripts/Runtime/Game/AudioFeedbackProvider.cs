using UnityEngine;

namespace Runtime.Game
{
    public class AudioFeedbackProvider : MonoBehaviour
    {
        public static AudioFeedbackProvider Instance { get; private set; }

        [field: SerializeField] public AudioClip MatchSound { get; private set; }
        [field: SerializeField] public AudioClip MismatchSound { get; private set; }
        [field: SerializeField] public AudioClip GameOverSound { get; private set; }
        [field: SerializeField] public AudioClip RevealSound { get; private set; }

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