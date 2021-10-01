using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject startButton;
    [SerializeField]
    private GameObject settingsButton;
    [SerializeField]
    private GameObject mainMenuPanel;
    [SerializeField]
    private GameObject endScreenWin;
    [SerializeField]
    private GameObject endScreenLose;
    [SerializeField]
    private GameObject continueButton;
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI _cutTreesText;

    //Coins
    [SerializeField]
    private GameObject coinToSpawn;
    [SerializeField]
    private Transform coinContainer;
    [SerializeField]
    private Transform coinSpawnOrigin;
    [SerializeField]
    private TextMeshProUGUI coinsText;

    private void OnEnable()
    {
        BusSystem.OnLevelDone += HandleLevelDone;
        BusSystem.OnUpdateCoins += HandleUpdateCoins;
        BusSystem.OnPhaseOneEnd += EnableCutTreesText;
        BusSystem.OnTreeChopped += CollectCash;
    }

    private void OnDisable()
    {  
        BusSystem.OnLevelDone -= HandleLevelDone;
        BusSystem.OnUpdateCoins -= HandleUpdateCoins;
        BusSystem.OnPhaseOneEnd -= EnableCutTreesText;
        BusSystem.OnTreeChopped -= CollectCash;
    }

    private void Start()
    {
        levelText.text = string.Format("Level {0}", GameManager.Instance.currentLevel + 1);
    }

    public void StartLevel()
    {
        BusSystem.CallNewLevelStart();
        mainMenuPanel.SetActive(false);
    }

    public void LevelEndContinue()
    {
        endScreenWin.SetActive(false);
        endScreenLose.SetActive(false);
        mainMenuPanel.SetActive(true);

        BusSystem.CallNewLevelLoad();
        levelText.text = string.Format("Level {0}", GameManager.Instance.currentLevel + 1);
    }

    private void SpawnCoin(Vector3 pos)
    {
        GameObject _newCoin = Instantiate(coinToSpawn);
        _newCoin.GetComponent<RectTransform>().position = pos;
        _newCoin.GetComponent<RectTransform>().localScale = Vector3.one * 1.5f;
        _newCoin.transform.SetParent(coinContainer);
        _newCoin.GetComponent<Coin>().InitEndLevel(coinsText.GetComponent<RectTransform>());
    }

    private void HandleLevelDone(bool isWin)
    {
        if (isWin)
        {
            _cutTreesText.enabled = false;
            endScreenWin.SetActive(true);
            StartCoroutine(DelayContinueButton());
           // BusSystem.CallAddCash(100);
            //for (int i = 0; i < 10; i++)
            //{
            //    SpawnCoin(coinSpawnOrigin.transform.position);
            //}
        }
        else
        {
            endScreenLose.SetActive(true);
        }
    }

    private void HandleUpdateCoins(int value)
    {
        coinsText.text = value.ToString();
    }

    IEnumerator DelayContinueButton()
    {
        continueButton.SetActive(false);
        yield return new WaitForSeconds(5f);
        continueButton.SetActive(true);
    }

    void EnableCutTreesText()
    {
        _cutTreesText.enabled = true;
        FunctionTimer.Create(() => _cutTreesText.enabled = false, 4f);
    }

    void CollectCash()
    {
        BusSystem.CallAddCash(30);
        for (int i = 0; i < 3; i++)
        {
            SpawnCoin(coinSpawnOrigin.transform.position);
        }
    }
}
