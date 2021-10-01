using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static void RemoveAllChildren(this RectTransform trans)
    {
        foreach (RectTransform child in trans)
        {
            Object.Destroy(child.gameObject);
        }
    }

    public static void RemoveAllChildren(this Transform trans)
    {
        foreach (Transform child in trans)
        {
            Object.Destroy(child.gameObject);
        }
    }

    public static void ActivateAtIndex(this List<GameObject> listObject, int index)
    {
        foreach (var item in listObject)
        {
            item.SetActive(false);
        }
        listObject[index].SetActive(true);
    }

    public static void ToggleActivateAll(this List<GameObject> listObject, bool isOn)
    {
        foreach (var item in listObject)
        {
            item.SetActive(isOn);
        }
    }

    public static void ShuffleMe<T>(this IList<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;

        for (int i = list.Count - 1; i > 1; i--)
        {
            int rnd = random.Next(i + 1);

            T value = list[rnd];
            list[rnd] = list[i];
            list[i] = value;
        }
    }
}
