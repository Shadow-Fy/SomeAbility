# 🗑️回收库🗑️

此回收库中的每个 Scene 都是一个学习后的内容并进行了复现，如果想看源码可以移步 github [👉点击此处](https://github.com/Shadow-Fy/SomeAbility)
所有此 README 中的内容都已经通过unity引擎进行了复现



## 🏃移动跳跃手感二次优化🏃

首先附上此教程的来源 [👉点击此处：Improve your Platformer with Acceleration | Examples in Unity - YouTube](https://www.youtube.com/watch?v=KKGdDBFcu0Q)
附上视频中的源码：[👉点击此处](https://github.com/DawnosaurDev/platformer-movement)
这是国外的一位大佬 🦕**Dawnosaur** 根据**蔚蓝**、**空洞骑士**、**超级肉肉男孩**这三款游戏的移动跳跃操作手感，制作的一视频，讲述了如何根据这些游戏来优化自己的游戏。
如果不能翻墙看原视频也可以通过此文了解到部分



附上一张自己整理的图便于理解这个
![PlatformerDemo.drawio](https://img2023.cnblogs.com/blog/2821249/202310/2821249-20231020170510513-2068922.png)

首先关于玩家的操作是有两部分负责：

1. 玩家本身的移动操作等，需要用代码进行实现。
2. 玩家进行移动操作的各种相关数据（包括移动速度、跳跃高度等等），这一部分通过 ScriptObject类 进行管理。

 如果你熟悉 MVC 架构的话看到这个可能你会突然眼前一亮或者对这位大佬的技术水平有了一小部分的认可（我开始也是这样的），如果不懂 MVC架构的话可以去查一下这是什么以及这样写的好处，废话不多说继续主题



### 移动

首先关于如何优化移动方式，大部分人写移动的代码都会如下：

```c#
public Rigidbody2D rb;
public Vector2 movementInput;
public float speed;

void Start()
{
    // 获取刚体
    rb = getComponent<Rigidbody2D>();
}

void Update()
{
    // 获取上下左右按键的输入
    moveInput.x = Input.GetAxisRaw("Horizontal");
	moveInput.y = Input.GetAxisRaw("Vertical");
}

void FixedUpdate()
{
    // 操作player移动
    rb.velocity = new vector2(speed * movemnetInput.x , rb.velocity.y);
}
```

大部分人在写角色的移动代码时应该和这个类似，并且是通过unity自带的物理引擎来实现会更为真实。

但是这样写有好处也会有一定的不方便之处： 

- 快速跑起来，对于初学者来说编码很简单并且反应灵敏。

+ 游戏移动没有加速或减速时间会让角色感觉想当僵硬化和机械化

+ 开发后期如果游戏中有设计一些较为复杂的道具需要和角色进行复杂的互动如传送带弹簧等对象变得困难，实现起来笨重



所以取而代之的是💪**力（Force）💪**，使用力可以获得更流畅的运动，并且结合了unity的物理引擎
但是如果自己之前尝试过使用力可能会发现不舒服，并且反应迟钝难以控制，所以需要通过代码进行优化。

首先创建角色的 ScriptObject类用于管理角色的属性值，具体属性值如下：

设置了三个属性值：最大速度、加速时的加速倍率、减速时的减速倍率
（这里和原作者的设置不一样，因为暂时不能理解原作者的设计思路所以直接进行了一个简化）

```c#
[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Speed")]
    // 角色移动时需要达到的最大速度
    public float runMaxSpeed；
    // 角色加速时倍率
    public float runAccelAmount;
   	// 角色减速时倍率
    public float runDeccelAmount;
}
```





在让角色移动时并不是直接给角色添加力就没有了，还需要对角色的力进行把控，如下图：*（图片已带上原作者的水印）*
图片将移动分成了三个部分，
🚩**Input**： 表示我们按下方向键后希望角色预期达到的速度
🚩**Current Velocity**： 表示角色当前的移动速度
🚩**Force Applied**： 表示我们还需要添加的力

添加的力会随着当前速度增大而减小，当速度达到最大就不需要再添加更多的力；最初开始移动时当前速度为0，所以会添加更多的力使得起步更快。
具体如何来控制我们可以利用 **速度差** ，用当前的速度和最大的速度求差，**使用这个差值来添加力**。

![Snipaste_2023-10-20_16-34-48](https://img2023.cnblogs.com/blog/2821249/202310/2821249-20231020170524874-1912412498.png)

根据我们在 PlayerData 中创建好的三个属性值在操作角色的脚本 PlayerMovemnet 中创建角色移动的函数，代码实现效果如下：

```c#
void Run()
{
    // 让需要达到的最大目标速度获得方向（正负）
    float targetSpeed = moveInput.x * data.runMaxSpeed;
    // 加速倍率
    float accelRate;
    
    //根据是否移动获取加速倍率，如果是在移动获取加速的加速倍率；如果是停止移动，获取减速的加速倍率
    accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount : data.runDeccelAmount;

    // 为了使添加力后的速度最终提高到我们设定的加速度，通过速度差来提高速度，并且这样会有更舒适的手感
    float speedDif = targetSpeed - rb.velocity.x;

    // 添加加速度等同于提高加速的倍率
    float movement = speedDif * accelRate;
    // 添加力移动玩家
    rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
}
```



### 跳跃

想让角色跳跃到预期效果一般会对各种参数进行调整，又想让角色**跳的高**，但是又想让角色**悬停在空中的时间短**，就要不断调整角色刚体的重力和跳跃的力度才能找到比较好的一个手感；如果又想通过更改添加的力从而来调整角色跳跃的高度又需要很多次的尝试。

更好的方式是通过物理学定理📝，利用**高度以及跳跃时间**来计算**重力和跳跃需要添加的力**

但是添加力之前首先我们要知道AddForce添加的力是具体如何实现的，给出下面一段代码：

```c#
// 方向朝右，力的大小为 force ，添加的力为爆发力，意味着只添加一瞬间
rb.AddForce(Vector2.right * force, ForceMode2D.Impulse);
```

首先我们直接通过角色身上的 RigidBody2D 来检测 AddForce 添加的力是否契合物理公式

已知角色 RigidBody2D 组件中的 velocity 表示的是角色的当前速度； F = ma ，其中 F 就是 force ，m 就是 RigidBody2D 组件中的mass ，我们在游戏中将质量设置为1， 在 Material 处添加一个2d物理材质，并将摩擦力调整为0（排除摩擦力的影响）；然后在 Update 中通过输入检测按下某个键来给角色添加一个爆发力，并且打印出当前速度（更好观察）。

```c#
void Update()
{
    if (Input.GetKeyDown(KeyCode.Z))
    {
        rb.AddForce(Vector2.right * 3, ForceMode2D.Impulse);// 我直接将force固定为3
    }
    Debug.Log(rb.velocity.x);
}
```

![image-20231022205616021](https://img2023.cnblogs.com/blog/2821249/202310/2821249-20231023115242946-393382950.png)

![image-20231022205631527](https://img2023.cnblogs.com/blog/2821249/202310/2821249-20231023115243416-998257140.png)

![image-20231022205740594](https://img2023.cnblogs.com/blog/2821249/202310/2821249-20231023115243832-1205279888.png)

经过测试发现，初速度为3，通过物理公式计算瞬间的速度：F = ma ，m = 1，a = 3，看见加速度计算出来后等于3就可以意识到在移动的一瞬间角色获得的初速度也是3；这里可以等同于在添加瞬间力的时候，我们**需要多大的初速度 force 的值就为多少**（前提是m = 1）

🎉经结果得出结论 unity 物理引擎契合物理公式，接下来就可以根据物理公式来设计跳跃。



这里已经制作了一幅图来表示如何求我们的重力，根据加速度公式推出重力加速度 g 的公式，可得：

```c#
// gravityStrenth表示我们需要的力，由于重力加速度的方向始终朝下所以需要带上负号
gravityStrenth = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
```

这个重力并不是我们实际上要更改的值，我们要更改的值是 Rigidbody2D 组件中的 **GravityScale** 

但是在 Rigidbody2D 中，GravityScale 实际上定义的是**刚体受全局重力的倍数**，所以需要根据我们的重力来算全局重力的倍率

```c#
gravityScale = gravityStrenth / Physics2D.gravity.y;
```

然后计算我们跳跃时需要添加的力（或者说向上的力提供的加速度/向上的瞬间初速度）

❗注：之前求的重力加速度并不是我们这里的加速度❗

所以为了求这个加速度，我们可以把从地面跳跃到最高点抽象为从最高点自由落体到地面，这样我们就可以求得最终落地时的瞬时速度，而由上面的实验得出初速度的数值大小即是我们需要添加的力的大小（m=1），所以利用物理公式 v = g * t 求得跳跃所添加的力

```c#
// jumpForce 即为最终添加的力，并且因为我们需要的是力的数值大小所以要求重力加速度的绝对值
jumpForce = Mathf.Abs(gravityStrenth) * jumpTimeToApex;
```

![image-20231022193203159](https://img2023.cnblogs.com/blog/2821249/202310/2821249-20231023115244380-588620073.png)

这样一个能够直观从 unity 的 Game 窗口感受到跳跃的效果的代码就写好了，但是这里仍未达到最终比较好的手感。



💡对于跳跃较好的手感，会根据按下跳跃键的时长来决定跳跃的高度以及下落的速度。

长按跳跃键可以跳到最高点并且正常下落，短按跳跃键快速上升以及快速下降。

那具体如何设计才简单又方便，首先要检测跳跃键按下了多久，其次是要如何在提前释放跳跃键后让玩家直接下落并且速度也和最开始跳跃时一样快呢❓

最简单的方法就是通过跳跃的状态来调整 Rigidbody2D 中的 gravityScale，首先我们要把跳跃分为几个状态：

1. 起跳状态
2. 上升状态
3. 上升至最高点状态
4. 下落状态
5. 短按跳跃状态
6. 在下落同时按下方向键下加速下落状态

根据这些状态创建我们所需要的变量：

在 PlayerData 中添加跳跃相关的一些属性变量值，包括我们上面所提到的一些数据：

```c#
[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Jump")]
    // 跳跃高度
    public float jumpHeight;
    // 跳跃到最高点所需时间
    public float jumpTimeToApex;
    // 用于计算玩家跳跃到接近最高点的几乎一瞬间的时间，让玩家能稍微长一点时间悬浮在空中以此来优化跳跃的手感
    public float jumpHangTimeThreshold;
    // 为了让接近最高点时的悬停时间更长让 gravityScale 乘这个倍率来略微变小，所以这个值的大小在0-1之间
    [Range(0f, 1)] public float jumpHangGravityMult;
    // 当玩家短按空格时，需要将 gravityScale 乘这个倍率来达到快速下降的效果
    public float jumpCutGravityMult;
    // 跳跃提供的力
    [HideInInspector] public float jumpForce;
    
    [Header("Gravity")]
    // 在下落同时按下方向键下时提高下落时速度的倍率
    public float fastFallGravityMult;
    // 限制加速下落时的最大速度，防止超出屏幕外
    public float fastFallMaxSpeed;
    // 正常下落时的重力倍率
    public float fallGravityMult;
    // 正常下落的最大速度
    public float fallMaxSpeed;
    // 实际需要的重力
    [HideInInspector] public float gravityStrenth;
    // 通过计算得到的全局重力的倍率
    [HideInInspector] public float gravityScale;
    
    private void OnValidate()
    {
        // 物理学自由落体公式，已知时间和高度求重力g
        gravityStrenth = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
        // unity刚体的GravityScale定义了刚体受全局重力的倍数，所以需要根据我们的重力来算全局重力的倍率
        gravityScale = gravityStrenth / Physics2D.gravity.y;
        // 计算让角色跳跃起来的力的大小/瞬时速度
        jumpForce = Mathf.Abs(gravityStrenth) * jumpTimeToApex;
    }
}
```

在定义好基础变量后我们还需要两个能提高跳跃时手感的变量：

+ 如果原本正处于跳跃后的下落状态，在快要接触地面时想马上跳跃于是立刻按下跳跃键，但是此时由于并未落地所以按下的跳跃键后无法得到反馈落地后就不再进行跳跃，所以我们需要一个跳跃预输入时间 **jumpInputBufferTime**（预输入这个概念可能在很多其他大型游戏里面就已经出现）
+ 如果从一个平台上不慎滑落，处于刚滑落的一瞬间，这个时候反应过来想跳跃但是此时地面检测表示你已经不在地面上所以你无法进行跳跃，最终导致你直接坠落，部分游戏可能会不希望出现这样的情况，所以会有一个 **coyateTime**，这个概念已经在最开始的图中有解释所以这里不进行描述

在 PlayerData 中补充这两个变量，并限制一个范围

```c#
[Range(0.01f, 0.5f)] public float coyateTime;
[Range(0.01f, 0.5f)] public float jumpInputBufferTime;
```



之后在 PlayerMovement 中创建对应的变量以及编写跳跃的函数

首先我们需要利用物理检测来判断角色是否在地面上：

```c#
public class BetterPlayerMovement : MonoBehaviour
{
    // 这个值一般都会设置为 bool 类型，这里设置成 float 类型就是为了实现跳跃的缓冲时间（coyatetime）
    public float LastOnGroundTime { get; private set; } // 用于判断是否在地面上

    [Header("Check")]
    [SerializeField] private Transform groundCheckPoint; //用于地面检测的落脚点
    [SerializeField] private Vector2 groundCheckSize; // 用与地面检测的范围

    [Header("Layer & Tag")]
    [SerializeField] private LayerMask groundLayer; // 用于地面检测的图层

    void Update()
    {
        LastOnGroundTime -= Time.deltaTime;
        CheckOnGround();
    }
    
    void CheckOnGround()
    {
        if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
        {
            // 处于地面时不断刷新缓冲时间
            LastOnGroundTime = data.coyateTime;
        }
    }
}

```



之后跳跃的代码和正常跳跃的代码相差不会很大，所以这里就不详细叙述，直接附上代码并补充了注释

然后创建跳跃所需的变量，并且创建跳跃的函数，判断好跳跃的多个状态：

```c#
public class BetterPlayerMovement : MonoBehaviour
{
    public PlayerData data; // 获取数据
    private Rigidbody2D rb; // 获得刚体


    public float LastOnGroundTime { get; private set; }
    public float LastInputJumpTime { get; private set; } // 判断是否按下跳跃键

    public bool isJumping; // 判断是否正处于跳跃上升状态
    public bool isJumpFalling; // 判断是否处于跳跃后的下落状态
    public bool jumpCut; // 判断是否提前松开跳跃键

    [Header("Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize;

    [Header("Layer & Tag")]
    [SerializeField] private LayerMask groundLayer;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        LastOnGroundTime -= Time.deltaTime;
        LastInputJumpTime -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            LastInputJumpTime = data.jumpInputBufferTime;
        }
        
        // 判断是否短按跳跃键，这里还没有运用这个变量，具体是在调整 gravityScale 时用到
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isJumping && rb.velocity.y > 0)
            {
                jumpCut = true;
            }
        }

        CheckOnGround();
		
        // 是否处于长按跳跃键后的正常下落状态
        if (isJumping && rb.velocity.y < 0)
        {
            isJumping = false;
            isJumpFalling = true;
        }
		// 如果在地面上刷新跳跃相关的变量
        if (LastOnGroundTime > 0 && !isJumping)
        {
            jumpCut = false;
            isJumpFalling = false;
        }
		// 判断如果在地面上并且按下了跳跃就执行跳跃函数
        if (LastInputJumpTime > 0 && LastOnGroundTime > 0)
        {
            isJumping = true;
            isJumpFalling = false;
            jumpCut = false;
            Jump();
        }
    }

    void Jump()
    {
        // 执行跳跃后就刷新所有时间防止多次跳跃
        LastInputJumpTime = 0;
        LastOnGroundTime = 0;

        float force = data.jumpForce;
        // 如果正处于下落时想跳跃，希望能跳跃的效果和在平地上的跳跃效果相同
        if (rb.velocity.y < 0)
            force -= rb.velocity.y;
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

    }

    void CheckOnGround()
    {
        if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
        {
            LastOnGroundTime = data.coyateTime;
        }
    }
}
```



接下来最主要的就是重力的控制：

```c#
// 下落并且按下方向键下
if (rb.velocity.y < 0 && moveInput.y < 0)
{
    rb.gravityScale = data.gravityScale * data.fastFallGravityMult;
    rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -data.fastFallMaxSpeed));
}
// 短按空格
else if (jumpCut)
{
    rb.gravityScale = data.gravityScale * data.jumpCutGravityMult;
    rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -data.fallMaxSpeed));
}
// 正在跳跃中且跳跃接近最高点
else if ((isJumping || isJumpFalling) && Mathf.Abs(rb.velocity.y) < data.jumpHangTimeThreshold)
{
    rb.gravityScale = data.gravityScale * data.jumpHangGravityMult;
}
// 正常下落
else if (rb.velocity.y < 0)
{
    rb.gravityScale = data.gravityScale * data.fallGravityMult;
    rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -data.fallMaxSpeed));
}
// 在地面时恢复正常重力
else
    rb.gravityScale = data.gravityScale;
```

