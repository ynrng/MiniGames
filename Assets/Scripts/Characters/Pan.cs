using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    [SerializeField] private GameState gameState;// = default;
    [SerializeField] private InputManagerSO _inputManagerSO = default;

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
        // _inputManagerSO.rotateEvent += OnRotate;

    }

    /// <summary>
    /// 取消注册监听
    /// </summary>
    void OnDisable()
    {
        _inputManagerSO.moveEvent -= OnMove;
        // _inputManagerSO.rotateEvent -= OnRotate;

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
        Debug.Log("vec1:" + coord);
        // 静止状态下，Z=-1，在判断Z<-2时，颠勺
        if(coord.z < -2) {
            
        }
        // 静止状态下 x,y=0 ，用x y来决定是否锅的倾斜
    }

}
