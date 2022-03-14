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
    public int hasGrenade;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenade;

    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject;
    GameObject equipWeapon;
    int equipWeaponIndex = -1; // 조건에서 0으로 뒀기 때문에

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>(); //자식오브젝트의 컴포넌트를 가져옴
    }

    void Update()
    {
        GetInput(); //입력 변수
        Move();
        Turn();
        Jump();
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
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        //normalized: 방향 값이 1로 보정된 벡터(가로 세로가 1이어도 대각서은 더 긴 루트2이기 때문에
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //움직임 벡터

        //회피를 할 때 회피를 하고 있는 방향으로
        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap)
            moveVec = Vector3.zero;

        //이동 * 속도, transform은 deltatime까지 넣기
        //삼항 연산자(bool 형태 조건 ? true일 때 값 : false 일 때 값, if문 대신 사용
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero); //기본을 달리기
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        //LookAt(): 지정된 벡터를 향해서 회전시켜주는 함수
        transform.LookAt(transform.position + moveVec); //나아가는 방향으로 회전한다
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            //AddForce() 함수로 물리적인 힘을 가하기
            //ForceMode는 4가지 모드가 있음, Impulse: 즉발적임
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    //회피
    void Dodge()
    {
        //움직임을 조건으로 추가해서 점프와 회피로 나누기
        //액션 도중 다른 액션이 실행되지 않도록 조건 추가(!isDodge)
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
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

        if((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            //빈손일 경우
            if(equipWeapon != null)
                equipWeapon.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex];
            equipWeapon.SetActive(true);

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
        if(iDown && nearObject != null && !isJump && !isDodge)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
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
                    hasGrenade += item.value;
                    if (hasGrenade == maxHasGrenade)
                        return;
                    grenades[hasGrenade].SetActive(true);
                    hasGrenade += item.value;
                    break;
            }
            Destroy(other.gameObject);
        }
    }

    //오브젝트 감지
    void OnTriggerStay(Collider other)
    {
        //지금 다 있는 오브젝트가 weapon이다
        if (other.tag == "Weapon")
            nearObject = other.gameObject;

        //Debug.Log(nearObject.name);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}
