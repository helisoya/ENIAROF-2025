using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer instance; // Singleton instance

    private EventInstance myMusicInstance;

    private void Awake()
    {
        // Ensure this is the only instance
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            //Destroy(gameObject); // Destroy duplicate
        }
    }

    public void Start()
    {
        FMODEvents bonjour = FMODEvents.instance;
        myMusicInstance = RuntimeManager.CreateInstance(bonjour.Gameplay_Music);
        myMusicInstance.start();

    }

    private void Update()
    {
        //myMusicInstance.setParameterByName("Health", 10);
    }

    public void OutofMenu()
    {
        // myMusicInstance.setParameterByName("menu", 0);
    }

    public void BackToMenu()
    {
        //myMusicInstance.setParameterByName("menu", 1);
        //myMusicInstance.setParameterByName("victory", 0);
        //myMusicInstance.setParameterByName("defeat", 0);
        //AudioManager.instance.ambienceEventInstance.setParameterByName("filter EQ", 0);
        //FMODUnity.RuntimeManager.StudioSystem.setParameterByName("filter EQ", 0f);
    }

    public void Replay()
    {

        //myMusicInstance.setParameterByName("menu", 0);  
        //myMusicInstance.setParameterByName("victory", 0);
        // myMusicInstance.setParameterByName("defeat", 0);
        // myExplosionInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        // _dashCooldownEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        //_dashCanalisationEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        // AudioManager.instance.ambienceEventInstance.setParameterByName("filter EQ", 0);
        //FMODUnity.RuntimeManager.StudioSystem.setParameterByName("filter EQ", 0f);
    }

    public void VictoryMusic()
    {
        //myMusicInstance.setParameterByName("victory", 1);
        //AudioManager.instance.ambienceEventInstance.setParameterByName("filter EQ", 1);
        //FMODUnity.RuntimeManager.StudioSystem.setParameterByName("filter EQ", 1.0f);
    }

    public void DefeatMusic()
    {
        // myMusicInstance.setParameterByName("defeat", 1);
        // AudioManager.instance.ambienceEventInstance.setParameterByName("filter EQ", 1);
        // FMODUnity.RuntimeManager.StudioSystem.setParameterByName("filter EQ", 1.0f);
    }

}