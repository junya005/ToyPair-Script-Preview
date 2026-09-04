using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class PlayerSounds : MonoBehaviour
{
    public enum SoundType
    {
        Jump,
        Dush_Charge,
        Dush_Relese_1,
        Dush_Relese_2,
        Brake,
        Collision_1,
        Collision_2,
        Stan,
        Death,
        Interact,
    }

    [System.Serializable]
    public class SoundPairData
    {
        public SoundType key;
        public AudioClip sound;
    }

    private AudioSource _audioSource;

    [SerializeField, Tooltip("使用する音源のリストを指定してください")]
    private List<SoundPairData> _soundPairList = new List<SoundPairData>();

    private Dictionary<SoundType, AudioClip> _sounds = new Dictionary<SoundType, AudioClip>();

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        foreach(var pair in _soundPairList)
        {
            _sounds.Add(pair.key, pair.sound);
        }
    }

    public void PlayOneShot(SoundType type)
    {
        if(_sounds.ContainsKey(type))
        {
            if(_sounds[type] == null) return;
            StopLoop();
            _audioSource.PlayOneShot(_sounds[type]);
        }
        else
        {
            Debug.LogWarning($"{type}の音は現在登録されていません。");
        }
    }

    public void PlayLoop(SoundType type)
    {
        if(_sounds.ContainsKey(type))
        {
            if(_audioSource.isPlaying && _audioSource.clip == _sounds[type]) return;

            StopLoop();

            _audioSource.loop = true;
            _audioSource.clip = _sounds[type];
            _audioSource.Play();
        }
    }

    public void StopLoop()
    {
        _audioSource.Stop();
        _audioSource.loop = false;
    }
}
