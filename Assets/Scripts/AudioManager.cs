using System.Collections;
using System.IO;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    public EventInstance musicEventInstance;
    public EventInstance ambienceEventInstance;

    [Header("Music")]
    public EventReference titleScreenMusic;
    public EventReference combatMusic;
    public EventReference treeMusic;

    [Header("SFX")]
    public EventReference tileClack;
    public EventReference tileShuffle;
    public EventReference uiHover;
    public EventReference uiClick;
    public EventReference whoosh;
    public EventReference playerHurt;
    public EventReference enemyHurt;
    public EventReference gameOver;
    public EventReference enemyDeath;
    public EventReference qiOrb;
    public EventReference treeChannel;

    [Header("Ambience")]

    [Header("Volume")]
    [Range(0, 1)] public float masterVolume { get; set; }
    [Range(0, 1)] public float sfxVolume { get; set; }
    [Range(0, 1)] public float musicVolume { get; set; }
    [Range(0, 1)] public float ambienceVolume { get; set; }

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;
    private Bus ambienceBus;

    [HideInInspector] public bool areBussesInitialized = false;
    private bool isMusicPlaying = false;

    public const float DEFAULT_VOLUME = 0.5f;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this);
        else instance = this;

        DontDestroyOnLoad(gameObject);

        StartCoroutine(LoadBusses());
    }

    void Update()
    {
        if (areBussesInitialized)
        {
            masterBus.setVolume(masterVolume);
            sfxBus.setVolume(sfxVolume);
            musicBus.setVolume(musicVolume);
            ambienceBus.setVolume(ambienceVolume);
        }
    }

    public IEnumerator LoadBusses()
    {
        while (!RuntimeManager .HaveAllBanksLoaded) yield return null;

        masterBus = RuntimeManager.GetBus("bus:/");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");

        if (!areBussesInitialized)
        {
            masterVolume = DEFAULT_VOLUME;
            sfxVolume = DEFAULT_VOLUME;
            musicVolume = DEFAULT_VOLUME;
            ambienceVolume = DEFAULT_VOLUME;
        }

        masterBus.setVolume(masterVolume);
        sfxBus.setVolume(sfxVolume);
        musicBus.setVolume(musicVolume);
        ambienceBus.setVolume(ambienceVolume);

        areBussesInitialized = true;
    }

    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance newInstance = RuntimeManager.CreateInstance(eventReference);
        return newInstance;
    }

    public void StopMusic()
    {
        musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        isMusicPlaying = false;
    }

    public void PauseUnpause()
    {
        musicEventInstance.getPaused(out bool isMusicPaused);
        musicEventInstance.setPaused(!isMusicPaused);
    }

    public void PlayMusic(EventReference music)
    {
        StartCoroutine(InitializeMusic(music));
    }

    private IEnumerator InitializeMusic(EventReference music)
    {
        yield return new WaitUntil(() => areBussesInitialized);

        if (isMusicPlaying) StopMusic();
        musicEventInstance = CreateEventInstance(music);
        musicEventInstance.start();
        isMusicPlaying = true;
    }
}
