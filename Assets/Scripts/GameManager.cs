using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject setting;
    public GameObject portal;
    public Slider slider;
    public UIManager uiM;
    AudioSource audio;

    public int puzzleCount;

    private void Awake()
    {
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {

            PlayerPrefs.DeleteKey("Level");
            PlayerPrefs.DeleteKey("MaxHP");
            PlayerPrefs.DeleteKey("HP");
            PlayerPrefs.DeleteKey("Gold");
            PlayerPrefs.DeleteKey("Atk");
            PlayerPrefs.DeleteKey("Potion");


            uiM.SetPotion();
        }

        audio = GetComponent<AudioSource>();

        if (PlayerPrefs.HasKey("Volume")) audio.volume = PlayerPrefs.GetFloat("Volume");

        if (PlayerPrefs.HasKey("Level"))
        {
            CharaLoad();
            uiM.GetGold();
            uiM.SetPotion();
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            if(puzzleCount == 9)
            {
                puzzleCount++;
                PuzzleFinish();
            }
        }
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
    }

    public void CharaLoad()
    {
        Player p = player.GetComponent<Player>();

        p.level = PlayerPrefs.GetInt("Level");
        p.hpMax = PlayerPrefs.GetInt("MaxHP");
        p.currentHp = PlayerPrefs.GetInt("HP");
        p.gold = PlayerPrefs.GetInt("Gold");
        p.atk = PlayerPrefs.GetInt("Atk");
        p.potion = PlayerPrefs.GetInt("Potion");

        p.HpBar.fillAmount = (float)p.currentHp / p.hpMax;
    }

    public void CharaSave()
    {
        Player p = player.GetComponent<Player>();

        PlayerPrefs.SetInt("Level", p.level);
        PlayerPrefs.SetInt("MaxHP", p.hpMax);
        PlayerPrefs.SetInt("HP", p.currentHp);
        PlayerPrefs.SetInt("Gold", p.gold);
        PlayerPrefs.SetInt("Atk", p.atk);
        PlayerPrefs.SetInt("Potion", p.potion);
    }

    public void SceneChange(bool save,int index)
    {
        if (save) CharaSave();
        SceneManager.LoadScene(index);
    }

    public void SoundSettingIn()
    {
        Time.timeScale = 0;
        setting.SetActive(true);
        slider.value = audio.volume;
    }

    public void SoundSetting()
    {
        audio.volume = slider.value;
    }

    public void SoundSettingOut()
    {
        PlayerPrefs.SetFloat("Volume", slider.value);
        setting.SetActive(false);
        Time.timeScale = 1;
    }

    public void PuzzleFinish()
    {
        portal.SetActive(true);
    }
}
