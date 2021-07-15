using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera cam;
    public DialogueManager dialogueManager;

    private Character soldier, alien, hero;

    private Character currentCharacter;

    void Start()
    {
        soldier = new Character(dialogueManager, "Soldier", "Assets/Soldier_Main.json", "Assets/Soldier_Side.json");
        //alien = new Character(dialogueManager, "Alien", "Assets/Alien_Main.json", "Assets/Alien_Side.json");
        hero = new Character(dialogueManager, "Hero", "Assets/Hero_Main.json", "Assets/Hero_Side.json");

        ChangeCabin("Soldier");
    }
    
    public void ChangeCabin(string character)
    {
        switch (character)
        {
            case "Soldier":
                cam.transform.position = new Vector3(-2000, 50, -100);
                SetCharacter(soldier);
                break;
            case "Alien":
                break;
            case "Hero":
                cam.transform.position = new Vector3(-3000, 70, -200);
                SetCharacter(hero);
                break;
            default:
                break;
        }

    }

    private void SetCharacter(Character character)
    {
        currentCharacter = character;
        character.DisplayConversation();
    }

    public void ContinueConversation()
    {
        currentCharacter.ContinueConversation();
    }

    internal void WordClicked(string lastClickedWord)
    {
        currentCharacter.WordClicked(lastClickedWord);
    }
}
