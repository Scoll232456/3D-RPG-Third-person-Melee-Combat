using UnityEngine;
using UnityEngine.UI; // 别忘了引入 UI 命名空间

public class EnemyHealthBar : MonoBehaviour
{
    // 血条Slider的引用
    public Slider healthSlider;

    // 角色的最大生命值
    public float maxHealth = 100f;

    // 角色当前生命值
    private float currentHealth;

    // 用于让血条始终面向摄像机的变量 (可选的，如果不需要一直面向可以不加)
    private Transform mainCameraTransform;

    void Start()
    {
        // 初始化生命值
        currentHealth = maxHealth;
        // 获取主摄像机的Transform，用于后续的朝向计算
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        // 更新Slider的初始值
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void Update()
    {
        // 让血条Canvas始终面向摄像机 (Billboarding效果) [citation:1][citation:2]
        // 如果你希望血条固定在一个角度（比如永远不转），可以注释掉这段代码
        //if (mainCameraTransform != null)
        //{
        //    // 让血条的面（forward）指向摄像机，但保持竖直（不绕X轴旋转）
        //    // 方法1：只复制摄像机的Y轴旋转
        //    Vector3 targetDirection = mainCameraTransform.forward;
        //    targetDirection.y = 0; // 保持竖直
        //    if (targetDirection != Vector3.zero)
        //        transform.rotation = Quaternion.LookRotation(targetDirection);

        //    // 方法2：更简单的写法，让血条正面直接朝向摄像机 [citation:2]
        //    // transform.forward = mainCameraTransform.forward;
        //}
    }

    // 公开方法：用于从外部调用，对小兵造成伤害
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 限制血量范围

        // 更新UI Slider的值 [citation:1][citation:4]
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        // 可以在这里添加死亡判断等逻辑
        if (currentHealth <= 0)
        {
            // 处理死亡，比如销毁小兵
            // Destroy(gameObject);
        }
    }

    // 可选：获取当前血量
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}