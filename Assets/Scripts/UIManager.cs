using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    GameObject npcQ;

    Text text;
    string[] messageIn;
    int textIndex = 0;
    int messageIndex = 0;

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
    }

    private void Update()
    {
        
        
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
    public void textOn(string[] message, bool select)
    {
        Player p = player.GetComponent<Player>();

        textBool = select;

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
        if (texting && messageIndex != messageIn.Length - 2 && text.text == messageIn[messageIndex])
        {
            texting = false;
            textBar.SetActive(false);
            textIndex = 0;
            text.text = "";
            textButton.SetActive(false);

            Player p = player.GetComponent<Player>();

            p.PlayerStop(false);
        }
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

}
