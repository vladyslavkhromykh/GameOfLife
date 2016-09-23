using UnityEngine;

public class StateHandler : MonoBehaviour
{
    private static StateHandler _instance = null;
    public static StateHandler Instance { get { return _instance; } }

    public enum BoxState
    {
        Live,
        Dead
    }

    public enum CellState
    {
        Live,
        Dead
    }

    public enum EditState
    {
        Idle,
        Edit
    }
    public EditState editState;

    public enum LifeState
    {
        Idle,
        Life
    }
    public LifeState lifeState;

    private void Awake()
    {
        SingletonInit();
        Initialize();
    }

    private void SingletonInit()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }

        _instance = this;
    }

    private void Initialize()
    {
        editState = EditState.Idle;
        lifeState = LifeState.Idle;
    }
}
