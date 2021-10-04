﻿using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //Main Screen
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject settingsButton;
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject endScreenWin;
    [SerializeField] GameObject endScreenLose;
    [SerializeField] GameObject _winButtons;
    
    //Level
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Image _cutTreesText;
    [SerializeField] Image _perfectIcon;

    //Coins
    [SerializeField] GameObject coinToSpawn;
    [SerializeField] Transform coinContainer;
    [SerializeField] Transform coinSpawnOrigin;
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] TextMeshProUGUI _coinsEarnedText;

    //Other
    [SerializeField] GameObject _tutorial;
    IEnumerator _storedCoroutine;

    private void OnEnable()
    {
        BusSystem.OnLevelDone += HandleLevelDone;
        BusSystem.OnUpdateCoins += HandleUpdateCoins;
        BusSystem.OnPhaseOneEnd += EnableCutTreesUI;
        BusSystem.OnAddCash += CollectCash;
        BusSystem.OnPerfectRound += ShowPerfectIcon;
        BusSystem.OnNewLevelLoad += HidePerfectIcon;
        BusSystem.OnNewLevelStart += ShowTutorial;
    }

    private void OnDisable()
    {
        BusSystem.OnLevelDone -= HandleLevelDone;
        BusSystem.OnUpdateCoins -= HandleUpdateCoins;
        BusSystem.OnPhaseOneEnd -= EnableCutTreesUI;
        BusSystem.OnAddCash -= CollectCash;
        BusSystem.OnPerfectRound -= ShowPerfectIcon;
        BusSystem.OnNewLevelLoad -= HidePerfectIcon;
        BusSystem.OnNewLevelStart -= ShowTutorial;
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

    public void RetryLevel()
    {
        CloseEndScreen();
    }

    public void AdvanceLevel()
    {
        GameManager.Instance.AdvanceLevel();
        CloseEndScreen();
    }

    void CloseEndScreen()
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
        _winButtons.SetActive(false);
        yield return new WaitForSeconds(5f);
        _winButtons.SetActive(true);
    }

    void EnableCutTreesUI()
    {
        _cutTreesText.enabled = true;
        Invoke("DisableCutTreesUI", 3f);
    }

    void DisableCutTreesUI()
    {
        _cutTreesText.enabled = false;
    }

    void CollectCash(int treeHP)
    {
        BusSystem.CallSoundPlay(SoundEffects.PickIngot);

        for (int i = 0; i < 10; i++)
        {
            SpawnCoin(coinSpawnOrigin.transform.position);
        }

        ShowCoinsEarnedText(treeHP);
    }

    void ShowCoinsEarnedText(int amount)
    {
        _coinsEarnedText.enabled = false;

        if (_storedCoroutine != null)
            StopCoroutine(_storedCoroutine);

        _storedCoroutine = HideCoinsEarnedText();
        _coinsEarnedText.enabled = true;
        _coinsEarnedText.text = $"+{amount}";

        if (amount >= 100)
        {
            _coinsEarnedText.color = Color.green;
            _coinsEarnedText.enableAutoSizing = false;
            _coinsEarnedText.fontSize = 80;
        }
        else
        {
            _coinsEarnedText.color = Color.white;
            _coinsEarnedText.enableAutoSizing = true;
            _coinsEarnedText.fontSize = 50;
        }
           

        StartCoroutine(_storedCoroutine);
    }

    IEnumerator HideCoinsEarnedText()
    {
        yield return new WaitForSeconds(2.5f);
        _coinsEarnedText.enabled = false;
    }

    void ShowPerfectIcon()
    {
        _perfectIcon.gameObject.SetActive(true);
    }

    void HidePerfectIcon()
    {
        _perfectIcon.gameObject.SetActive(false);
    }

    void ShowTutorial()
    {
        _tutorial.SetActive(true);

        Invoke("HideTutorial", 3);
    }

    void HideTutorial()
    {
        _tutorial.SetActive(false);
    }
}