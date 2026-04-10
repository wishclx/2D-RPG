using UnityEngine;
using System; 

[CreateAssetMenu(menuName = "RPG Setup/Skill Data", fileName = "Skill Data - ")]
public class Skill_DataSO : ScriptableObject
{
    public int cost;
    public SkillType skillType;
    public UpgraedData upgradeData;

    [Header("Sklii description")]
    public string displayName;
    [TextArea]
    public string description;
    public Sprite icon;
}

[Serializable]
public class UpgraedData
{
    public SkillUpgradeType upgradeType;
    public float cooldown;
}