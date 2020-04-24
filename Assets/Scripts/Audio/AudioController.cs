using System;
using System.Linq;
using Hellmade.Sound;
using UnityEngine;
using Audio = General.Audio;
using Random = UnityEngine.Random;

public class AudioController : PersistentSingleton<AudioController>
{
    private const  float MaxVolume = 1;
    [SerializeField] private StringAudioDictionary soundClips = new StringAudioDictionary();
    [SerializeField] private StringAudioDictionary musicClips = new StringAudioDictionary();

    [SerializeField] [Range(0, MaxVolume)] private float musicVolume = MaxVolume / 2;
    [SerializeField] [Range(0, MaxVolume)] private float soundVolume = MaxVolume / 2;
    
    public string defaultMusic;

    private int _musicPlaying = -1;
    public bool IsMusicPlaying => _musicPlaying != -1;

    private bool _loaded;
    
    public void Awake()
    {
        if (!InitSingletonInstance())
            return;

        EazySoundManager.IgnoreDuplicateMusic = false;
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", MaxVolume / 2);
        soundVolume = PlayerPrefs.GetFloat("SoundVolume", MaxVolume / 2);
        SetMusicVolume(musicVolume);
        SetSoundVolume(soundVolume);
        _loaded = true;
    }

    //  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Volumes  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public float SetMusicVolume(float newMusicVolume)
    {
        musicVolume = Mathf.Clamp(newMusicVolume, 0f, MaxVolume);
        EazySoundManager.GlobalMusicVolume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        return musicVolume;
    }
    
    public float SetSoundVolume(float newSoundVolume)
    {
        soundVolume = Mathf.Clamp(newSoundVolume, 0f, MaxVolume);
        EazySoundManager.GlobalSoundsVolume = soundVolume;
        EazySoundManager.GlobalUISoundsVolume = soundVolume;
        PlayerPrefs.SetFloat("SoundVolume", soundVolume);
        return soundVolume;
    }

    private void OnValidate()
    {
        if (!_loaded) 
            return;
        
        SetMusicVolume(musicVolume);
        SetSoundVolume(soundVolume);
    }

    public float GetMusicVolume() => musicVolume;
    public float GetSoundVolume() => soundVolume;

    //  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Simple functions  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    #region

    public void PlayMusic(string key)
    {
        PlayMusic(key, null, null, null);
    }

    public void PlaySound(string key, Transform t = null)
    {
        PlaySound(key, null, null, null, null, null, t);
    }
    
    public void PlaySound(string key)
    {
        PlaySound(key, null, null, null, null, null, null);
    }

    public void PlaySoundClip(AudioClip clip)
    {
        PlaySound(clip, EazySoundManager.GlobalSoundsVolume);
    }

    // Default theme helper
    public void PlayDefaultMusic(float? volume = null, bool? loop = null, float? pitch = null)
    {
        PlayMusic(defaultMusic, volume, loop, pitch);
    }
    
    #endregion

    private AudioOptions UnifyAudioOptions(
        Audio audioEntry,
        bool? loop = null,
        float? volume = null,
        float? pitch = null,
        float? volumeVariation = null,
        float? pitchVariation = null)

    {
        var rLoop = loop ?? audioEntry.loop;
        var rVolume = volume ?? audioEntry.volume;
        var rPitch = pitch ?? audioEntry.pitch;
        var rVolumeVariation = volumeVariation ?? audioEntry.volumeVariation;
        var rPitchVariation = pitchVariation ?? audioEntry.pitchVariation;

        return new AudioOptions
        {
            Loop = rLoop,
            Volume = rVolume,
            Pitch = rPitch,
            VolumeVariation = rVolumeVariation,
            PitchVariation = rPitchVariation,
        };
    }

    private (float volume, float pitch) ApplyVariations(AudioOptions options)
    {
        var volume = options.Volume + Random.Range(-options.VolumeVariation, options.VolumeVariation);
        var pitch = options.Pitch + Random.Range(-options.PitchVariation, options.PitchVariation);

        return (volume, pitch);
    }


    // Basically wrappers for EazySoundManager's method, which fetch the Audio from the Dictionary
    public int PlayMusic(string key, float? volume, bool? loop = null, float? pitch = null)
    {
        if (!musicClips.TryGetValue(key, out var musicEntry))
            return -1;

        if (musicEntry == null)
            return -1;

        var clip = musicEntry.audioClip;
        var options = UnifyAudioOptions(musicEntry, loop, volume, pitch);
        var (f, pitch1) = ApplyVariations(options);
        return PlayMusic(clip, f, options.Loop, pitch1);
    }

    public int PlayMusic(AudioClip clip, float volume = 0, bool loop = true, float pitch = 1)
    {
        if (!Application.isPlaying)
        {
            return -1;
        }
        
        var id = EazySoundManager.PlayMusic(clip, volume, loop, true);
        var eazyAudio = EazySoundManager.GetMusicAudio(id);
        eazyAudio.Pitch = pitch;

        _musicPlaying = id;
        
        return id;
    }

    public int PlayRandomSound(string key, float? volume = null, float? pitch = null, float? volumeVariation = null,
        float? pitchVariation = null, bool? loop = null, Transform sourceTransform = null)
    {
        var soundEntries = soundClips.Where(kvp => kvp.Key.StartsWith(key)).ToList();
        if (soundEntries.Any())
        {
            var entry = soundEntries[Random.Range(0, soundEntries.Count)];
            return PlaySound(entry.Key, volume, pitch, volumeVariation, pitchVariation, loop, sourceTransform);
        }

        return -1;
    }

    public int PlaySound(string key, float? volume, float? pitch = null, float? volumeVariation = null,
        float? pitchVariation = null, bool? loop = null, Transform sourceTransform = null)
    {
        if (!soundClips.TryGetValue(key, out var soundEntry))
            return -1;

        if (soundEntry == null)
            return -1;

        var clip = soundEntry.audioClip;
        var options = UnifyAudioOptions(soundEntry, loop, volume, pitch, volumeVariation, pitchVariation);
        var playOptions = ApplyVariations(options);
        return PlaySound(clip, playOptions.volume, options.Loop, playOptions.pitch, sourceTransform);
    }

    public int PlaySound(AudioClip clip, float volume, bool loop = true, float pitch = 1,
        Transform sourceTransform = null)
    {
        if (!Application.isPlaying)
        {
            return -1;
        }
        
        var id = EazySoundManager.PlaySound(clip, volume, loop, sourceTransform);
        var eazyAudio = EazySoundManager.GetSoundAudio(id);
        eazyAudio.Pitch = pitch;

        return id;
    }

    public int PlayUISound(AudioClip clip, float volume, float pitch = 1)
    {
        if (!Application.isPlaying)
        {
            return -1;
        }
        
        var id = EazySoundManager.PlayUISound(clip, volume);
        var eazyAudio = EazySoundManager.GetUISoundAudio(id);
        eazyAudio.Pitch = pitch;

        return id;
    }

    private struct AudioOptions
    {
        public bool Loop;
        public float Volume;
        public float Pitch;
        public float VolumeVariation;
        public float PitchVariation;
    }

    [Serializable]
    public struct AudioEntry
    {
        public string Key;
        public Audio Audio;
    }
}