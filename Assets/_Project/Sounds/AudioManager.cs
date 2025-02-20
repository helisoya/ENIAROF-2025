using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.IO;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 1;
    [Range(0, 1)]
    public float sfxVolume = 1;
    [Range(0, 1)]
    public float musicVolume = 1;

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    public static AudioManager instance { get; private set; }

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    public EventInstance ambienceEventInstance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        //masterBus = RuntimeManager.GetBus("bus:/");
        //musicBus = RuntimeManager.GetBus("bus:/Music");
        //sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void Start()
    {
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        sfxBus.setVolume(sfxVolume);

    }



    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = RuntimeManager.CreateInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }


    public void SetAmbienceParameter(string parameterName, float parameterValue)
    {
        ambienceEventInstance.setParameterByName(parameterName, parameterValue);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;


    }
    
        public void StopEvent(EventReference eventReference, bool allowFadeOut = true)
    {
        EventInstance eventInstance = CreateEventInstance(eventReference);
        eventInstance.stop(allowFadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
        eventInstance.release();
    }

    public void SetParameter(EventInstance eventInstance, string parameterName, float parameterValue)
    {
        eventInstance.setParameterByName(parameterName, parameterValue);
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference; // Override l'event reference renseigné dans le GameObject
        eventEmitters.Add(emitter); // Pour du cleanup
        return emitter;
    }

    public void CleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
