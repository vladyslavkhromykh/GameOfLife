using UnityEngine;

public static class Config
{
    public static string editModeButtonEditModeText = "Apply";
    public static string editModeButtonDefaultText = "Edit Life";

    public static string startLifeButtonLifeModeText = "Stop";
    public static string startLifeButtonDefaultModeText = "Start";

    public static string emptyCellPrefabPath = "Prefabs/EmptyCell";
    public static string liveBoxPrefabPath = "Prefabs/LiveBox";
    public static string deadBoxPrefabPath = "Prefabs/DeadBox";

    public static KeyCode buttonForSaveConfiguration = KeyCode.Return;

    public static float cellsDistance = 2.0f;

    public static int defaultSize = 7;
    public static int defaultR1 = 3;
    public static int defaultR2 = 4;
    public static int defaultR3 = 3;
    public static int defaultR4 = 2;
    public static int defaultDelay = 1000;

    public static int minSize = 3;
    public static int maxSize = 25;
}
