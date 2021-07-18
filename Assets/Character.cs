using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


enum DialogState { MainDialog, SideDialog }
enum DialogType { Dialog, Question, Response }

public class Character
{
    /// <summary> Index of current main dialog step. </summary>
    private int mainDialogStep = -1;

    private int sideDialogStep = -1;

    private int responseDialogStep = -1;

    /// <summary> The side conversation. </summary>
    private Dictionary<string, JToken> sideConversations;

    private JToken sideConversation;

    /// <summary> The main conversation. </summary>
    private JToken mainConversation;

    private JToken currentConversation;
    private GameManager gameManager;
    private DialogueManager dialogManager;
    private SoundManager soundManager;

    public string name
    {
        get;
        private set;
    }

    private DialogState state;
    private DialogType dialogType;

    private bool conversationEnded;

    private string lastResponse;
    private string lastChoiceClicked;

    private string currentSideDialog;
    private int baseQuestionIndex;

    public Character(DialogueManager dialogManager, GameManager gameManager, SoundManager soundManager, string name)
    {
        this.gameManager = gameManager;
        this.dialogManager = dialogManager;
        this.soundManager = soundManager;
        this.name = name;
        this.sideConversations = new Dictionary<string, JToken>();

        mainConversation = JObject.Parse(File.ReadAllText($"Assets/{name}/{name}.json"));

        foreach (string file in Directory.GetFiles($"Assets/{name}", $"{name}_*.json"))
        {
            string findableName = file.Replace($@"Assets/{name}\{name}_", "").Replace(".json", "");
            sideConversations.Add(findableName, JObject.Parse(File.ReadAllText(file)));
        }

        //select the first conversation in the main dialog file.
        conversationEnded = false;
        state = DialogState.MainDialog;
        dialogType = DialogType.Dialog;
        currentConversation = GetNextMainConversation();
    }

    internal void WordClicked(string lastClickedWord)
    {
        JArray links = null;
        if (dialogType == DialogType.Dialog)
        {
            links = (JArray)currentConversation["links"];
        }

        if (dialogType == DialogType.Response)
        {
            links = (JArray)currentConversation.SelectToken($"$.choices[?(@.choice=='{lastChoiceClicked}')].links");
        }

        foreach (JToken link in links.Children())
        {
            string word = (string)link["word"];
            string destinationName = (string)link["goto"];

            if (word.Equals(lastClickedWord))
            {
                sideDialogStep = -1;
                state = DialogState.SideDialog;
                currentSideDialog = destinationName;
                ContinueConversation();
            }
        }
    }

    internal void ChoiceClicked(string choiceText)
    {
        lastChoiceClicked = choiceText;
        responseDialogStep = -1;
        string response = GetNextResponse();

        lastResponse = response;

        dialogType = DialogType.Response;

        soundManager.PlaySound("mouse_click");

        gameManager.SwitchToDialogUI();
        DisplayConversation();
    }

    /// <summary>
    /// Advances the dialog of this character.
    /// </summary>
    internal void ContinueConversation()
    {
        Debug.Log($"Going into new conversation. {state}, {dialogType}");
        JToken nextConversation = null;

        if (currentConversation != null && dialogType == DialogType.Response)
        {
            string nextResponse = GetNextResponse();
            
            //don't advance the convo if there's another response
            if (nextResponse != null)
            {
                dialogManager.SetConversation(nextResponse);
                return;
            }
        }

        //since side dialogs are only a one paragraph thing we can go back into the main dialog
        if (state == DialogState.SideDialog)
        {
            nextConversation = GetSideConversation(currentSideDialog);
        }

        if (state == DialogState.MainDialog)
        {
            nextConversation = GetNextMainConversation();
        }


        currentConversation = nextConversation;

        if (nextConversation != null)
        {
            //check if the next conversation is questions
            JToken choices = nextConversation.Value<JToken>("choices");
            if (choices != null)
            {
                if (state == DialogState.MainDialog)
                {
                    baseQuestionIndex = mainDialogStep;
                }

                dialogType = DialogType.Question;
                gameManager.SwitchToQuestionUI(choices);
            }
            else
            {

                string characterName = nextConversation.Value<string>("character");
                if (characterName != null)
                {
                    dialogManager.SetCharacter(characterName);
                }
                else
                {
                    dialogManager.SetCharacter(name);
                }

                dialogManager.SetConversation(nextConversation.Value<string>("dialog"));
            }
        }
        else
        {
            //go back to the main dialog state if we get to the end of a side dialog.
            if (state == DialogState.SideDialog)
            {
                state = DialogState.MainDialog;
                ContinueConversation();
            }
            else
            {
                //go back to the base questions
                mainDialogStep = baseQuestionIndex - 1;
                state = DialogState.MainDialog;
                dialogType = DialogType.Question;
                ContinueConversation();
            }
        }
    }

    /// <summary>
    /// Displays the current dialog for this character.
    /// </summary>
    internal void DisplayConversation()
    {
        if (dialogType == DialogType.Dialog || dialogType == DialogType.Response)
        {
            gameManager.SwitchToDialogUI();
        }

        if (dialogType == DialogType.Question)
        {
            JToken choices = currentConversation.Value<JToken>("choices");
            gameManager.SwitchToQuestionUI(choices);
        }

        if (conversationEnded)
        {
            dialogManager.EndDialogue(); //calling this again to make sure dialog window closes.
            return;
        }

        if (dialogType == DialogType.Response)
        {
            dialogManager.SetCharacter(name);
            dialogManager.SetConversation(lastResponse);
            return;
        }

        if (currentConversation != null)
        {

            if (dialogType == DialogType.Dialog)
            {
                string currentDialog = (string)currentConversation["dialog"];
                dialogManager.SetCharacter(name);
                dialogManager.SetConversation(currentDialog);
            }

            if (dialogType == DialogType.Response)
            {
                string currentDialog = (string)currentConversation["responses"][responseDialogStep];
                dialogManager.SetCharacter(name);
                dialogManager.SetConversation(currentDialog);
            }
        }
    }

    private void EndDialogue()
    {
        conversationEnded = true;
        dialogManager.EndDialogue();
    }

    private JToken GetSideConversation(string name)
    {
        Debug.Log($"Going into side dialog: {name}");
        sideDialogStep++;
        return sideConversations[name].SelectToken($"$.conversations[{sideDialogStep}]");
    }

    private JToken GetNextMainConversation()
    {
        mainDialogStep++;
        return mainConversation.SelectToken($"$.conversations[{mainDialogStep}]");
    }

    private string GetNextResponse()
    {
        responseDialogStep++;
        return (string)currentConversation.SelectToken($"$.choices[?(@.choice=='{lastChoiceClicked}')].responses[{responseDialogStep}]");
    }

}
