using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class EndGameScript : MonoBehaviour
{
    public static EndGameScript singleton;
    public static bool UseCollision = false;

    [SerializeField] private GameObject graphics;
    [SerializeField] private ParticleSystem endFireParticles;
    [SerializeField] private ParticleSystem endWaterParticles;
    private GameObject UIManager;
    public PlayerStateManager.State stateIndex;

    [SerializeField]
    private UnityEvent onSuccess;
    [SerializeField]
    private UnityEvent onFail;

    void Awake()
    {
        singleton = this;
        UIManager = GameObject.Find("UIManager");
    }

    void OnDestroy()
    {
        if (singleton == this)
            singleton = null;
    }

    void Start()
    {
        // Setup enum utils settings
        /*const int unusedCount = 2; // How many are utils ones and not effective ones (Default & No Effect => 2)
        const int skipCount = 1; // How many values to skip from the array (1 => check what's the the values of the enum elements, start at 1 not 0)
        System.Array values = System.Enum.GetValues(typeof(PlayerStateManager.State));*/

        // Get Play mode & previously recorded values
        GameManager.PlayMode mode = GameManager.Mode;
        int recordedState = GameManager.RecordState;
        Vector3 recordedPos = GameManager.RecordedPos;
        Quaternion recordedRot = GameManager.RecordedRot;

        // Sample in replay mode (get state from recorded one)
        if (mode == GameManager.PlayMode.Playing && recordedState != -1)
        {
            UseCollision = true;
            stateIndex = (PlayerStateManager.State)recordedState;
            transform.SetPositionAndRotation(recordedPos, recordedRot);
            graphics.SetActive(true);

            // Sample in recording mode (Generate new random state)
        }
        else
        {
            // Sample wich effect to apply. Can be updated/better later
            //stateIndex = (PlayerStateManager.State)values.GetValue(skipCount + Random.Range(0, values.Length - unusedCount));
            UseCollision = false;
            stateIndex = PlayerStateManager.State.Clean;
            graphics.SetActive(false);
        }


        // Check if fire is applied
        if ((stateIndex & PlayerStateManager.State.Burn) == PlayerStateManager.State.Burn)
        {
            endFireParticles.Play();

            // Check if Water is applied
        }
        else if ((stateIndex & PlayerStateManager.State.Wet) == PlayerStateManager.State.Wet)
        {
            endWaterParticles.Play();
        }
    }

    public void TriggerEnd(Transform anchor, int stateCache)
    {
        // Manage recording mode end
        if (GameManager.Mode == GameManager.PlayMode.Recording) {
            //Debug.Log("Recording state : " + stateCache);
            GameManager.RecordState = stateCache;
            GameManager.RecordTime = GameObject.Find("Timer").GetComponent<TimerScript>().elapsedTime;
            GameManager.RecordedPos = anchor.position;
            GameManager.RecordedRot = anchor.rotation;
            GameManager.LoadNextMode();
            return;
        }

        // Manage replaying mode end
        int stateIndex = (int)this.stateIndex;
        if ((stateIndex & stateCache) == stateIndex) {
            //Debug.Log("Vous avez gagn�(e) !!!");
            // GameManager.ClearRecord();
            // GameManager.LoadNextMode();
            onSuccess.Invoke();
            GameObject UIManager = GameObject.Find("UIManager");
            UIManager.GetComponent<InGameUIManager>().WinUI();

        } else {
            //Debug.Log("Vous n'�tes pas dans le bon �tat !");
            onFail.Invoke();
        }
    }
}