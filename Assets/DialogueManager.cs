using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public TextMeshProUGUI dialogueText;
    public Animator animator;
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
        animator.SetBool("isOpen", true);

        sentences.Clear();

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

        StopAllCoroutines();
    }

    IEnumerator TypeSentence(string sentence)
    {
        if (sentence == null)
            throw new InvalidDataException("Sentance is null!");

        string charName = gameManager.characterName.ToLower();

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
        animator.SetBool("isOpen", false);
    }
}
