using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [Header("无线地图")]
    private GameObject mainCamera;
    public float mapWidth;
    public int mapNums;

    private float totalWidth;
    
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");    //  摄像机
        mapWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x - 0.5f;  //  背景宽度（-0.5f是为了防止地图移动后出现缝隙）
        totalWidth = mapWidth * mapNums;    //  背景总长度
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tempPosition = transform.position;
        if (mainCamera.transform.position.x > transform.position.x + totalWidth / 2)
        {
            tempPosition.x += totalWidth;
            transform.position = tempPosition;
        }
        else if(mainCamera.transform.position.x < transform.position.x - totalWidth / 2)
        {
            tempPosition.x -= totalWidth;
            transform.position = tempPosition;
        }
    }
}