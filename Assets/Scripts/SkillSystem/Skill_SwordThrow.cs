using UnityEngine;

public class Skill_SwordThrow : Skill_Base
{

    private SkillObject_Sword currentSword;// 当前投掷的剑对象

    [Header("Regular Sword Upgrade")]
    [SerializeField] private GameObject swordPrefab;// 剑的预制体
    [Range(0, 10)]
    [SerializeField] private float throwPower = 5f;// 投掷力量

    [Header("Pierce Sword Upgrade")]
    [SerializeField] private GameObject pierceSwordPrefab;// 穿刺剑的预制体
    public int amountToPierce = 2;//穿刺数量

    [Header("Spin Sword Upgrade")]
    [SerializeField] private GameObject spinSwordPrefab;// 旋转剑的预制体
    public int maxDistance = 5;// 最大旋转距离
    public float attacksPerSecond = 6;// 每秒攻击次数
    public float maxSpinDuration = 3;// 最大旋转持续时间

    [Header("Trajectory prediction")]
    [SerializeField] private GameObject predictionDot;// 预制体，用于显示预测轨迹点
    [SerializeField] private int numberOfDots = 20;// 预测点的数量
    [SerializeField] private float spaceBetweenDots = 0.05f;// 预测点之间的间隔
    private float swordGravity;// 投掷物体的重力缩放因子
    private Transform[] dots;// 存储预测点的Transform组件
    private Vector2 confirmedDirection;// 确认的投掷方向


    protected override void Awake()
    {
        base.Awake();
        swordGravity = swordPrefab.GetComponent<Rigidbody2D>().gravityScale;// 获取剑对象的重力缩放因子，以便在预测轨迹时考虑重力影响
        dots = GenerateDots();// 生成预测点
    }

    public override bool CanUseSkill()
    {
        if (currentSword != null)// 如果当前已经有一个投掷的剑对象存在，则不能再次使用技能，直到当前剑对象被销毁或回收。
        {
            currentSword.GetSwordBackToPlayer();// 让当前剑对象返回玩家，准备下一次投掷。
            return false;
        }

        return base.CanUseSkill();
    }

    public void ThrowSword()
    {
        GameObject swordPrefab = GetSwordPrefab();
        GameObject newSword = Instantiate(swordPrefab, dots[1].position, Quaternion.identity);// 在预测点位置实例化新的剑对象

        currentSword = newSword.GetComponent<SkillObject_Sword>();// 获取新剑对象的 SkillObject_Sword 组件
        currentSword.SetupSword(this, GetThrowPower());// 设置新剑对象的投掷参数
    }

    private GameObject GetSwordPrefab()
    {
        if (Unlocked(SkillUpgradeType.SwordThrow))
            return swordPrefab;

        if (Unlocked(SkillUpgradeType.SwordThrow_Pierce))
            return pierceSwordPrefab;

        if (Unlocked(SkillUpgradeType.SwordThrow_Spin))
            return spinSwordPrefab;

        Debug.Log("没有有效的技能!");
        return null;
    }

    private Vector2 GetThrowPower() => confirmedDirection * throwPower * 10;// 计算投掷力量向量

    public void PredictTrajectory(Vector2 direction)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            // 计算每个预测点的位置，基于初始位置、投掷方向和时间间隔
            dots[i].position = GetTrajectory(direction, spaceBetweenDots * i);// 计算并设置每个预测点的位置
        }
    }

    private Vector2 GetTrajectory(Vector2 direction, float t)
    {
        // 计算投掷物体在给定时间点的位置，考虑重力影响
        float scaledThrowPower = throwPower * 10;// 调整投掷力量的缩放因子

        Vector2 initialVelocity = direction * scaledThrowPower;// 计算初始速度

        Vector2 gravityEffect = 0.5f * Physics2D.gravity * swordGravity * t * t;// 重力位移项：s_g = 1/2 * a * t^2（a 为重力加速度 * swordGravity）

        Vector2 predictedPoint = (initialVelocity * t) + gravityEffect;// 预测总位移：s = v0 * t + s_g（初速度位移 + 重力位移）

        Vector2 playerPosition = transform.root.position;// 获取玩家位置

        return playerPosition + predictedPoint;// 返回预测点的世界坐标
    }

    public void ConfirmTrajectory(Vector2 direction) => confirmedDirection = direction;// 确认投掷方向
    public void EnableDots(bool enable)
    {
        foreach (Transform t in dots)
            t.gameObject.SetActive(enable);// 根据参数启用或禁用预测点
    }

    private Transform[] GenerateDots()
    {
        Transform[] newDots = new Transform[numberOfDots];

        for (int i = 0; i < numberOfDots; i++)
        {
            newDots[i] = Instantiate(predictionDot, transform.position, Quaternion.identity, transform).transform;// 实例化预测点并存储其Transform组件
            newDots[i].gameObject.SetActive(false);// 初始时隐藏预测点
        }

        return newDots;
    }
}
