using System.IO;
using UnityEngine;

/// <summary> The types of endings. </summary>
enum EndingType {Bad, Good, Best}

public class EndingManager : MonoBehaviour
{
    /// <summary> The dialog manager for the ending scene. </summary>
    public EndingDialog endingDialog;

    /// <summary> The sound manager for the ending scene. </summary>
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

    /// <summary>
    /// Reads the ending from file for the given character.
    /// </summary>
    /// <param name="character">The character's name.</param>
    /// <param name="type">The type of ending.</param>
    /// <returns>The ending for the given character and type.</returns>
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

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
}