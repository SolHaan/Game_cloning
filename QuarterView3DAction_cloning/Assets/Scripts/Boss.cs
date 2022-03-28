using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    //미사일 관련 변수
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    public bool isLook; //플레이어 바라보는 플래그

    //플레이어 움직임 예측 벡터
    Vector3 lookVec;
    Vector3 tauntVec; //어디에 저장할지

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true;
        StartCoroutine(Think());
    }

    void Update()
    {
        //패턴 정지 로직
        if(isDead)
        {
            StopAllCoroutines();
            return;
        }

        if (isLook) //바라보고 있는 중일때
        {
            //플레이어 입력값으로 예측 벡터값 생성
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
        else
            nav.SetDestination(tauntVec);
    }
    
    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f); //이게 길어지면 보스 패턴이 쉬워짐

        int ranAction = Random.Range(0, 5);
        switch (ranAction)
        {
            case 0:
            case 1:
                //미사일 발사 패턴
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                //돌 굴러가는 패턴
                StartCoroutine(RockShot());
                break;
            case 4:
                //점프 공격 패턴
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot"); //각 패턴에 맞는 애니메이션을 SetTrigger() 함수로 실행
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation); //미사일 생성
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f); //0.5초 뒤 한발 더 나감
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation); //미사일 생성
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2.5f);

        StartCoroutine(Think()); // 패턴이 끝나면 다음 패턴을 위해 다시 Think() 코루틴 실행
    }

    IEnumerator RockShot()
    {
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);

        isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator Taunt()
    {
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        boxCollider.enabled = true;
        StartCoroutine(Think());
    }
}
