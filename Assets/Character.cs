using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Character
{
    /// <summary> Index of current main dialog step. </summary>
    private int mainDialogStep = -1;

    /// <summary> The side conversation. </summary>
    private JToken sideConversation;

    /// <summary> The main conversation. </summary>
    private JToken mainConversation;

    private JToken currentConversation;

    private DialogueManager dialogManager;

    public string Name { get; internal set; }

    public Character(DialogueManager dialogManager, string name, string mainDialogFile, string sideDialogFile)
    {
        this.dialogManager = dialogManager;
        this.Name = name;

        mainConversation = JObject.Parse(File.ReadAllText(mainDialogFile));
        sideConversation = JObject.Parse(File.ReadAllText(sideDialogFile));

        //select the first conversation in the main dialog file.
        currentConversation = GetNextMainConversation();
    }

    internal void WordClicked(string lastClickedWord)
    {
        JArray links = (JArray)currentConversation["links"];

        foreach (JToken link in links.Children())
        {
            string word = (string)link["word"];
            string destinationName = (string)link["goto"];
            
            if (word.Equals(lastClickedWord))
            {
                ContinueConversation(destinationName);
            }
        }
    }

    /// <summary>
    /// Advances the dialog of this character.
    /// </summary>
    internal void ContinueConversation(string sideConversationName)
    {
        Debug.Log($"Going into new conversation.");
        JToken nextConversation;

        //since side dialogs are only a one paragraph thing we can go back into the main dialog
        if (sideConversationName != null)
        {
            nextConversation = GetSideConversation(sideConversationName);

            Debug.Log($"Going to side conversation: {nextConversation}"); 
        }
        else
        {
            nextConversation = GetNextMainConversation();
        }

        currentConversation = nextConversation;

        if (nextConversation != null)
        {
            dialogManager.SetConversation(nextConversation.Value<string>("dialog"));
        }
        else
        {
            dialogManager.EndDialogue();
        }
    }

    internal void ContinueConversation()
    {
        ContinueConversation(null);
    }

    /// <summary>
    /// Displays the current dialog for this character.
    /// </summary>
    internal void DisplayConversation()
    {
        string currentDialog = (string)currentConversation["dialog"];

        dialogManager.SetConversation(currentDialog);
    }

    private JToken GetSideConversation(string name)
    {
        return sideConversation.SelectToken($"$.conversations[?(@.name == '{name}')]");
    }

    private JToken GetNextMainConversation()
    {
        mainDialogStep++;
        return mainConversation.SelectToken($"$.conversations[{mainDialogStep}]");
    }

}
