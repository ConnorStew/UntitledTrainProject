using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

enum EndingType {Bad, Good, Best}

public class EndingManager : MonoBehaviour
{
    public EndingDialog endingDialog;
    public SoundManager soundManager;

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
        switch(character)
        {
            case "Soldier":
                soundManager.PlaySound("soldier_ending");
                break;
            case "Alien":
                soundManager.PlaySound("alien_ending");
                break;
            case "Hunter":
                soundManager.PlaySound("hunter_ending");
                break;
        }

        return File.ReadAllText($"Assets/{character}/{type}.txt");
    }

    public void Quit()
    {
        Application.Quit();
    }

}