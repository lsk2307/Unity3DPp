using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    public GameObject[] puzzle;
    public GameObject gameM;
    public Material colorThis;
    int num;
    public bool on = false;

    GameManager gameManager;

    public bool puzzleEnd;

    Color32 red = new Color32(255, 0, 0, 120);
    Color32 green = new Color32(0, 255, 0, 120);

    void Awake()
    {
        colorThis = GetComponent<MeshRenderer>().material;
        gameManager = gameM.GetComponent<GameManager>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (puzzleEnd) return;

        for (int i = 0; i < puzzle.Length; ++i)
        {
            if(puzzle[i] == gameObject)
            {
                num = i;

                break;
            }
        }

        colorThis.color = !on ? green : red;
        on = !on ? true : false;
        gameManager.puzzleCount += on ? 1 : -1;


        if (num % 3 != 0)
        {
            Puzzle pz = puzzle[num - 1].GetComponent<Puzzle>();

            pz.colorThis.color = !pz.on ? green : red;
            pz.on = !pz.on ? true : false;
            gameManager.puzzleCount += pz.on ? 1 : -1;
        }

        if (num % 3 != 2)
        {
            Puzzle pz = puzzle[num + 1].GetComponent<Puzzle>();
            pz.colorThis.color = !pz.on ? green : red;
            pz.on = !pz.on ? true : false;
            gameManager.puzzleCount += pz.on ? 1 : -1;
        }

        if (num / 3 != 0)
        {
            Puzzle pz = puzzle[num - 3].GetComponent<Puzzle>();
            pz.colorThis.color = !pz.on ? green : red;
            pz.on = !pz.on ? true : false;
            gameManager.puzzleCount += pz.on ? 1 : -1;
        }

        if (num / 3 < 2)
        {
            Puzzle pz = puzzle[num + 3].GetComponent<Puzzle>();
            pz.colorThis.color = !pz.on ? green : red;
            pz.on = !pz.on ? true : false;
            gameManager.puzzleCount += pz.on ? 1 : -1;
        }

        if(gameManager.puzzleCount >= 9)
        {
            gameManager.PuzzleFinish();

            foreach(GameObject pz in puzzle)
            {
                Puzzle pzS = pz.GetComponent<Puzzle>();
                pzS.puzzleEnd = true;
            }
        }
    }
}
