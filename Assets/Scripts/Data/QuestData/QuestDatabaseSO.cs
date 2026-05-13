using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Quest Data/Quest Database", fileName = "Quest Database")]
public class QuestDatabaseSO : ScriptableObject
{
    public QuestDataSO[] allQuests;

    public QuestDataSO GetQuestById(string id)
    {
        //使用LINQ的FirstOrDefault方法在allQuests数组中查找第一个满足条件的QuestDataSO对象，并返回该对象。如果没有找到满足条件的对象，则返回null。
        return allQuests.FirstOrDefault(q => q != null && q.questSaveId == id);
    }

#if UNITY_EDITOR
    [ContextMenu("自动收集任务资源")]
    public void CollectionItemData()
    {
        string[] guids = AssetDatabase.FindAssets("t:QuestDataSO");//在项目中查找所有类型为QuestDataSO的资源，并返回它们的GUID（全局唯一标识符）数组

        //将每个GUID转换为资源路径，并加载对应的QuestDataSO资源，最后将所有非空的QuestDataSO对象存储在allQuests数组中
        allQuests = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<QuestDataSO>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(q => q != null)
            .ToArray();

        EditorUtility.SetDirty(this);//标记当前对象为已修改状态，以便在编辑器中保存更改
        AssetDatabase.SaveAssets();//保存所有修改过的资源到磁盘
    }
#endif
}
