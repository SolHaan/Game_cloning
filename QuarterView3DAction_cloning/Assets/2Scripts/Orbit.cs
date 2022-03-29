using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target; //���� ��ǥ
    public float orbitSpeed; //���� �ӵ�
    Vector3 offset; //��ǥ���� �Ÿ�

    void Start()
    {
        offset = transform.position - target.position; //�÷��̾� - ����ź
    }

    void Update()
    {
        transform.position = target.position + offset; //����ź�� ��� ����ٴϵ���
        //RotateAround() Ÿ�� ������ ȸ���ϴ� �Լ� (Ÿ�� ��ġ, ȸ����, ȸ����ġ)
        //-> ����: ��ǥ�� �����̸� �ϱ׷����� ������ ����
        transform.RotateAround(target.position,
                               Vector3.up,
                               orbitSpeed * Time.deltaTime);
        offset = transform.position - target.position;
    }
}
