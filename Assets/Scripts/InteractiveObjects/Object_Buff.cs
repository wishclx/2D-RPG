using System.Collections;
using UnityEngine;


public class Object_Buff : MonoBehaviour
{
    private Player_Stats statsToModify;

    [Header("Buff details")]
    [SerializeField] private BuffEffectData[] buffs;
    [SerializeField] private string buffName;
    [SerializeField] private float buffDuration = 4.0f; // buff 持续时间

    [Header("Floaty movement")]
    [SerializeField] private float floatSpeed = 1.0f;
    [SerializeField] private float floatRange = .1f;
    private Vector3 statrPosistion;

    private void Awake()
    {
        statrPosistion = transform.position;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        transform.position = statrPosistion + new Vector3(0, yOffset);
    }

    private void OnTriggerEnter2D(Collider2D collision)//当玩家进入触发器时，尝试应用buff效果
    {
        statsToModify = collision.GetComponent<Player_Stats>();//尝试获取玩家的Player_Stats组件，如果没有则返回

        if (statsToModify.CanApplyBuffOf(buffName))//如果玩家可以应用该buff效果，则应用buff并销毁该对象
        {
            statsToModify.ApplyBuff(buffs, buffDuration, buffName);
            Destroy(gameObject);
        }
    }

}
