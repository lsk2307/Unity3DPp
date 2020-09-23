using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

public class Main : MonoBehaviour
{
    public GameObject[] menus;
    public Slider volumeS;
    public AudioSource audio;

    Image im;
    public Sprite[] sp;
    Sprite secondSp;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Volume") && gameObject.tag == "BGM") audio.volume = PlayerPrefs.GetFloat("Volume");

        if(gameObject.tag == "Menu")
        {
            im = GetComponent<Image>();
            secondSp = sp[1];
        }
    }

    public void ImageChange()
    {
        Sprite originSp = im.sprite;
        im.sprite = secondSp;
        secondSp = originSp;
    }

    public void GameStart()
    {
        SceneManager.LoadScene(1);
    }

    public void Setting()
    {
        ImageChange();
        for (int i = 0; i < 3; i++)
        {
            menus[i].SetActive(false);
        }
        menus[3].SetActive(true);

        volumeS.value = audio.volume;
    }

    public void SettingEnd()
    {
        PlayerPrefs.SetFloat("Volume", volumeS.value);
        menus[3].SetActive(false);
        for (int i = 0; i < 3; i++)
        {
            menus[i].SetActive(true);
        }
    }

    public void SoundChange()
    {
        audio.volume = volumeS.value;
    }

    public void GameEnd()
    {
        Application.Quit();
    }
}
