using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary> The current state of the dialog, whether its on the main line or a side dialog. </summary>
enum DialogState { MainDialog, SideDialog }

/// <summary> The type of dialog being displayed. </summary>
enum DialogType { Dialog, Question, Response }

/// <summary>
/// This class represents a character within the game.
/// </summary>
public class Character
{
    /// <summary> Index of current main dialog step. </summary>
    private int mainDialogStep = -1;

    /// <summary> Index of current side dialog step. </summary>
    private int sideDialogStep = -1;

    /// <summary> The side conversation. </summary>
    private Dictionary<string, JToken> sideConversations;

    /// <summary> The main conversation. </summary>
    private JToken mainConversation;

    /// <summary> The current conversation being displayed. </summary>
    private JToken currentConversation;

    /// <summary> The current side conversation being displayed. </summary>
    private string currentSideDialog;

    /// <summary> The current game manager. </summary>
    private GameManager gameManager;

    /// <summary> The current dialog manager. </summary>
    private DialogueManager dialogManager;

    /// <summary> The current sound manager. </summary>
    private SoundManager soundManager;

    /// <summary> The state of the dialog being displayed. </summary>
    private DialogState state;

    /// <summary> The type of dialog being displayed. </summary>
    private DialogType dialogType;

    /// <summary> Whether the conversation has ended with this character. </summary>
    private bool conversationEnded;

    /// <summary> The last response given by the character. </summary>
    private string lastResponse;

    /// <summary> The last choice chosen by the player.. </summary>
    private string lastChoiceClicked;

    /// <summary> The index in the json array that contains the main character questions. </summary>
    private int baseQuestionIndex;

    /// <summary> The amount of unique dialogs that the user has read. </summary>
    public List<string> readDialogs;

    /// <summary> This characters name. </summary>
    public string name
    {
        get;
        private set;
    }

    /// <summary>
    /// Initalises a character.
    /// </summary>
    /// <param name="dialogManager">The game's dialog manager.</param>
    /// <param name="gameManager">The game's game manager.</param>
    /// <param name="soundManager">The game's sound manager.</param>
    /// <param name="name">The name of this character.</param>
    public Character(DialogueManager dialogManager, GameManager gameManager, SoundManager soundManager, string name)
    {
        this.gameManager = gameManager;
        this.dialogManager = dialogManager;
        this.soundManager = soundManager;
        this.name = name;
        this.sideConversations = new Dictionary<string, JToken>();
        this.readDialogs = new List<string>();

        //read the main conversation from file
        mainConversation = JObject.Parse(File.ReadAllText($"Assets/{name}/{name}.json"));

        //read all side conversations from files
        foreach (string file in Directory.GetFiles($"Assets/{name}", $"{name}_*.json"))
        {
            Debug.Log($"Reading:{file}");
            string findableName = file.Replace($@"Assets/{name}\{name}_", "").Replace(".json", "");
            sideConversations.Add(findableName, JObject.Parse(File.ReadAllText(file)));
        }

        //select the first conversation in the main dialog file.
        conversationEnded = false;
        state = DialogState.MainDialog;
        dialogType = DialogType.Dialog;
        currentConversation = GetNextMainConversation();
    }

    /// <summary>
    /// Checks if a dialog has been read before, if not it's added to the readDialogs list.
    /// </summary>
    /// <param name="dialog">The dialog to check.</param>
    internal void DialogRead(string dialog)
    {
        //if the dialog hasn't been read before
        if (!readDialogs.Contains(dialog))
        {
            readDialogs.Add(dialog);
        }
    }

    /// <summary>
    /// Checks if a word being clicked links to another conversation, if so, it starts that conversation.
    /// </summary>
    /// <param name="clickedWord">The word that was clicked</param>
    internal void WordClicked(string clickedWord)
    {
        JArray links = null;
        if (dialogType == DialogType.Dialog)
        {
            links = (JArray)currentConversation["links"];
        }

        if (dialogType == DialogType.Response)
        {
            if (currentConversation["choices"] != null)
            {
                links = (JArray)currentConversation.SelectToken($"$.choices[?(@.choice=='{lastChoiceClicked}')].links");
            } 
            else
            {
                if (state == DialogState.SideDialog)
                {
                    links = (JArray)currentConversation.SelectToken($"$.links");
                }
            }
        }

        foreach (JToken link in links.Children())
        {
            string word = (string)link["word"];
            string destinationName = (string)link["goto"];

            if (word.Equals(clickedWord))
            {
                sideDialogStep = -1;
                state = DialogState.SideDialog;
                currentSideDialog = destinationName;
                ContinueConversation();
            }
        }
    }

    /// <summary>
    /// Switches to a new conversation based on a given choice.
    /// </summary>
    /// <param name="choiceText">The text from the clicked button.</param>
    internal void ChoiceClicked(string choiceText)
    {
        lastChoiceClicked = choiceText;
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
            string character = (string)currentConversation["character"];
            if (character != null)
            {
                dialogManager.SetCharacter(character);
            }
            else
            {
                dialogManager.SetCharacter(name);
            }

            if (dialogType == DialogType.Dialog)
            {
                string currentDialog = (string)currentConversation["dialog"];
                dialogManager.SetConversation(currentDialog);
            }

            if (dialogType == DialogType.Response)
            {
                string currentDialog = (string)currentConversation["response"];
                dialogManager.SetConversation(currentDialog);
            }
        }
    }

    /// <summary>
    /// Gets the current side dialog from the given side dialog file.
    /// </summary>
    /// <param name="name">The name of the side dialog.</param>
    /// <returns>The current dialog for the given side conversation.</returns>
    private JToken GetSideConversation(string name)
    {
        sideDialogStep++;
        return sideConversations[name].SelectToken($"$.conversations[{sideDialogStep}]");
    }

    /// <summary>
    /// Gets the next step in the main conversation.
    /// </summary>
    /// <returns>The next dialog from the main conversation.</returns>
    private JToken GetNextMainConversation()
    {
        mainDialogStep++;
        return mainConversation.SelectToken($"$.conversations[{mainDialogStep}]");
    }

    /// <summary>
    /// Gets the response to the last clicked choice.
    /// </summary>
    /// <returns>The response to the last clicked choice</returns>
    private string GetNextResponse()
    {
        return (string)currentConversation.SelectToken($"$.choices[?(@.choice=='{lastChoiceClicked}')].response");
    }

}
