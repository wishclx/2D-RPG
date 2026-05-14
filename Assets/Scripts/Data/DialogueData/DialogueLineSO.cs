using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Dialogue Data/New Line Data", fileName = "Line -")]
public class DialogueLineSO : ScriptableObject
{
    [Header("对话信息")]
    public string dialogueGroupName;
    public DialogueSpeakerSO speaker;

    [Header("对话设置")]
    [TextArea] public string[] textLine;

    [Header("对话选项")]
    [TextArea] public string playerChoiceAnswer;
    public DialogueLineSO[] choiceLines;


    [Header("对话动作")]
    [TextArea] public string actionLine;
    public DialogueActionType actionType;

    public string GetFirstLine() => textLine[0];//返回第一行对话文本

    public string GetRandomLine()
    {
        return textLine[Random.Range(0, textLine.Length)];//随机返回一行对话文本
    }
}
