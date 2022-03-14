using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target; //따라갈 목표
    public Vector3 offset; //고정값(오프셋)

    void Update()
    {
        transform.position = target.position + offset; //타겟의 위치에서 고정값을 더한 값
    }
}
