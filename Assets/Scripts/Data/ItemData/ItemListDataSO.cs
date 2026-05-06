using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item list", fileName = "List of items -  ")]
public class ItemListDataSO : ScriptableObject
{
    public ItemDataSO[] itemList;

    public ItemDataSO GetItemData(string saveId)
    {
        //使用LINQ查询itemList数组，查找第一个满足条件的ItemDataSO对象，并返回该对象。如果没有找到满足条件的对象，则返回null。
        return itemList.FirstOrDefault(item => item != null && item.saveID == saveId);
    }

#if UNITY_EDITOR
    [ContextMenu("自动收集ItemDataSO资源")]
    public void CollectionItemData()
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemDataSO");//在项目中查找所有类型为ItemDataSO的资源，并返回它们的GUID（全局唯一标识符）数组

        //将每个GUID转换为资源路径，并加载对应的ItemDataSO资源，最后将所有非空的ItemDataSO对象存储在itemList数组中
        itemList = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<ItemDataSO>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(item => item != null)
            .ToArray();

        EditorUtility.SetDirty(this);//标记当前对象为已修改状态，以便在编辑器中保存更改
        AssetDatabase.SaveAssets();//保存所有修改过的资源到磁盘
    }
#endif
}
