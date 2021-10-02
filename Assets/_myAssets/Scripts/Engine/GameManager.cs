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

    [HideInInspector]
    public bool canVibrate;
    public int currentLevel;
    public LevelManager levelMan;
    int cash = 0;

    private void OnEnable()
    {
        if (PlayerPrefs.HasKey(Constants.CURRENT_LEVEL))
        {
           // currentLevel = PlayerPrefs.GetInt(Constants.CURRENT_LEVEL);
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
    }

    internal void StartGame()
    {
        GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Start, string.Format("{0}", currentLevel % levelMan.GetLevelLenght()));
    }

    private void HandleLevelDone(bool isWin)
    {
        GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(isWin ? GameAnalyticsSDK.GAProgressionStatus.Complete : GameAnalyticsSDK.GAProgressionStatus.Fail, string.Format("{0}", currentLevel % levelMan.GetLevelLenght()));

        if (isWin)
        {
            currentLevel++;

            if (currentLevel >= levelMan.GetLevelLenght())
                currentLevel = 0;


             PlayerPrefs.SetInt(Constants.CURRENT_LEVEL, currentLevel);
             PlayerPrefs.Save();
        }
        else
        {

        }
    }

    public void AddCash(int value)
    {
        cash += value;

        if (cash <= 0)
            cash = 0;

        PlayerPrefs.SetInt(Constants.CURRENT_CASH, cash);
        PlayerPrefs.Save();
        BusSystem.CallUpdateCoins(cash);
    }

    void ResetLogCount()
    {
        AddCash(-100);
    }
}
