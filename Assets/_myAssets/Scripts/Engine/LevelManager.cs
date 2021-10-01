using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> levelList;
    [SerializeField]
    private Transform playzone;

    private GameObject _newLevel;

    [SerializeField]
    private List<GameObject> characters;
    private int currentCharIndex = 0;

    private void OnEnable()
    {
        BusSystem.OnNewLevelLoad += HandleNewLevelLoad;
    }


    private void OnDisable()
    {
        BusSystem.OnNewLevelLoad -= HandleNewLevelLoad;
    }

    private void HandleNewLevelLoad()
    {
        if (levelList.Count == 0)
            throw new Exception("Level list is empty!");

        playzone.RemoveAllChildren();

        _newLevel = Instantiate(levelList[GameManager.Instance.currentLevel % GetLevelLenght()]);

        foreach (Transform child in _newLevel.transform)
            child.gameObject.SetActive(true);

        _newLevel.transform.SetParent(playzone);
        currentCharIndex++;
        characters.ActivateAtIndex(currentCharIndex % characters.Count);
    }

    public int GetLevelLenght()
    {
        return levelList.Count;
    }
}
