using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public int gold;//金币

    public List<Inventory_Item> itemListl;//玩家背包中的物品列表
    public SerializableDictionary<string, int> inventory;
    public SerializableDictionary<string, int> storageItems;//仓库中的物品列表
    public SerializableDictionary<string, int> storageMaterials;//仓库中的材料列表

    public SerializableDictionary<string, ItemType> equipItems;//已装备的物品字典，键是装备槽位的唯一ID，值是该槽位上装备的物品类型

    public int skillPoints;
    public SerializableDictionary<string, bool> skillTreeUI;//技能树UI状态的字典，键是技能树节点的唯一ID，值是该节点是否已解锁
    public SerializableDictionary<SkillType, SkillUpgradeType> skillUpgrades;//技能升级的字典，键是技能类型，值是该技能的升级类型

    public SerializableDictionary<string, bool> unlockedCheckpoints;//解锁的检查点的字典，键是检查点的唯一ID，值是该检查点是否已解锁
    public SerializableDictionary<string, Vector3> inScenePortals;//场景内传送门位置的字典，键是传送门的唯一ID，值是该传送门的位置

    public SerializableDictionary<string, bool> completedQuests;//已完成的任务字典，键是任务的唯一ID，值是该任务是否已完成
    public SerializableDictionary<string, int> activeQuests;//进行中的任务字典，键是任务的唯一ID，值是该任务的当前进度

    public string portalDestinationSceneName;//传送门目的地场景的名字
    public bool returningFromTown;//是否城镇

    public string lastScenePlayed;//玩家上次进入的场景名称
    public Vector3 lastPlayerPosition;//玩家上次的位置

    public GameData()
    {
        inventory = new SerializableDictionary<string, int>();
        storageItems = new SerializableDictionary<string, int>();
        storageMaterials = new SerializableDictionary<string, int>();

        equipItems = new SerializableDictionary<string, ItemType>();

        skillTreeUI = new SerializableDictionary<string, bool>();
        skillUpgrades = new SerializableDictionary<SkillType, SkillUpgradeType>();

        unlockedCheckpoints = new SerializableDictionary<string, bool>();
        inScenePortals = new SerializableDictionary<string, Vector3>();

        completedQuests = new SerializableDictionary<string, bool>();
        activeQuests = new SerializableDictionary<string, int>();
    }
}
