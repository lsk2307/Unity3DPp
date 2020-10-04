using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
    public GameObject weapon1;
    public GameObject weapon2;
    Vector3 w1;
    Vector3 w2;

    RectTransform r1;
    RectTransform r2;

    public GameObject textBar;
    public GameObject player;
    public GameObject textButton;
    public GameObject startUI;
    public GameObject shopUI;
    GameObject npcQ;

    public Text goldText;
    public Text potionCount;

    Text text;
    string[] messageIn;
    int textIndex = 0;
    int messageIndex = 0;

    bool shopping;
    bool textBool;
    bool swap;
    public bool texting;
    public bool npc;

    public int MonsterA_KillCount;

    void Awake()
    {
        r1 = weapon1.GetComponent<RectTransform>();
        r2 = weapon2.GetComponent<RectTransform>();

        w1 = r1.anchoredPosition;
        w2 = r2.anchoredPosition;

        text = textBar.GetComponentInChildren<Text>();
        text.text = "";

        if(startUI.activeSelf == true)
        Time.timeScale = 0;
    }

    //무기 스왑 UI
    public void WeaponSwap()
    {
        if(!swap)
        {
            r1.anchoredPosition = w2;
            weapon1.transform.SetSiblingIndex(1);
            r2.anchoredPosition = w1;
            weapon1.transform.SetSiblingIndex(0);
            swap = true;
        }
        else
        {
            r1.anchoredPosition = w1;
            weapon1.transform.SetSiblingIndex(0);
            r2.anchoredPosition = w2;
            weapon1.transform.SetSiblingIndex(1);
            swap = false;
        }
    }

    //텍스트바 키기
    public void textOn(string[] message, bool select, bool shop)
    {
        Player p = player.GetComponent<Player>();

        textBool = select;
        shopping = shop;

        p.PlayerStop(true);

        texting = true;
        textBar.SetActive(true);

        messageIndex = 0;
        messageIn = message;

        Invoke("Texting", 0.05f);
    }

    //텍스트바 실행
    void Texting()
    {
        if (text.text == messageIn[messageIndex])
        {
            if (messageIndex == messageIn.Length - 2 && textBool)
            textButton.SetActive(true);
            return;
        }

        if (texting)
        {
            text.text += messageIn[messageIndex][textIndex];

            textIndex++;
        }

        Invoke("Texting", 0.05f);
    }


    //텍스트 선택지 "예"
    public void textClickYes()
    {
        NPC npcS = npcQ.GetComponent<NPC>();
        npcS.IconChange();

        textIndex = 0;
        text.text = "";
        textButton.SetActive(false);
        messageIndex++;

        npcS.QuestIndexUp();
        Invoke("Texting", 0.05f);
    }

    //텍스트 선택지 "아니요"
    public void textClickNo()
    {
        texting = false;
        textBar.SetActive(false);
        textIndex = 0;
        text.text = "";
        textButton.SetActive(false);

        Player p = player.GetComponent<Player>();

        p.PlayerStop(false);
    }

    public void TextEnd()
    {
        if (texting && messageIndex != messageIn.Length - 2 && text.text == messageIn[messageIndex] && !shopping)
        {
            texting = false;
            textBar.SetActive(false);
            textIndex = 0;
            text.text = "";
            textButton.SetActive(false);

            Player p = player.GetComponent<Player>();

            p.PlayerStop(false);
           
        }
        else if (texting && text.text == messageIn[messageIndex] && shopping)
        {
            if(messageIndex == messageIn.Length - 1)
            {
                texting = false;
                textBar.SetActive(false);
                textIndex = 0;
                text.text = "";

                ShopOn();
            }
            else
            {
                textIndex = 0;
                text.text = "";
                messageIndex++;

                Invoke("Texting", 0.05f);
            }
        }
    }

    void ShopOn()
    {
        shopUI.SetActive(true);
    }

    public void ShopOff()
    {
        shopUI.SetActive(false);

        Player p = player.GetComponent<Player>();

        p.PlayerStop(false);
    }

    public void Buy()
    {
        Player p = player.GetComponent<Player>();

        if (p.gold >= 100)
        {
            p.gold -= 100;
            p.potion += 1;
            GetGold();

            SetPotion();
        }
    }

    public void UsePotion()
    {
        Player p = player.GetComponent<Player>();

        if (p.allStop) return;

        if (p.potion > 0 && p.currentHp < p.hpMax)
        {
            p.potion -= 1;
            p.currentHp += 50;
            p.HPCheck();
            SetPotion();
        }
    }

    public void SetPotion()
    {
        Player p = player.GetComponent<Player>();
        potionCount.text = p.potion.ToString();
    }

    public void NPC_In(GameObject npcs)
    {
        npc = true;
        npcQ = npcs;
    }

    public void NPC_Out()
    {
        npc = false;
        npcQ = null;
    }

    public void NPC_Click()
    {
        if (!npc) return;

        NPC npcS = npcQ.GetComponent<NPC>();
        npcS.MouseDown();
    }
    public void GameEnd()
    {
        Application.Quit();
    }

    public void ReturnMain()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void StartUIFalse()
    {
        startUI.SetActive(false);
        Time.timeScale = 1;
    }

    public void GetGold()
    {
        Player p = player.GetComponent<Player>();
        goldText.text = p.gold + "G";
    }
}
