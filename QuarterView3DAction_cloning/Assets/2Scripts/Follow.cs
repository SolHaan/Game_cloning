using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target; //���� ��ǥ
    public Vector3 offset; //������(������)

    void Update()
    {
        transform.position = target.position + offset; //Ÿ���� ��ġ���� �������� ���� ��
    }
}
