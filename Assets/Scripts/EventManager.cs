using UnityEngine;
using System.Collections;

public static class EventManager
{
    public delegate void EnterToEditModeRequestDelegate();
    public static event EnterToEditModeRequestDelegate OnEnterToEditModeRequest;

    public delegate void ExitFromEditModeRequestDelegate();
    public static event ExitFromEditModeRequestDelegate OnExitFromEditModeRequest;

    public delegate void UpdateConfigurationRequestDelegate();
    public static event UpdateConfigurationRequestDelegate OnUpdateConfigurationRequest;

    public delegate void SetLiveCellInEditModeDelegate(Box deadBox);
    public static event SetLiveCellInEditModeDelegate OnSetLiveCellInEditMode;

    public delegate void SetDeadCellInEditModeDelegate(Cell cellToSetDead);
    public static event SetDeadCellInEditModeDelegate OnSetDeadCellInEditMode;

    public delegate void NewLifeInitializingRequestDelegate();
    public static event NewLifeInitializingRequestDelegate OnNewLifeInitializingRequest;

    public delegate void StartLifeRequestDelegate();
    public static event StartLifeRequestDelegate OnStartLifeRequest;

    public delegate void StopLifeRequestDelegate();
    public static event StopLifeRequestDelegate OnStopLifeRequest;

    public delegate void NewCellsCreatedDelegate();
    public static event NewCellsCreatedDelegate OnNewCellsCreated;

    public static void EnterToEditModeRequest()
    {
        if (OnEnterToEditModeRequest != null)
        {
            OnEnterToEditModeRequest();
        }
    }

    public static void ExitFromEditModeRequest()
    {
        if (OnExitFromEditModeRequest != null)
        {
            OnExitFromEditModeRequest();
        }
    }

    public static void UpdateConfigurationRequest()
    {
        if (OnUpdateConfigurationRequest != null)
        {
            OnUpdateConfigurationRequest();
        }
    }

    public static void SetLiveCellInEditMode(Box deadBox)
    {
        if (OnSetLiveCellInEditMode != null)
        {
            OnSetLiveCellInEditMode(deadBox);
        }
    }

    public static void SetDeadCellInEditMode(Cell cellToSetDead)
    {
        if (OnSetDeadCellInEditMode != null)
        {
            OnSetDeadCellInEditMode(cellToSetDead);
        }
    }

    public static void NewLifeInitializingRequest()
    {
        if (OnNewLifeInitializingRequest != null)
        {
            OnNewLifeInitializingRequest();
        }
    }

    public static void StartLifeRequest()
    {
        if (OnStartLifeRequest != null)
        {
            OnStartLifeRequest();
        }
    }
    
    public static void StopLifeRequest()
    {
        if (OnStopLifeRequest != null)
        {
            OnStopLifeRequest();
        }
    }

    public static void NewCellsCreated()
    {
        if (OnNewCellsCreated != null)
        {
            OnNewCellsCreated();
        }
    }
}
