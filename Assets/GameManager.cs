using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Camera cam;
    public DialogueManager dialogueManager;
    public SoundManager soundManager;

    public GameObject dialogPanel;
    public GameObject buttonPrefab;

    public GameObject continueButton;
    public GameObject dialogText;
    public GameObject nameText;

    private Character soldier, alien, hunter;

    public Character currentCharacter;

    private Queue<GameObject> buttons;

    internal string characterName { 
        get { return currentCharacter.name; } 
    }

    void Start()
    {
        soldier = new Character(dialogueManager, this, soundManager, "Soldier");
        alien = new Character(dialogueManager, this, soundManager, "Alien");
        hunter = new Character(dialogueManager, this, soundManager, "Hunter");

        buttons = new Queue<GameObject>();

        soundManager.PlaySound("Train_noise_inside");

        buttonPrefab.SetActive(false);
        currentCharacter = soldier;
        ChangeCabin("Soldier");
    }

    public void ContinueButtonClicked()
    {
        soundManager.PlaySound("mouse_click");
        ContinueConversation();
    }

    public void CoinClicked()
    {
        PlayerPrefs.SetString("character", characterName);
        PlayerPrefs.SetInt("points", currentCharacter.readDialogs.Count);
        SceneManager.LoadScene(2);
    }

    public void ChangeCabinButtonClicked(string character)
    {
        soundManager.PlaySound("mouse_click");
        ChangeCabin(character);
    }

    private void CharacterSound(string sound)
    {
        soundManager.StopSound("soldier_theme");
        soundManager.StopSound("hero_theme");
        soundManager.StopSound("alien_theme");

        soundManager.PlaySound(sound);
    }
    
    public void ChangeCabin(string character)
    {
        switch (character)
        {
            case "Soldier":
                cam.transform.position = new Vector3(141, 94.3f, -109.9f);

                
                if (currentCharacter.Equals(alien))
                    soundManager.PlaySound("alien_to_soldier");

                if (currentCharacter.Equals(hunter))
                    soundManager.PlaySound("hunter_to_soldier");

                CharacterSound("soldier_theme");
                SetCharacter(soldier);
                break;
            case "Alien":
                cam.transform.position = new Vector3(2060.46f, 94.3f, -109.9f);

                
                if (currentCharacter.Equals(soldier))
                    soundManager.PlaySound("soldier_to_alien");

                if (currentCharacter.Equals(hunter))
                    soundManager.PlaySound("hunter_to_alien");

                CharacterSound("alien_theme");
                SetCharacter(alien);
                break;
            case "Hunter":
                cam.transform.position = new Vector3(985.96f, 94.3f, -109.9f);

                
                if (currentCharacter.Equals(soldier))
                    soundManager.PlaySound("soldier_to_hunter");

                if (currentCharacter.Equals(alien))
                    soundManager.PlaySound("alien_to_hunter");

                CharacterSound("hero_theme");
                SetCharacter(hunter);
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

    internal void SwitchToQuestionUI(JToken choices)
    {
        //hide the other dialog ui components
        dialogText.SetActive(false);
        continueButton.SetActive(false);

        //destroy any previous buttons.
        while (buttons.Count > 0)
        {
            GameObject button = buttons.Dequeue();
            Destroy(button);
        }

        RectTransform rt = (RectTransform)dialogPanel.transform;

        float x = 500;// dialogPanel.transform.position.x;// - rt.rect.width / 8;
        float y = 100;// dialogPanel.transform.position.y; // + rt.rect.height / 8
        foreach (JToken choice in choices.Children())
        {
            string choiceText = choice.Value<string>("choice");

            GameObject button = (GameObject)Instantiate(buttonPrefab);
            button.transform.SetParent(dialogPanel.transform);
            button.transform.position = new Vector3(x, y);
            button.GetComponent<Button>().onClick.AddListener(delegate { currentCharacter.ChoiceClicked(choiceText); } );
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = choiceText;
            button.SetActive(true);

            buttons.Enqueue(button);

            Debug.Log(button.transform.position.x);
            Debug.Log(button.transform.position.y);

            y -= 30;
        }
    }

    internal void SwitchToDialogUI()
    {
        //destroy any previous buttons.
        while (buttons.Count > 0)
        {
            GameObject button = buttons.Dequeue();
            Destroy(button);
        }

        //show dialog UI
        dialogText.SetActive(true);
        continueButton.SetActive(true);
    }
}
