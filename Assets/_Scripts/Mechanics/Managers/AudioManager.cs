using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager am { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip earthCardPlaySFX;
    [SerializeField] private AudioClip waterCardPlaySFX;
    [SerializeField] private AudioClip airCardPlaySFX;
    [SerializeField] private AudioClip fireCardPlaySFX;
    [SerializeField] private AudioClip winSFX;
    [SerializeField] private AudioClip loseSFX;
    [SerializeField] private AudioClip buttonPressSFX;
    [SerializeField] private AudioClip foilRevealSFX;

    private bool isMuted = false;

    public AudioClip MenuMusic => menuMusic;
    public AudioClip ButtonClicksSFX => buttonPressSFX;
    public bool IsMuted => isMuted;

    private void Awake()
    {
        if (am == null)
        {
            am = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //Load saved mute state
        isMuted = PlayerPrefs.GetInt("isMuted", 0) == 1;
        ApplyMute();
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("isMuted", isMuted ? 1 : 0);
        ApplyMute();
    }

    private void ApplyMute()
    {
        float volume = isMuted ? -80f : 0f;
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayCardSFX(ElementType element)
    {
        AudioClip clip = null;
        switch (element)
        {
            case ElementType.Earth: clip = earthCardPlaySFX; break;
            case ElementType.Water: clip = waterCardPlaySFX; break;
            case ElementType.Air: clip = airCardPlaySFX; break;
            case ElementType.Fire: clip = fireCardPlaySFX; break;
        }

        if (clip != null) PlaySFX(clip);
    }

}
