using UnityEngine;

namespace Runtime.Feedback
{
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "Game/Audio Library", order = 0)]
    public class AudioLibrary : ScriptableObject
    {
        [field: SerializeField] public AudioClip CardFlip { get; private set; }
        [field: SerializeField] public AudioClip CardMatch { get; private set; }
        [field: SerializeField] public AudioClip CardMismatch { get; private set; }
        [field: SerializeField] public AudioClip GameOver { get; private set; }
    }
}
