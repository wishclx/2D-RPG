using UnityEngine;

public class Player_SwordThrowState : PlayerState
{
    private Camera mainCamera;

    public Player_SwordThrowState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        skillsManager.swordThrow.EnableDots(true);// 进入剑抛出状态时启用预测点显示。

        if (mainCamera != Camera.main)
            mainCamera = Camera.main;// 获取主摄像机引用。
    }

    public override void Update()
    {
        base.Update();

        Vector2 dirToMouse = DirectionToMouse();// 计算从玩家位置到鼠标位置的方向向量。

        player.SetVelocity(0, rb.linearVelocity.y);
        player.HandleFlip(dirToMouse.x);// 根据方向向量的 x 分量决定玩家朝向。
        skillsManager.swordThrow.PredictTrajectory(dirToMouse);// 调用技能管理器中的剑抛出技能的预测方法，传入方向向量以显示预测轨迹。  

        if (input.Player.Attack.WasPressedThisFrame())// 如果玩家在剑抛出状态下按下攻击输入，触发剑抛出动画。
        {
            anim.SetBool("swordThrowPerformed", true);

            skillsManager.swordThrow.EnableDots(false);// 禁用预测点显示，因为玩家已经确认了投掷方向。
            skillsManager.swordThrow.ConfirmTrajectory(dirToMouse);// 确认投掷方向，准备执行投掷逻辑。
        }

        if (input.Player.RangeAttack.WasReleasedThisFrame() || triggerCalled) // 当玩家释放远程攻击输入或触发器被调用时，切换回空闲状态。
            stateMachine.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool("swordThrowPerformed", false);
        skillsManager.swordThrow.EnableDots(false);// 退出剑抛出状态时确保预测点被禁用。
    }

    private Vector2 DirectionToMouse()
    {
        if (mainCamera == null)
            return new Vector2(player.facingDir, 0f);

        Vector2 mousePos = player.mousePosistion;

        bool outOfScreen =
            mousePos.x < 0 || mousePos.x > Screen.width ||
            mousePos.y < 0 || mousePos.y > Screen.height;

        if (outOfScreen)
            return new Vector2(player.facingDir, 0f);

        float zDistance = Mathf.Abs(mainCamera.transform.position.z - player.transform.position.z);
        Vector3 screenPoint = new Vector3(mousePos.x, mousePos.y, zDistance);
        Vector2 worldMousePos = mainCamera.ScreenToWorldPoint(screenPoint);

        Vector2 direction = worldMousePos - (Vector2)player.transform.position;

        if (direction.sqrMagnitude < 0.0001f)
            return new Vector2(player.facingDir, 0f);

        return direction.normalized;
    }
}
