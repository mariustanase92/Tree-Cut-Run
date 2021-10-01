using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Sprite soundOffSprite;
    public Sprite soundOnSprite;
    public Image soundIcon;
    public Image vibrateIcon;

    private bool isVibrationOn;
    private bool isSoundOn;

    private void Start()
    {
        if (PlayerPrefs.GetInt("vibrationOptions") == 1)
        {
            isVibrationOn = false;
            vibrateIcon.sprite = soundOffSprite;
            GameManager.Instance.canVibrate = false;
        }
        else
        {
            isVibrationOn = true;
            GameManager.Instance.canVibrate = true;
            vibrateIcon.sprite = soundOnSprite;
        }

        if (PlayerPrefs.GetInt("soundOptions") == 1)
        {
            isSoundOn = false;
            soundIcon.sprite = soundOffSprite;
            AudioListener.volume = 0;
        }
        else
        {
            isSoundOn = true;
            AudioListener.volume = 1;
            soundIcon.sprite = soundOnSprite;
        }
    }

    public void ChangeSoundOptions()
    {
        if (isSoundOn)
        {
            isSoundOn = false;
            soundIcon.sprite = soundOffSprite;
            PlayerPrefs.SetInt("soundOptions", 1);
            AudioListener.volume = 0;
        }
        else
        {
            isSoundOn = true;
            soundIcon.sprite = soundOnSprite;
            AudioListener.volume = 1;
            PlayerPrefs.SetInt("soundOptions", 0);
        }
        PlayerPrefs.Save();
    }

    public void ChangeVibrateOptions()
    {
        if (isVibrationOn)
        {
            isVibrationOn = false;
            vibrateIcon.sprite = soundOffSprite;
            GameManager.Instance.canVibrate = false;
            PlayerPrefs.SetInt("vibrationOptions", 1);
        }
        else
        {
            isVibrationOn = true;
            vibrateIcon.sprite = soundOnSprite;
            GameManager.Instance.canVibrate = true;
            PlayerPrefs.SetInt("vibrationOptions", 0);
        }
        PlayerPrefs.Save();
    }
}
