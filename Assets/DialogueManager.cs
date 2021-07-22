using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

/// <summary>
/// This class is responsible for displaying dialog to the player.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    /// <summary> The text that displays the character name. </summary>
    public TextMeshProUGUI nameText;

    /// <summary> The text that displays dialog to the player. </summary>
    public TextMeshProUGUI dialogueText;

    /// <summary> The animator being used for the soldier. </summary>
    public Animator soldierAnimator;

    /// <summary> The animator being used for the hunter. </summary>
    public Animator hunterAnimator;

    /// <summary> The animator being used for the alien </summary>
    public Animator alienAnimator;

    /// <summary> The speed to write text at. </summary>
    public float textSpeed;

    /// <summary> The game's sound manager. </summary>
    public SoundManager soundManager;

    /// <summary> The game's game manager. </summary>
    public GameManager gameManager;

    /// <summary> The sentences being queued for display. </summary>
    private Queue<string> sentences = new Queue<string>();

    /// <summary>
    /// Sets the current character name.
    /// </summary>
    /// <param name="characterName">The name to display.</param>
    public void SetCharacter(string characterName)
    {
        nameText.text = characterName;
    }

    /// <summary>
    /// Sets the current conversation and clears previous dialog.
    /// </summary>
    /// <param name="dialog">The dialog to display.</param>
    public void SetConversation(string dialog)
    {
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

    /// <summary>
    /// Displays the next sentance that's queued, if not, closes the dialog.
    /// </summary>
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
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

    /// <summary>
    /// Types a sentance out with delay between characters.
    /// </summary>
    /// <param name="sentence">The sentance to type.</param>
    /// <returns>Internal unity wait time.</returns>
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
}
