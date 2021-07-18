using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

enum EndingType {Bad, Good, Best}

public class EndingManager : MonoBehaviour
{
    public EndingDialog endingDialog;

    void Start()
    {
        string character = PlayerPrefs.GetString("character");
        int points = PlayerPrefs.GetInt("points");

        if (points > 9)
        {
            endingDialog.DisplaySentence(GetEnding(character, EndingType.Best));
        }
        else if (points > 4)
        {
            endingDialog.DisplaySentence(GetEnding(character, EndingType.Good));
        }
        else
        {
            endingDialog.DisplaySentence(GetEnding(character, EndingType.Bad));
        }
    }

    private string GetEnding(string character, EndingType type)
    {
        return File.ReadAllText($"Assets/{character}/{type}.txt");
    }

    public void Quit()
    {
        Application.Quit();
    }

}