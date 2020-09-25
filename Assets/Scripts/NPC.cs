using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public GameObject[] uiIcon;
    public GameObject uiManager;
    public GameObject gameManager;
    public Light light;

    int questIndex = 0;
    public int npcNum;

    string[] textS;
    UIManager uiM;

    bool nextStage;

    private void Awake()
    {
        uiM = uiManager.GetComponent<UIManager>();
    }

    private void Update()
    {
        //느낌표, 물음표가 항상 카메라를 향하도록
        foreach(GameObject ui in uiIcon)
        {
            Vector3 v = Camera.main.transform.position - ui.transform.position;
            v.x = v.z = 0;
            ui.transform.LookAt(Camera.main.transform.position - v);
        }

        switch (npcNum)
        {
            case 0:
                if (questIndex == 1 && uiM.MonsterA_KillCount >= 3)
                {
                    questIndex = 2;
                    Image uit = uiIcon[1].GetComponent<Image>();
                    uit.color = new Color32(255, 217, 0, 255);
                }

                if (nextStage && !uiM.texting)
                {
                    GameManager gm = gameManager.GetComponent<GameManager>();

                    gm.SceneChange(true, 2);
                }

                break;
            case 1:
                break;
        }
       
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    //메세지 실행
    //    if (other.tag == "Player" && Input.GetMouseButtonDown(0) && !uiM.texting)
    //    {
    //        switch (questIndex)
    //        {
    //            case 0:
    //                textS = new string[] { "저 앞에 몬스터를 잡아 주겠어?", "잘 부탁 드려요" }; 
    //                uiM.textOn(textS, this.gameObject, true);
    //                break;
    //            case 1:
    //                textS = new string[] { "모든 몬스터는 세마리야, 잘 부탁해" };
    //                uiM.textOn(textS, this.gameObject, false);
    //                break;
    //            case 2:
    //                textS = new string[] { "고마워 다른 곳으로 보내줄게" };
    //                uiM.textOn(textS, this.gameObject, false);
    //                nextStage = true;
    //                break;
    //            case 3:
    //                break;
    //        }
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        light.enabled = true;
        uiM.NPC_In(this.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        light.enabled = false;
        uiM.NPC_Out();
    }

    //느낌표 -> 물음표
    public void IconChange()
    {
        uiIcon[0].SetActive(false);
        uiIcon[1].SetActive(true);
    }

    public void QuestIndexUp()
    {
        questIndex++;
    }

    public void MouseDown()
    {
        if (!uiM.texting)
        {
            if(npcNum == 0)
            switch (questIndex)
            {
                case 0:
                    textS = new string[] { "저 앞에 몬스터를 잡아 주겠어?", "잘 부탁 드려요" };
                    uiM.textOn(textS, true, false);
                    break;
                case 1:
                    textS = new string[] { "모든 몬스터는 세마리야, 잘 부탁해" };
                    uiM.textOn(textS, false, false);
                    break;
                case 2:
                    textS = new string[] { "고마워 다른 곳으로 보내줄게" };
                    uiM.textOn(textS, false, false);
                    nextStage = true;
                    break;
            }

            if(npcNum == 1)
            {
                textS = new string[] { "이 앞은 보스가 기다리고 있어요", "물약을 든든히 준비해서 가세요" };
                uiM.textOn(textS, false, true);
            }
        }
    }
}
