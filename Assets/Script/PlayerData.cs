
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Speed")]
    public float runMaxSpeed;
    public float runAccelAmount;
    public float runDeccelAmount;
    public float jumpHangAccelerationMult;
    public float jumpHangMaxSpeedMult;
    [Range(0.1f, 1)] public float accelInAir;
    [Range(0.1f, 1)] public float deccelInAir;

    [Header("Jump")]
    public float jumpHeight;
    public float jumpTimeToApex;
    public float jumpHangTimeThreshold;
    public float jumpCutGravityMult;
    [Range(0f, 1)] public float jumpHangGravityMult;
    [HideInInspector] public float jumpForce;


    [Space(20)]

    [Header("Coyate")]
    [Range(0.01f, 0.5f)] public float coyateTime;
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime;


    [Space(20)]
    [Header("Gravity")]
    public float fastFallGravityMult;
    public float fallGravityMult;
    public float fastFallMaxSpeed;
    public float fallMaxSpeed;
    [HideInInspector] public float gravityStrenth;
    [HideInInspector] public float gravityScale;
    private void OnValidate()
    {
        // runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        // runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        // 物理学自由落体公式，已知时间和高度求重力g
        gravityStrenth = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
        // unity刚体的GravityScale定义了刚体受全局重力的倍数，所以需要根据我们的重力来算全局重力的倍率
        gravityScale = gravityStrenth / Physics2D.gravity.y;
        jumpForce = Mathf.Abs(gravityStrenth) * jumpTimeToApex;
    }


}
