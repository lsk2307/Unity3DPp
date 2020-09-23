using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour
{
    public Transform stick;
    public GameObject player;
    Player p;

    Vector3 originPos;
    Vector3 joyVec;
    float radius;

    bool moving;



    // Start is called before the first frame update
    void Start()
    {
        radius = GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        originPos = stick.transform.position;

        float Can = transform.parent.GetComponent<RectTransform>().localScale.x;
        radius *= Can;

        p = player.GetComponent<Player>();
    }

    private void Update()
    {
        if(moving)
        {
            p.JoyMove(new Vector3(0, Mathf.Atan2(joyVec.x, joyVec.y) * Mathf.Rad2Deg, 0));
        }

    }

    // Update is called once per frame
    public void Drag(BaseEventData _Data)
    {
        moving = true;

        PointerEventData Data = _Data as PointerEventData;
        Vector3 Pos = Data.position;

        joyVec = (Pos - originPos).normalized;

        float Dis = Vector3.Distance(Pos, originPos);
        
        if (Dis < radius)
        {
            stick.position = originPos + joyVec * Dis;
        }
        else
        {
            stick.position = originPos + joyVec * radius;
        }
    }

    public void DragEnd()
    {
        stick.position = originPos;
        joyVec = Vector3.zero;
        p.MoveAnimOff();
        moving = false;
    }
}
