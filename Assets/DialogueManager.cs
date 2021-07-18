using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Animator dialogAnimator;
    public Animator soldierAnimator;
    public Animator hunterAnimator;
    public Animator alienAnimator;
    public float textSpeed;
    public SoundManager soundManager;
    public GameManager gameManager;

    private Queue<string> sentences = new Queue<string>();

    public void SetCharacter(string characterName)
    {
        nameText.text = characterName;
    }

    public void SetConversation(string dialog)
    {
        dialogAnimator.SetBool("isOpen", true);

        switch (gameManager.currentCharacter.name)
        {
            case "Soldier":
                soldierAnimator.SetBool("Talking", true);
                soldierAnimator.SetBool("Idle", false);
                break;

            case "Hunter":
                hunterAnimator.SetBool("Talking", true);
                hunterAnimator.SetBool("Idle", false);
                break;

            case "Alien":
                alienAnimator.SetBool("Talking", true);
                alienAnimator.SetBool("Idle", false);
                break;
        }

        sentences.Clear();

        gameManager.currentCharacter.DialogRead(dialog);


        sentences.Enqueue(dialog);

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopTyping();
        StartCoroutine(TypeSentence(sentence));
    }

    /// <summary>
    /// Stops all voice sounds and stops typing.
    /// </summary>
    private void StopTyping()
    {
        soundManager.StopSound("hunter_voice");
        soundManager.StopSound("alien_voice");
        soundManager.StopSound("soldier_voice");

        switch (gameManager.currentCharacter.name)
        {
            case "Soldier":
                soldierAnimator.SetBool("Talking", false);
                soldierAnimator.SetBool("Idle", true);
                break;

            case "Hunter":
                hunterAnimator.SetBool("Talking", false);
                hunterAnimator.SetBool("Idle", true);
                break;

            case "Alien":
                alienAnimator.SetBool("Talking", false);
                alienAnimator.SetBool("Idle", true);
                break;
        }

        StopAllCoroutines();
    }

    IEnumerator TypeSentence(string sentence)
    {
        if (sentence == null)
            throw new InvalidDataException("Sentance is null!");

        string charName = gameManager.characterName.ToLower();

        //don't play voice sound if the player is talking.
        if (!nameText.text.Equals("Player"))
            soundManager.PlaySound($"{charName}_voice");

        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        soundManager.StopSound($"{charName}_voice");
    }

    public void EndDialogue()
    {
        dialogAnimator.SetBool("isOpen", false);
    }
}
