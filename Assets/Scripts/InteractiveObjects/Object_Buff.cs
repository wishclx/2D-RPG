using System.Collections;
using UnityEngine;

[System.Serializable]
/// <summary>
/// Buff 的职责说明。
/// </summary>
public class Buff
{
    public StatType type;
    public float value;
}

/// <summary>
/// Object_Buff 的职责说明。
/// </summary>
public class Object_Buff : MonoBehaviour
{
    private SpriteRenderer sr;
    private Entity_Stats statsToModify;

    [Header("Buff details")]
    [SerializeField] private Buff[] buffs;
    [SerializeField] private string buffName;
    [SerializeField] private float buffDuration = 4.0f; // buff 持续时间
    [SerializeField] private bool canBeUsed = true;

    [SerializeField] private float floatSpeed = 1.0f;
    [SerializeField] private float floatRange = .1f;
    private Vector3 statrPosistion;

    /// <summary>
    /// 执行 Awake 逻辑。
    /// </summary>
    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        statrPosistion = transform.position;
    }

    /// <summary>
    /// 执行 Update 逻辑。
    /// </summary>
    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;

        transform.position = statrPosistion + new Vector3(0, yOffset);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canBeUsed == false)
            return;

        statsToModify = collision.GetComponent<Entity_Stats>();
        StartCoroutine(BuffCo(buffDuration));
    }

    /// <summary>
    /// 执行 BuffCo 逻辑。
    /// </summary>
    private IEnumerator BuffCo(float duration)
    {
        canBeUsed = false;
        sr.color = Color.clear;
        ApplyBuff(true);

        yield return new WaitForSeconds(duration);

        ApplyBuff(false);
        Destroy(gameObject);
    }

    /// <summary>
    /// 执行 ApplyBuff 逻辑。
    /// </summary>
    private void ApplyBuff(bool apply)
    {
        foreach (var buff in buffs)
        {
            if (apply)
                statsToModify.GetStatByType(buff.type).AddModifier(buff.value, buffName);

            else
                statsToModify.GetStatByType(buff.type).RemoveModifier(buffName);

        }
    }
}


