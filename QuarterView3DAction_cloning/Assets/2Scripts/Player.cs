using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed; //속도
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades; //공전물체
    public int hasGrenades;

    //아이템 변수
    public int ammo;
    public int coin;
    public int health;
    public int score;
    public int hasGrenade;

    //수류탄
    public GameObject grenadeObj;
    
    //보는 위치로 총쏘기
    public Camera followCamera;

    public GameManager manger;
    public AudioSource jumpSound;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenade;

    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool fDown; //공격
    bool gDown; //수류탄 공격
    bool rDown; //장전
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady = true; //딜레이 후 준비완료
    bool isBorder; //벽 충돌 플래그
    bool isDamage; //무적타임
    bool isShop; //쇼핑중
    bool isDead;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;
    MeshRenderer[] meshs;

    GameObject nearObject;
    public Weapon equipWeapon;
    int equipWeaponIndex = -1; // 조건에서 0으로 뒀기 때문에
    float fireDelay; //공격딜레이

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>(); //자식오브젝트의 컴포넌트를 가져옴
        meshs = GetComponentsInChildren<MeshRenderer>();

        //Debug.Log(PlayerPrefs.GetInt("MaxScore"));
        //PlayerPrefs.SetInt("MaxScore", 112500);
    }

    void Update()
    {
        GetInput(); //입력 변수
        Move();
        Turn();
        Jump();
        Grenade();
        Attack();
        Reload();
        Dodge();
        Swap();
        Interation();
    }

    //코드를 기능에 따라 구분되게 함수로 분리하기
    void GetInput()
    {
        //키보드 입력
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical"); //projectsetting - inputmanager에서 관리하고 있음
        //Shift는 누를 때만 작동되도록 GetButton() 함수 사용
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1"); //마우스 왼쪽
        gDown = Input.GetButtonDown("Fire2"); //마우스 오른쪽
        rDown = Input.GetButtonDown("Reload"); //R
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //움직임 벡터

        //회피를 할 때 회피를 하고 있는 방향으로
        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap || isReload || !isFireReady || isDead) //공격 중에는 이동 불가 되도록 설정, 장전 시
            moveVec = Vector3.zero;

        //이동 * 속도, transform은 deltatime까지 넣기
        //삼항 연산자(bool 형태 조건 ? true일 때 값 : false 일 때 값, if문 대신 사용
        if (!isBorder) //이동 제한
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero); //기본을 달리기
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        //#1. 키보드에 의한 회전
        transform.LookAt(transform.position + moveVec); //나아가는 방향으로 회전한다

        //#2. 마우스에 의한 회전
        if(fDown && !isDead) //마우스 클릭 시에만
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            //ray가 닿은 floor 위치
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0; //높이 무시
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap && !isDead)
        {
            //AddForce() 함수로 물리적인 힘을 가하기
            //ForceMode는 4가지 모드가 있음, Impulse: 즉발적임
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;

            jumpSound.Play();
        }
    }

    //수류탄
    void Grenade()
    {
        //수류탄 쓰기 이전에 제한 조건들
        if (hasGrenade == 0)
            return;

        if(gDown && !isReload && !isSwap && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                //생성된 수류탄의 리지드바디를 활용하여 던지는 로직 구현
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse); //회전

                hasGrenade--;
                grenades[hasGrenade].SetActive(false);
            }
        }
    }

    //공격
    void Attack()
    {
        if (equipWeapon == null) //무기가 있을때만 실행되도록 현재장비 체크
            return;

        //공격딜레이에 시간을 더해주고 공격가능 여부를 확인
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay; //공속보다 높게 설정해서 딜레이

        if(fDown && isFireReady && !isDodge && !isSwap && !isShop && !isDead) //회피, 교체할땐 안함
        {
            equipWeapon.Use(); //무기에 있는 함수 실행
            //삼항연산자를 사용해서 근접이면 스윙, 원거리면 쏘기
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0; //다음 공격까지 기다리도록 딜레이 0
        }
    }

    //재장전
    void Reload()
    {
        if (equipWeapon == null) //무기 없을때
            return;

        if (equipWeapon.type == Weapon.Type.Melee) //근접
            return;

        if (ammo == 0) //총알 없음
            return;

        if(rDown && !isJump && !isDodge && !isSwap && isFireReady && !isShop && !isDead)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2f); //장전 3초
        }
    }

    void ReloadOut()
    {
        //플레이어가 소지한 탄을 고려해서 계산하기
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = equipWeapon.maxAmmo; //무기는 탄이 들어가고
        ammo -= reAmmo; //들어간 개수만큼 소지 탄은 사라짐
        isReload = false;
    }

    //회피
    void Dodge()
    {
        //움직임을 조건으로 추가해서 점프와 회피로 나누기
        //액션 도중 다른 액션이 실행되지 않도록 조건 추가(!isDodge)
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap && !isShop && !isDead)
        {
            dodgeVec = moveVec;
            speed *= 2; //회피는 이동속도만 2배로 상승하도록
            anim.SetTrigger("doDodge");
            isDodge = true;

            //시간차
            Invoke("DodgeOut", 0.4f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f; //2배였으니까 0.5를 곱하면 원래대로 돌아옴
        isDodge = false;
    }

    //무기 교체
    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isShop && !isDead)
        {
            //빈손일 경우
            if(equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    //무기 입수
    void Interation()
    {
        if (iDown && nearObject != null && !isJump && !isDodge && !isShop && !isDead)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
            else if (nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }
        }
    }

    //스스로 도는 현상 고치기
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    //벽 통과 현상
    void StopToWall()
    {
        //Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        //Raycast: Ray를 쏘아 닿는 오브젝트를 감지하는 함수
        //(위치, 방향, 길이, 레이어마스크)
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    //착지 구현
    void OnCollisionEnter(Collision collision)
    {
        //바닥에 부딪히면 점프 가능
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            //enum 타입변수 + switch문으로 간단명료하게 조건문 생성
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value; //아이템 수치를 플레이어 수치에 적용
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    if (hasGrenade == maxHasGrenade)
                        return;
                    grenades[hasGrenade].SetActive(true); //수류탄 개수대로 공전체가 활성화 되도록 구현
                    hasGrenade += item.value;
                    break;
            }
            Destroy(other.gameObject);
        }
        else if (other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage; //체력깎음

                bool isBossAtk = other.name == "Boss Melee Area";
                StartCoroutine(OnDamege(isBossAtk)); //리액션 코루틴
            }

            //리지드바디 유무
            if (other.GetComponent<Rigidbody>() != null)
                Destroy(other.gameObject);
        }
    }
    
    IEnumerator OnDamege(bool isBossAtk)
    {
        isDamage = true;
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }

        if (isBossAtk)
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);

        if (health <= 0 && !isDead)
            OnDie();

        yield return new WaitForSeconds(1f); //무적 타임 설정

        isDamage = false; //무적해제
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

        if (isBossAtk)
            rigid.velocity = Vector3.zero;
    }

    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        manger.GameOver();
    }

    //오브젝트 감지
    void OnTriggerStay(Collider other)
    {
        //지금 다 있는 오브젝트가 weapon이다
        if (other.tag == "Weapon" || other.tag == "Shop")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
        else if (other.tag == "Shop") //퇴장 함수 호출
        {
            Shop shop = nearObject.GetComponent<Shop>();
            shop.Exit();
            isShop = false;
            nearObject = null;
        }
    }
}
