using UnityEngine;

namespace General
{
    [CreateAssetMenu(fileName = "New Audio", menuName = "Template/Audio")]
    public class Audio : ScriptableObject
    {
        public AudioClip audioClip;

        public bool uiSound = false;
        public bool loop = false;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        [Range(0, 1f)] public float volumeVariation = 0f;
        [Range(0, 1f)] public float pitchVariation = 0f;
    }
}