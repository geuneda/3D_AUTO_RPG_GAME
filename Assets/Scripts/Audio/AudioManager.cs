using System.Collections;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Pool")]
    private ObjectPool<AudioSource> audioSourcePool;
    [SerializeField] private int poolSize = 5;

    protected override void Awake()
    {
        base.Awake();
        InitializeAudioPool();
    }

    private void InitializeAudioPool()
    {
        var audioSourceObj = new GameObject("AudioSource");
        var audioSource = audioSourceObj.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSourceObj.SetActive(false);

        audioSourcePool = new ObjectPool<AudioSource>(audioSource, poolSize, transform);
        Destroy(audioSourceObj);
    }

    public void PlayOneShot(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;

        var audioSource = audioSourcePool.Get();
        audioSource.transform.position = position;
        audioSource.volume = volume;
        audioSource.clip = clip;
        audioSource.Play();

        StartCoroutine(ReturnToPool(audioSource, clip.length));
    }

    private IEnumerator ReturnToPool(AudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (audioSource)
        {
            audioSource.Stop();
            audioSourcePool.Return(audioSource);
        }
    }
} 