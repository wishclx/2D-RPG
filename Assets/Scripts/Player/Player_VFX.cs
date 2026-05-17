using System.Collections;
using UnityEngine;

public class Player_VFX : Entity_VFX
{
    public void CreateEffectOf(GameObject effect, Transform target)
    {
        Instantiate(effect, target.position, Quaternion.identity);//在目标位置创建特效实例，旋转为默认值
    }
}
