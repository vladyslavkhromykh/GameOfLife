using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LifeController lifeController;

    private Camera _camera;
    private Transform _cameraTransform;
    private Vector3 _pointForRotation;

    [Range (5.0f, 15.0f)]
    public float sensivity;
    private KeyCode configSaveButton;
    private RaycastHit _hit;
    private Ray _ray;
    private Box _box;

    private void Awake()
    {
        Initialize();

        EventManager.OnNewCellsCreated += ResetCameraPosition;
    }

    private void OnDestroy()
    {
        EventManager.OnNewCellsCreated -= ResetCameraPosition;
    }

    private void Initialize()
    {
        _camera = GetComponent<Camera>();
        _cameraTransform = GetComponent<Transform>();
        configSaveButton = Config.buttonForSaveConfiguration;
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            RotateCamera();
        }

        else if (Input.GetMouseButtonDown(0))
        {
            CastForBoxInEditMode();
        }

        if (Input.GetKeyDown(configSaveButton))
        {
            UpdateConfifuration();
        }

    }

    private void CastForBoxInEditMode()
    {
        _ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(_ray, out _hit))
        {
            if (StateHandler.Instance.editState == StateHandler.EditState.Edit)
            {
                _box = _hit.transform.GetComponent<Box>();

                if (_box != null)
                {
                    switch (_box.boxState)
                    {
                        case StateHandler.BoxState.Dead:
                            EventManager.SetLiveCellInEditMode(_box);
                            break;

                        case StateHandler.BoxState.Live:
                            EventManager.SetDeadCellInEditMode(lifeController.boxedInEditMode[_box.gameObject]);
                            break;
                    }
                }
            }
        }
    }

    private void UpdateConfifuration()
    {
        if (StateHandler.Instance.lifeState == StateHandler.LifeState.Life)
        {
            Debug.LogWarning("You can not set new value for life game at the life process. Stop life and theb save new values");
            return;
        }

        if (StateHandler.Instance.editState == StateHandler.EditState.Edit)
        {
            Debug.LogWarning("You can not set new value for life game at the edit process. Apply changes and then save new values.");
            return;
        }

        EventManager.UpdateConfigurationRequest();
    }
    
    private void RotateCamera()
    {
        _cameraTransform.RotateAround(_pointForRotation, Vector3.up, Input.GetAxis("Vertical") * sensivity * Time.deltaTime);
    }
    
    private void SetNewPointForCameraRotation()
    {
        _pointForRotation = lifeController.positions[
            lifeController.config.size / 2,
            lifeController.config.size / 2,
            lifeController.config.size / 2];
    }

    private void MoveCameraToNewPosition()
    {
        _cameraTransform.position = new Vector3(_pointForRotation.x * 2f, _pointForRotation.y * 3f, -_pointForRotation.z * 2f);
        _cameraTransform.LookAt(_pointForRotation);
    }

    private void ResetCameraPosition()
    {
        SetNewPointForCameraRotation();
        MoveCameraToNewPosition();
    }
}
