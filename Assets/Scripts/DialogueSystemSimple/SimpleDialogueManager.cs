using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SimpleDialogueManager : MonoBehaviour
{
    // when player move, check it's panel index and grid position with struct.
    // if has it in list, start dialogue; automatically update dialogue text after 1 sec.

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //public TextMeshProUGUI dialogueText;
    bool[] isDialogueActive = new bool[100];
    int currentDialogueIndex = 0;
    private DialogueDataList dialogueData;
    private DialogueDataList.DialogueData[] dialogueDatas;
    public GameObject dialogueTextPrefab;
    public GameObject chatboxPrefab;
    private const float MAX_CHATBOX_WIDTH = 200f; // Maximum width for chatbox in pixels

    void Start()
    {
        // Load dialogue data from Resources folder
        dialogueData = Resources.Load<DialogueDataList>("DialogueDatas/" + SceneManager.GetActiveScene().name);
        if (dialogueData != null)
        {
            dialogueDatas = dialogueData.dialogueDatas;
            
            // Set panelObject for each dialogue entry based on designer's input
            for (int i = 0; i < dialogueDatas.Length; i++)
            {
                DialogueDataList.DialogueData data = dialogueDatas[i];
                foreach (scrPanel pn in FindObjectsByType<scrPanel>(FindObjectsSortMode.None))
                {
                    if (pn.Time_index == data.panelIndex)
                    {
                        data.panelObject = pn;
                        dialogueDatas[i] = data;
                        isDialogueActive[i] = false;
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.Log("Dialogue data not found for scene: " + SceneManager.GetActiveScene().name);
        }
        currentDialogueIndex = 0;
    }

    public void StartDialogue(GameObject panelObject, scrPlayer player)
    {

        if (dialogueData == null) return;
        if (dialogueDatas.Length == 0) return;
        //if (isDialogueActive) return;
        foreach (DialogueDataList.DialogueData data in dialogueDatas)
        {
            if (isDialogueActive[data.panelIndex]) return; //if the dialogue is already active, return
            if (data.panelObject != null && 
                data.panelObject.gameObject == panelObject && 
                data.gridPosition == player.GetComponent<GridObject>().gridPosition)
            {
                //isDialogueActive = true;
                isDialogueActive[data.panelIndex] = true;
                currentDialogueIndex = 0;

                //calculate position deviation, deviate one unit from player's position to the center of the screen
                Vector3 deviation = (Camera.main.WorldToScreenPoint(player.transform.position) - player.transform.position).normalized;

                //instantiate chatbox
                GameObject chatbox = Instantiate(chatboxPrefab);
                chatbox.transform.SetParent(GameObject.Find("Canvas").transform);
                SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
                Vector3 topRightWorld = sr.bounds.max;
                Vector3 topRightScreen = Camera.main.WorldToScreenPoint(topRightWorld);
                Vector3 offset = new Vector3(-20, 20, 0); // Adjust these values as needed
                chatbox.transform.position = topRightScreen + offset;

                //instantiate dialogue text
                GameObject dialogueText = Instantiate(dialogueTextPrefab);
                dialogueText.transform.SetParent(GameObject.Find("Canvas").transform);
                dialogueText.transform.position = topRightScreen + offset + new Vector3(5, 0, 0);
                StartCoroutine(UpdateDialogueText(data, player, dialogueText, chatbox));
                break;
            }
        }
    }

    // recursion to go through all dialogue lines
    IEnumerator UpdateDialogueText(DialogueDataList.DialogueData data, scrPlayer player, GameObject dialogueText, GameObject chatbox)
    {
        string currentText = data.dialogueText[currentDialogueIndex];
        TextMeshProUGUI textComponent = dialogueText.GetComponent<TextMeshProUGUI>();
        textComponent.text = ""; // Clear the text initially
        // type one line
        // Type out the text character by character
        int archivedTextLength = 0;
        foreach (char c in currentText)
        {
            textComponent.text += c;

            /*
            //update chatbox size
            var rt = chatbox.GetComponent<RectTransform>();
            textComponent.ForceMeshUpdate();// Force TMP to update so we get accurate layout info
            float currentTextWidth = textComponent.preferredWidth;// Use preferred width 
            float chatboxWidth = Mathf.Min(currentTextWidth, MAX_CHATBOX_WIDTH);
            */
            // Resize current chatbox

            var rt = chatbox.GetComponent<RectTransform>();
            float width = Mathf.Min(textComponent.preferredWidth, textComponent.rectTransform.rect.width) + 10;
            rt.sizeDelta = new Vector2(width, textComponent.preferredHeight);

            /*
            // If new text exceeds max width, spawn a new chatbox
            if (currentTextWidth > MAX_CHATBOX_WIDTH)
            {
                chatboxes.Add(chatbox); // archive current
                GameObject newChatbox = Instantiate(chatboxPrefab, chatbox.transform.parent);
                chatbox = newChatbox;

                // Move it down by one line
                chatbox.transform.localPosition = chatboxes[chatboxes.Count - 1].transform.localPosition + Vector3.down * textComponent.fontSize;

                // Reset width for new chatbox
                chatbox.GetComponent<RectTransform>().sizeDelta = new Vector2(0, textComponent.fontSize);
            }
            */

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(1f);
        currentDialogueIndex++;
        if (currentDialogueIndex < data.dialogueText.Length)
        {
            StartCoroutine(UpdateDialogueText(data, player, dialogueText, chatbox));
        }
        else
        {
            EndDialogue(dialogueText, data, chatbox);
        }
    }

    void EndDialogue(GameObject dialogueText, DialogueDataList.DialogueData data, GameObject chatbox)
    {
        isDialogueActive[data.panelIndex] = false;
        currentDialogueIndex = 0;
        Destroy(dialogueText);
        Destroy(chatbox);
    }
}
