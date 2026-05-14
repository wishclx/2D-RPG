using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Dialogue Data/New Speaker Data", fileName = "Speaker -")]
public class DialogueSpeakerSO : ScriptableObject
{
    public string speakerName;
    public Sprite speakerPortrait;//角色头像，可以在对话界面显示
}
