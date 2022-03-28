using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//근접공격
public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }; //근접공격, 원거리
    public Type type; //무기 타입
    public int damage; //데미지
    public float rate; //공격속도
    public BoxCollider meleeArea; //근접공격 범위
    public TrailRenderer trailEffect; //휘두를때 효과

    //재장전
    public int maxAmmo; //최대 탄창
    public int curAmmo; //현재 남은 탄

    //원거리
    public Transform bulletPos; //생성위치
    public GameObject bullet; //프리팹 저장
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public void Use()
    {
        if(type == Type.Melee) //근접일때
        {
            StartCoroutine("Swing");
        }
        else if (type == Type.Range && curAmmo > 0) //원거리이면서 남은 탄이 있으면
        {
            curAmmo--; //탄 소모
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing()
    {
        //Trail Renderer 와 BoxCollider을 시간차로 활성화
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        //yield return new WaitForSeconds(0.3f);
        //meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;

        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = false;
    }

    IEnumerator Shot()
    {
        //#1. 총알 발사
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        ///총알에 속도 적용하기
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50; //나가는 방향이 z축이므로 forward

        yield return null;
        //#2. 탄피 배출
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3); //z축의 반대방향, 튀기듯이 만듬
        caseRigid.AddForce(caseVec, ForceMode.Impulse); //가하는 힘, 즉발
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); //회전도 줌
    }
}
