using UnityEngine;

public class Cell : MonoBehaviour
{
    public StateHandler.CellState cellState;

    public CellData cellData;

    private void Awake()
    {
        cellState = StateHandler.CellState.Dead;
        cellData = new CellData();
    }
}
