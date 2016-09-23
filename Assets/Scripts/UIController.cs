using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public LifeController lifeController;

    public Button newLifeInitialButton;
    public Button startLifeButton;
    private Text _startLifeButtonText;
    public Button editModeButton;
    private Text _editModeButtonText;
    
    public InputField size;
    public InputField r1, r2, r3, r4;
    public InputField delay;

    private void Awake()
    {
        Initialize();

        EventManager.OnEnterToEditModeRequest += SetEditViewForEditButton;
        EventManager.OnExitFromEditModeRequest += SetDefaultViewForEditButton;
        EventManager.OnStartLifeRequest += SetLifeViewForLifeButton;
        EventManager.OnStopLifeRequest += SetDefaultViewForLifeButton;
    }

    private void Start()
    {
        InitializeInputsFromDefaultData();
    }

    private void OnDestroy()
    {
        EventManager.OnEnterToEditModeRequest -= SetEditViewForEditButton;
        EventManager.OnExitFromEditModeRequest -= SetDefaultViewForEditButton;
        EventManager.OnStartLifeRequest -= SetLifeViewForLifeButton;
        EventManager.OnStopLifeRequest -= SetDefaultViewForLifeButton;
    }

    private void Initialize()
    {
        newLifeInitialButton.onClick.AddListener(NewLifeInitializingHandler);
        startLifeButton.onClick.AddListener(StartLifeHandler);
        editModeButton.onClick.AddListener(EditModeHandler);

        _editModeButtonText = editModeButton.transform.GetChild(0).GetComponent<Text>();
        _startLifeButtonText = startLifeButton.transform.GetChild(0).GetComponent<Text>();
    }

    private void NewLifeInitializingHandler()
    {
        if (StateHandler.Instance.editState == StateHandler.EditState.Edit)
        {
            Debug.LogWarning("You can not initialize new life when you in edit mode.");
            return;
        }

        EventManager.NewLifeInitializingRequest();
    }

    private void StartLifeHandler()
    {
        if (StateHandler.Instance.editState == StateHandler.EditState.Edit)
        {
            Debug.LogWarning("You can not start life when you in edit mode.");
            return;
        }

        if (StateHandler.Instance.lifeState == StateHandler.LifeState.Idle)
        {
            StateHandler.Instance.lifeState = StateHandler.LifeState.Life;
            EventManager.StartLifeRequest();

        }

        else
        {
            StateHandler.Instance.lifeState = StateHandler.LifeState.Idle;
            EventManager.StopLifeRequest();
        }
    }

    private void EditModeHandler()
    {
        if (StateHandler.Instance.lifeState == StateHandler.LifeState.Life)
        {
            Debug.LogWarning("You can not edit life grid when you in life mode.");
            return;
        }

        if (StateHandler.Instance.editState == StateHandler.EditState.Idle)
        {
            StateHandler.Instance.editState = StateHandler.EditState.Edit;
            EventManager.EnterToEditModeRequest();
        }

        else
        {
            StateHandler.Instance.editState = StateHandler.EditState.Idle;
            EventManager.ExitFromEditModeRequest();
        }
    }

    private void SetEditViewForEditButton()
    {
        _editModeButtonText.text = Config.editModeButtonEditModeText;
    }

    private void SetDefaultViewForEditButton()
    {
        _editModeButtonText.text = Config.editModeButtonDefaultText;
    }

    private void SetLifeViewForLifeButton()
    {
        _startLifeButtonText.text = Config.startLifeButtonLifeModeText;
    }

    private void SetDefaultViewForLifeButton()
    {
        _startLifeButtonText.text = Config.startLifeButtonDefaultModeText;
    }

    private void InitializeInputsFromDefaultData()
    {
        size.text = lifeController.config.size.ToString();
        r1.text = lifeController.config.r1.ToString();
        r2.text = lifeController.config.r2.ToString();
        r3.text = lifeController.config.r3.ToString();
        r4.text = lifeController.config.r4.ToString();
        delay.text = lifeController.config.delay.ToString();
    }
}
