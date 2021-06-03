using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    [SerializeField] private GameSO _gameSO;
    [SerializeField] private InputManagerSO _inputManagerSO;

    // Start is called before the first frame update
    void Start()
    {
    }

    /// <summary>
    /// 注册监听输入事件
    /// </summary>
    void OnEnable()
    {
        _inputManagerSO.moveEvent += OnMove;

    }

    /// <summary>
    /// 取消注册监听
    /// </summary>
    void OnDisable()
    {
        _inputManagerSO.moveEvent -= OnMove;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Recalculate()
    {

    }

    public void StartPlaying()
    {

    }

    private void OnRotate(Vector3 coord)
    {
        // 其实是旋转的矢量
    }

    private void OnMove(Vector3 coord)
    {
        if (_gameSO.gameState == GameState.Playing)
        {
            // 静止状态下，Z=-1，在判断Z<-2时，颠勺？// todo 判断一下rotate的增量
            if (coord.z < -2)
            {

            }
            // 静止状态下 x,y=0 ，用x y来决定是否锅的倾斜，最大倾斜角
            gameObject.transform.rotation = Quaternion.Euler(
                Mathf.Clamp(coord.x, -30f, 30f),
                Mathf.Clamp(-coord.z, -30f, 30f),
                Mathf.Clamp(-coord.z, -15f, 15f));

            Debug.Log("[Pan]OnMove:" + coord + ";" + gameObject.transform.rotation);

        }

    }

}
