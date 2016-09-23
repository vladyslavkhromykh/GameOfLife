using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LifeController : MonoBehaviour
{
    [Tooltip("Set flag for non-limited neighbors of cell")]
    public bool periodic;

    public Cell[,,] cells;
    public Vector3[,,] positions;

    public Transform cellsParent;
    public Transform liveBoxesParent;
    public Transform deadBoxedParent;

    private GameObject _emptyCellPrefab;
    private GameObject _liveBoxPrefab;
    private GameObject _deadBoxPrefab;

    public UIController uiController;

    public Dictionary<Cell, GameObject> cellsAndBoxes;
    public Dictionary<GameObject, Cell> boxedInEditMode;
    private List<Cell> _cellsToLive;
    private List<Cell> _cellsToDie;

    public class Configuration
    {
        public int size;
        public int r1, r2, r3, r4;
        public int delay;
    }
    public Configuration config;

    private Configuration preConfig;

    private delegate List<Cell> Neighbors(Cell cell);
    private Neighbors _neighbors;

    private void Awake()
    {
        Initialize();
        
        EventManager.OnNewLifeInitializingRequest += NewLifeInitializing;
        EventManager.OnStartLifeRequest += StartLife;
        EventManager.OnStopLifeRequest += StopLife;
        EventManager.OnUpdateConfigurationRequest += UpdatePreConfiguration;
        EventManager.OnEnterToEditModeRequest += InitializeEditMode;
        EventManager.OnExitFromEditModeRequest += ExitEditMode;
        EventManager.OnSetLiveCellInEditMode += SetLiveCellInEditMode;
        EventManager.OnSetDeadCellInEditMode += SetDeadCellInEditMode;
    }

    private void Start()
    {
        NewLifeInitializing();
    }

    private void OnDestroy()
    {
        EventManager.OnNewLifeInitializingRequest -= NewLifeInitializing;
        EventManager.OnStartLifeRequest -= StartLife;
        EventManager.OnStopLifeRequest -= StopLife;
        EventManager.OnUpdateConfigurationRequest -= UpdatePreConfiguration;
        EventManager.OnEnterToEditModeRequest -= InitializeEditMode;
        EventManager.OnExitFromEditModeRequest -= ExitEditMode;
        EventManager.OnSetLiveCellInEditMode -= SetLiveCellInEditMode;
        EventManager.OnSetDeadCellInEditMode -= SetDeadCellInEditMode;
    }

    #region Init

    private void Initialize()
    {
        InitializeNeighborsMethod();
        InitializeStorages();
        InitializeConfigs();
        InitializePrefabs();
    }

    private void InitializeNeighborsMethod()
    {
        if (periodic)
        {
            _neighbors = GetNeighborsWithoutLimits;
        }
        else
        {
            _neighbors = GetNeighborsWithLimits;
        }
    }

    private void InitializeStorages()
    {
        cellsAndBoxes = new Dictionary<Cell, GameObject>();
        boxedInEditMode = new Dictionary<GameObject, Cell>();

        _cellsToLive = new List<Cell>();
        _cellsToDie = new List<Cell>();
    }

    private void InitializeConfigs()
    {
        preConfig = new Configuration();

        preConfig.size = Config.defaultSize;
        preConfig.size = Config.defaultSize;
        preConfig.r1 = Config.defaultR1;
        preConfig.r2 = Config.defaultR2;
        preConfig.r3 = Config.defaultR3;
        preConfig.r4 = Config.defaultR4;
        preConfig.delay = Config.defaultDelay;

        config = new Configuration();

        config.size = Config.defaultSize;
        config.r1 = Config.defaultR1;
        config.r2 = Config.defaultR2;
        config.r3 = Config.defaultR3;
        config.r4 = Config.defaultR4;
        config.delay = Config.defaultDelay;

    }

    private void InitializePrefabs()
    {
        _emptyCellPrefab = Resources.Load(Config.emptyCellPrefabPath) as GameObject;
        _liveBoxPrefab = Resources.Load(Config.liveBoxPrefabPath) as GameObject;
        _deadBoxPrefab = Resources.Load(Config.deadBoxPrefabPath) as GameObject;
    }

    #endregion

    #region EditMode

    private void InitializeEditMode()
    {
        CreateDeadBoxes();
    }

    private void CreateDeadBoxes()
    {
        GameObject deadBox;

        for (int i = 0; i < config.size; i++)
        {
            for (int j = 0; j < config.size; j++)
            {
                for (int k = 0; k < config.size; k++)
                {
                    if (cells[i, j, k].cellState == StateHandler.CellState.Dead)
                    {
                        deadBox = Instantiate(_deadBoxPrefab) as GameObject;
                        deadBox.transform.SetParent(deadBoxedParent);
                        deadBox.transform.position = positions[i, j, k];
                        boxedInEditMode[deadBox] = cells[i, j, k];
                    }

                    else
                    {
                        boxedInEditMode[cellsAndBoxes[cells[i, j, k]]] = cells[i, j, k];
                    }
                }
            }
        }
    }

    private void SetLiveCellInEditMode(Box box)
    {
        SetCellLive(boxedInEditMode[box.gameObject]);
        DestroyDeadBox(box);
    }

    private void DestroyDeadBox(Box box)
    {
        boxedInEditMode.Remove(box.gameObject);
        Destroy(box.gameObject);
    }

    private void SetDeadCellInEditMode(Cell cell)
    {
        CreateDeadBox(cell);
        SetCellDead(cell);
    }

    private void CreateDeadBox(Cell cell)
    {
        GameObject deadBox = Instantiate(_deadBoxPrefab) as GameObject;
        deadBox.transform.SetParent(deadBoxedParent);
        deadBox.transform.position = positions[cell.cellData.i, cell.cellData.j, cell.cellData.k];
        boxedInEditMode[deadBox] = cells[cell.cellData.i, cell.cellData.j, cell.cellData.k];
    }

    private void ExitEditMode()
    {
        DestroyDeadBoxes();
        boxedInEditMode.Clear();
    }

    private void DestroyDeadBoxes()
    {
        foreach (KeyValuePair<GameObject, Cell> pair in boxedInEditMode)
        {
            if (pair.Value.cellState == StateHandler.CellState.Dead)
            {
                Destroy(pair.Key);
            }
        }
    }

    #endregion

    #region Life

    private void NewLifeInitializing()
    {
        ClearCells();
        ClearBoxes();

        SetNewConfiguration();
        CreateCells();

        CreateStartLiveCells();
    }

    private void CreateStartLiveCells()
    {
        SetCellLive(cells[config.size / 2 - 1, config.size / 2, config.size / 2]);
        SetCellLive(cells[config.size / 2, config.size / 2, config.size / 2]);
        SetCellLive(cells[config.size / 2 + 1, config.size / 2, config.size / 2]);
    }

    private void StartLife()
    {
        StartCoroutine("Life");
    }

    private void StopLife()
    {
        StopCoroutine("Life");
    }

    private IEnumerator Life()
    {
        while (true)
        {
            LifeGeneration();

            yield return new WaitForSeconds(config.delay / 1000f);
        }
    }

    private void LifeGeneration()
    {
        DefineLifeSolutions();
        MakeCellsSolutions();
        ClearCellLists();
    }

    private void ClearCellLists()
    {
        _cellsToLive.Clear();
        _cellsToDie.Clear();
    }

    public List<Cell> GetNeighborsWithLimits(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();

        int iIndex = cell.cellData.i;
        int jIndex = cell.cellData.j;
        int kIndex = cell.cellData.k;

        for (int i = iIndex - 1; i <= iIndex + 1; i++)
        {
            for (int j = jIndex - 1; j <= jIndex + 1; j++)
            {
                for (int k = kIndex - 1; k <= kIndex + 1; k++)
                {

                    if (i == iIndex && j == jIndex && k == kIndex)
                        continue;

                    if (i < 0 || i >= config.size)
                        continue;
                    if (j < 0 || j >= config.size)
                        continue;
                    if (k < 0 || k >= config.size)
                        continue;

                    neighbors.Add(cells[i, j, k]);
                }
            }
        }

        return neighbors;
    }

    public List<Cell> GetNeighborsWithoutLimits(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();

        int iIndex = cell.cellData.i;
        int jIndex = cell.cellData.j;
        int kIndex = cell.cellData.k;

        for (int i = iIndex - 1; i <= iIndex + 1; i++)
        {
            for (int j = jIndex - 1; j <= jIndex + 1; j++)
            {
                for (int k = kIndex - 1; k <= kIndex + 1; k++)
                {

                    AddUnlimitedNeighbors(iIndex, jIndex, kIndex, neighbors);

                    if (i == iIndex && j == jIndex && k == kIndex)
                        continue;

                    if (i < 0 || i >= config.size)
                        continue;
                    if (j < 0 || j >= config.size)
                        continue;
                    if (k < 0 || k >= config.size)
                        continue;

                    neighbors.Add(cells[i, j, k]);
                }
            }
        }

        return neighbors;
    }

    // Adds reflected neighbors
    private void AddUnlimitedNeighbors(int i, int j, int k, List<Cell> neighborsToAdd)
    {
        if (i == 0)
        {
            if (!neighborsToAdd.Contains(cells[config.size - 1, j, k]))
                neighborsToAdd.Add(cells[config.size - 1, j, k]);
        }

        else if (i == config.size - 1)
        {
            if (!neighborsToAdd.Contains(cells[0, j, k]))
                neighborsToAdd.Add(cells[0, j, k]);
        }

        if (j == 0)
        {
            if (!neighborsToAdd.Contains(cells[i, config.size - 1, k]))
                neighborsToAdd.Add(cells[i, config.size - 1, k]);
        }

        else if (j == config.size - 1)
        {
            if (!neighborsToAdd.Contains(cells[i, 0, k]))
                neighborsToAdd.Add(cells[i, 0, k]);
        }

        if (k == 0)
        {
            if (!neighborsToAdd.Contains(cells[i, j, config.size - 1]))
                neighborsToAdd.Add(cells[i, j, config.size - 1]);
        }

        else if (k == config.size - 1)
        {
            if (!neighborsToAdd.Contains(cells[i, j, 0]))
                neighborsToAdd.Add(cells[i, j, 0]);
        }
    }

    private void MakeCellsSolutions()
    {
        foreach (Cell cell in _cellsToLive)
        {
            SetCellLive(cell);
        }

        foreach (Cell cell in _cellsToDie)
        {
            SetCellDead(cell);
        }
    }

    private void DefineLifeSolutions()
    {
        for (int i = 0; i < config.size; i++)
        {
            for (int j = 0; j < config.size; j++)
            {
                for (int k = 0; k < config.size; k++)
                {
                    DefineLifeSolutionForCell(cells[i, j, k]);
                }
            }
        }
    }

    private void DefineLifeSolutionForCell(Cell cell)
    {
        List<Cell> neighbors = _neighbors(cell);

        foreach (Cell neighbor in neighbors)
        {

            if (IsCellGoingToLive(neighbor))
            {
                if (!_cellsToLive.Contains(neighbor))
                    _cellsToLive.Add(neighbor);
            }

            else if (IsCellGoingToDead(neighbor))
            {
                if (!_cellsToDie.Contains(neighbor))
                    _cellsToDie.Add(neighbor);
            }
        }
    }

    private void SetCellLive(Cell cell)
    {
        GameObject liveBox = Instantiate(_liveBoxPrefab);
        liveBox.transform.position = positions[cell.cellData.i, cell.cellData.j, cell.cellData.k];
        cellsAndBoxes[cell] = liveBox;
        liveBox.transform.SetParent(liveBoxesParent);
        cell.cellState = StateHandler.CellState.Live;

        if (StateHandler.Instance.editState == StateHandler.EditState.Edit)
        {
            boxedInEditMode[liveBox] = cell;
        }
    }

    private void SetCellDead(Cell cell)
    {
        Destroy(cellsAndBoxes[cell].gameObject);
        cellsAndBoxes[cell] = null;
        cell.cellState = StateHandler.CellState.Dead;
    }

    private bool IsCellGoingToLive(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();
        neighbors = _neighbors(cell);

        int liveNeighbors = neighbors.Where(neighbor => neighbor.cellState == StateHandler.CellState.Live).ToList().Count;

        return (liveNeighbors >= config.r1 && liveNeighbors <= config.r2 && cell.cellState == StateHandler.CellState.Dead);
    }

    private bool IsCellGoingToDead(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();
        neighbors = _neighbors(cell);

        int deadNeighbors = neighbors.Where(neighbor => neighbor.cellState == StateHandler.CellState.Dead).ToList().Count;

        return ((deadNeighbors > config.r3 || deadNeighbors < config.r4) && cell.cellState == StateHandler.CellState.Live);
    }

    #endregion

    #region Common

    private void CreateCells()
    {
        int n = config.size;
        float cellsDistance = Config.cellsDistance;
        GameObject cell;
        positions = new Vector3[n, n, n];
        cells = new Cell[n, n, n];


        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    cell = Instantiate(_emptyCellPrefab);

                    cell.GetComponent<Cell>().cellData.i = i;
                    cell.GetComponent<Cell>().cellData.j = j;
                    cell.GetComponent<Cell>().cellData.k = k;

                    positions[i, j, k] = new Vector3(i * cellsDistance, j * cellsDistance, k * cellsDistance);
                    cell.transform.position = positions[i, j, k];
                    cell.transform.SetParent(cellsParent);
                    cells[i, j, k] = cell.GetComponent<Cell>();
                }
            }
        }

        EventManager.NewCellsCreated();
    }

    private void ClearCells()
    {
        foreach (Transform child in cellsParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void ClearBoxes()
    {
        foreach (Transform child in liveBoxesParent)
        {
            Destroy(child.gameObject);
        }
    }

    #endregion

    #region Configuration

    private void UpdatePreConfiguration()
    {
        ParseConfigurationData();
        ValidateValues();
    }

    private void ParseConfigurationData()
    {
        int preSize = Convert.ToInt32(uiController.size.text);
        int preR1 = Convert.ToInt32(uiController.r1.text);
        int preR2 = Convert.ToInt32(uiController.r2.text);
        int preR3 = Convert.ToInt32(uiController.r3.text);
        int preR4 = Convert.ToInt32(uiController.r4.text);
        int preDelay = Convert.ToInt32(uiController.delay.text);

        preConfig.size = preSize;
        preConfig.r1 = preR1;
        preConfig.r2 = preR2;
        preConfig.r3 = preR3;
        preConfig.r4 = preR4;
        preConfig.delay = preDelay;
    }

    private void ValidateValues()
    {
        if (preConfig.size <= Config.minSize && preConfig.size >= Config.maxSize)
        {
            Debug.LogWarning("Size not in valid limits. Check value that you've enter.");
            return;
        }
    }

    private void SetNewConfiguration()
    {
        config.size = preConfig.size;
        config.r1 = preConfig.r1;
        config.r2 = preConfig.r2;
        config.r3 = preConfig.r3;
        config.r4 = preConfig.r4;
        config.delay = preConfig.delay;
    }

    #endregion
}