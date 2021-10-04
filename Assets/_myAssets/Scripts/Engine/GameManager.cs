using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("GameManager");
                    _instance = container.AddComponent<GameManager>();
                }
            }

            return _instance;
        }
    }

    //Vars
    [HideInInspector] public int currentLevel;
    [HideInInspector] public int phase1Reward = 50;
    [HideInInspector] public int phase2Multiplier = 30;
    [HideInInspector] public int cash = 0;
    [HideInInspector] public Vector3 worldGravity = new Vector3(0, -35.0F, 0);
    [HideInInspector] public bool canVibrate;

    //Refs
    public WorldSO worldDataSO;
    public LevelManager levelMan;

    private void OnEnable()
    {
        GetWorldSOData();

        if (PlayerPrefs.HasKey(Constants.CURRENT_LEVEL))
        {
           currentLevel = PlayerPrefs.GetInt(Constants.CURRENT_LEVEL);
        }
        if (PlayerPrefs.HasKey(Constants.CURRENT_CASH))
        {
           // cash = PlayerPrefs.GetInt(Constants.CURRENT_CASH);
        }

        BusSystem.CallUpdateCoins(cash);
        BusSystem.OnNewLevelStart += StartGame;
        BusSystem.OnLevelDone += HandleLevelDone;
        BusSystem.OnAddCash += AddCash;
    }

    private void OnDisable()
    {
        BusSystem.OnNewLevelStart -= StartGame;
        BusSystem.OnLevelDone -= HandleLevelDone;
        BusSystem.OnAddCash -= AddCash;
    }

    private void Start()
    {
        GameAnalyticsSDK.GameAnalytics.Initialize();

        BusSystem.CallNewLevelLoad();

        Physics.gravity = worldGravity;
    }

    internal void StartGame()
    {
        GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Start, 
            string.Format("{0}", currentLevel % levelMan.GetLevelLenght()));
    }

    private void HandleLevelDone(bool isWin)
    {
        GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(isWin ? 
            GameAnalyticsSDK.GAProgressionStatus.Complete : GameAnalyticsSDK.GAProgressionStatus.Fail, 
            string.Format("{0}", currentLevel % levelMan.GetLevelLenght()));
    }

    public void AddCash(int value)
    {
        cash += value;
        cash = Mathf.Clamp(cash, 0, cash);

        PlayerPrefs.SetInt(Constants.CURRENT_CASH, cash);
        PlayerPrefs.Save();
        BusSystem.CallUpdateCoins(cash);
    }

    public GameObject GetCurrentPlayzone()
    {
       return levelMan.GetCurrentPlayZone();
    }

    public void AdvanceLevel()
    {
        currentLevel++;

        if(currentLevel % levelMan.GetLevelLenght() == 0)
            currentLevel = 0;

        PlayerPrefs.SetInt(Constants.CURRENT_LEVEL, currentLevel);
        PlayerPrefs.Save();
    }

    void GetWorldSOData()
    {
        currentLevel = worldDataSO.currentLevel;
        phase1Reward = worldDataSO.phase1Reward;
        phase2Multiplier = worldDataSO.phase2Multiplier;
        cash = worldDataSO.cash;
        worldGravity = worldDataSO.worldGravity;
        canVibrate = worldDataSO.canVibrate;
    }
}
