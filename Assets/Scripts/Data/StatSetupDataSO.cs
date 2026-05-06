using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Default Stat Setup", fileName = "Default Stat Setup")]
// 允许在 Unity 菜单中创建默认属性配置的 ScriptableObject 资源。
/// <summary>
/// 角色属性配置数据。
/// 用于集中定义生命、攻击、防御和主属性的初始值。
/// </summary>
public class StatSetupDataSO : ScriptableObject
{
    [Header("Resources")]
    public float maxHealth = 100;
    public float healthRegen;

    [Header("Offense - Phyiscal Damage")]
    public float attackSpeed = 1;
    public float damage = 10;
    public float critChance;
    public float critPower = 150;
    public float armorReduction;

    [Header("Offense - Elemental Damage")]
    public float fireDamage;
    public float iceDamage;
    public float lightningDamage;

    [Header("Defense - Phyiscal Damage")]
    public float armor;
    public float evasion;

    [Header("Defense - Elemental Damage")]
    public float fireResistance;
    public float iceResistance;
    public float lightningResistance;

    [Header("Major Stats")]
    public float strength;
    public float agility;
    public float intelligence;
    public float vitality;
}
