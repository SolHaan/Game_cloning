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
        offset = transform.position - target.position;
    }

    void Update()
    {
        transform.position = target.position + offset;
        //RotateAround() 타겟 주의를 회전하는 함수 (타겟 위치, 회전축, 회전수치)
        //-> 단점: 목표가 움직이면 일그러지는 단점이 있음
        transform.RotateAround(target.position,
                               Vector3.up,
                               orbitSpeed * Time.deltaTime);
    }
}
