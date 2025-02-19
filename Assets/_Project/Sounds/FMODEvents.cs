using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Quizz")]
    
    [field: SerializeField] public EventReference Typewriter_SFX {get; private set;}

    [field: SerializeField] public EventReference ButtonClickLeft_SFX {get; private set;}
    [field: SerializeField] public EventReference ButtonClickRight_SFX {get; private set;}
    
    [field: SerializeField] public EventReference Transition_SFX {get; private set;}

    [field: SerializeField] public EventReference BookCompleted_SFX {get; private set;}

    [field: Header("Library Menu")]
    [field: SerializeField] public EventReference BookHover_SFX { get; private set; }
    [field: SerializeField] public EventReference BookPick_SFX { get; private set; }
    
    [field: SerializeField] public EventReference BookStored_SFX { get; private set; }
    [field: SerializeField] public EventReference Library_AMB { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference Gameplay_Music {get; private set;}
    [field: SerializeField] public EventReference Library_Music { get; private set; }  
    public static FMODEvents instance { get; private set;} // Une seule instance par scène

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one FMOD Events instance in the scene!!");
        }
        instance = this;
    }
}
