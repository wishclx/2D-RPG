using System.Collections;
using UnityEngine;

[System.Serializable]
public class Buff
{
    public StatType type; // Buff类型
    public float value; // Buff数值
}

public class Object_Buff : MonoBehaviour
{
    private SpriteRenderer sr;
    private Entity_Stats statsToModify;

    [Header("Buff details")]
    [SerializeField] private Buff[] buffs; // Buff类型和数值的数组 
    [SerializeField] private string buffName; // Buff名称
    [SerializeField] private float buffDuration = 4.0f; // Buff持续时间
    [SerializeField] private bool canBeUsed = true;

    [SerializeField] private float floatSpeed = 1.0f;
    [SerializeField] private float floatRange = .1f;
    private Vector3 statrPosistion;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        statrPosistion = transform.position;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        //Mathf.Sin()方法返回一个值，该值在-1和1之间，表示一个正弦波的当前值。Time.time是从游戏开始到现在的时间，以秒为单位。floatSpeed是控制浮动速度的变量，floatRange是控制浮动范围的变量。通过将这些值相乘，我们可以得到一个在一定范围内上下浮动的yOffset。
        transform.position = statrPosistion + new Vector3(0, yOffset);
    }

    private void OnTriggerEnter2D(Collider2D collision)//当另一个Collider2D进入这个对象的触发器时调用
    {
        if (canBeUsed == false)
            return;

        statsToModify = collision.GetComponent<Entity_Stats>();//获取碰撞对象的Entity_Stats组件
        StartCoroutine(BuffCo(buffDuration));
    }

    private IEnumerator BuffCo(float duration)
    {
        canBeUsed = false;
        sr.color = Color.clear;
        ApplyBuff(true);

        yield return new WaitForSeconds(duration);

        ApplyBuff(false);
        Destroy(gameObject);
    }

    private void ApplyBuff(bool apply)
    {
        foreach (var buff in buffs)
        {
            if (apply)
                statsToModify.GetStatByType(buff.type).AddModifier(buff.value, buffName);
            //调用Entity_Stats组件的GetStatByType方法获取对应类型的Stat对象，并调用AddModifier方法添加Buff效果，传入Buff数值和Buff名称作为参数。
            else
                statsToModify.GetStatByType(buff.type).RemoveModifier(buffName);
            //调用Entity_Stats组件的GetStatByType方法获取对应类型的Stat对象，并调用RemoveModifier方法移除Buff效果，传入Buff名称作为参数。
        }
    }
}
