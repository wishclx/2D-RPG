using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Skill Data", fileName = "Skill Data - ")]
public class SkillDataSO : ScriptableObject
{
    [Header("Sklii description")]
    public string displayName;
    [TextArea]
    public string description;
    public Sprite icon;

    [Header("Unlock & Upgrade")]
    public int cost;
    public bool unlockedByDefault;
    public SkillType skillType;
    public UpgraedData upgradeData;
}

[Serializable]
/// <summary>
/// UpgraedData 的职责说明。
/// </summary>
public class UpgraedData
{
    public SkillUpgradeType upgradeType;
    public float cooldown;
    public DamageScaleData damageScaleData;
}
