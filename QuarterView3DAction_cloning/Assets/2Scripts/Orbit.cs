using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target; //공전 목표
    public float orbitSpeed; //공전 속도
    Vector3 offset; //목표와의 거리

    void Start()
    {
        offset = transform.position - target.position; //플레이어 - 수류탄
    }

    void Update()
    {
        transform.position = target.position + offset; //수류탄이 계속 따라다니도록
        //RotateAround() 타겟 주위를 회전하는 함수 (타겟 위치, 회전축, 회전수치)
        //-> 단점: 목표가 움직이면 일그러지는 단점이 있음
        transform.RotateAround(target.position,
                               Vector3.up,
                               orbitSpeed * Time.deltaTime);
        offset = transform.position - target.position;
    }
}
