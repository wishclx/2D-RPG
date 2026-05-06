using UnityEngine;

public class Player_Health : Entity_Health
{
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();//获取玩家组件
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))//按下K键来测试玩家死亡
        {
            Die();//调用死亡方法
        }
    }

    protected override void Die()
    {
        base.Die();

        //GameManager.instance.SetLastPlayerPosition(transform.position);

        //打开死亡界面
        player.ui.OpenDeathScreenUI();

        //GameManager.instance.RestartScene();//重新加载当前场景
    }
}
