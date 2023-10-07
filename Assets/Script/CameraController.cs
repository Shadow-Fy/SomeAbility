using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;    //  玩家位置
    public Transform[] backGround;  //  背景位置
    public Vector2 lastPos; //  最后一次的相机位置
    public int mapNums;

    
    void Start()
    {
        lastPos = transform.position;   //  记录相机初始位置
    }

    // Update is called once per frame
    void Update()
    {
        //相机位置给玩家
        transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);

        //  计算相机在上一帧和当前帧之间移动的距离
        Vector2 amountToMove = new Vector2(transform.position.x - lastPos.x, transform.position.y - lastPos.y);
        
        //  根据相机移动的距离，移动远背景和中间背景的位置
        foreach (Transform varTransform in backGround)
        {
            varTransform.position +=
                new Vector3(amountToMove.x * (varTransform.position.z)*mapNums / backGround.Length,
                    amountToMove.y * (varTransform.position.z)*mapNums / backGround.Length, 0f);
        }

        lastPos = transform.position;
    }
}