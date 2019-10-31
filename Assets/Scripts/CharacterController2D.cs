using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

public class CharacterController2D : MonoBehaviour
{
    //대괄호 안의 내용들은 인스펙터 편집에만 영향
	[SerializeField] private float m_JumpForce = 400f;							// 점프 높이 조절
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// 앉은 상태에서 이동 속도
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// 움직임의 부드러운 정도
	[SerializeField] private LayerMask m_WhatIsGround;							// 땅으로 취급되는 레이어들
    [SerializeField] private LayerMask m_WhatIsPlatform;                          // 하향점프 발판으로 취급되는 레이어들
    [SerializeField] private Transform m_GroundCheck;							// 발 바로 밑
	[SerializeField] private Transform m_CeilingCheck;							// 머리 바로 위
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// 숙일 때는 캐릭터 크기가 작아지므로, 콜리더 두 개 중 하나가 사라져야 하는데, 이때 사라지는 콜리더
    [SerializeField] private Collider2D m_JumpDisableCollider;                  // 점프도 마찬가지

	const float k_GroundedRadius = .25f; // 오버랩 함수에 사용되는 고정 변수값
    const float k_CeilingRadius = .1f;  // 이하동문
    private bool m_Grounded;            // 플레이어가 땅 위에 서있는지를 오버랩서클로 판별
    private bool m_wasCrouching = false; // 앉기상태인지 판별
    private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // 캐릭터가 오른쪽을 보는지를 확인
	private Vector3 m_Velocity = Vector3.zero;
    private PlatformEffector2D effector;
    private bool isJumpCooldown=false;

    public PlayerMovement playermovement;
    public Transform Axis;

    event Action OnLandEvent;
    event Action OnJumpEvent;
    event Action OnCrouchEventTrue;
    event Action OnCrouchEventFalse;
    

    private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>(); //리지드바디 레퍼런스 받기

        OnLandEvent += playermovement.OnLanding;
        OnJumpEvent += playermovement.OnJumping;
        OnCrouchEventTrue += playermovement.OnCrouchingTrue;
        OnCrouchEventFalse += playermovement.OnCrouchingFalse;
    }

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded; //이전에 땅에 닿은 상태인지를 판별, 즉 점프 중이었는지 판별
		m_Grounded = false;

        // 주변부 확인, 땅 레이어가 있다면 참 반환
		if(Physics2D.OverlapCircle(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround)){ 
            m_Grounded = true;
            if (!wasGrounded) //점프중이었다면 idle 상태로 바꿔야 하므로 이벤트 invoke
            {
                OnLandEvent.Invoke();
            }
        }

	}


	public void Move(float move, bool crouch, bool jump)
	{
        bool JumpUnder = false;
        // 위에 블럭이 있는지 판별, 앉기 상태 유지 필요성을 따짐
        if (!crouch)
		{
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
        }

		if (m_Grounded)
		{
            if (m_JumpDisableCollider != null)
            {
                m_JumpDisableCollider.enabled = true;
            }
            // 앉기 상태라는 입력을 받을 경우
            if (crouch)
			{
                Collider2D promp = Physics2D.OverlapCircle(m_GroundCheck.position, k_GroundedRadius, m_WhatIsPlatform);
                if (promp && jump)
                {
                    effector=promp.GetComponent<PlatformEffector2D>();
                    JumpUnder = true;
                    StartCoroutine(VerticalJump());
                }
                if (!m_wasCrouching)//캐릭터 역시 앉은 상태로 바뀌고
				{
					m_wasCrouching = true;
                    // 앉기 상태일 떄의 콜리더 설정
                    Axis.position = new Vector3(Axis.position.x, Axis.position.y-0.82f, Axis.position.z);
                    
                    OnCrouchEventTrue.Invoke();//앉기 상태로 바꾸면서 이벤트 실행.
				}
                if (m_CrouchDisableCollider != null)
                {
                    m_CrouchDisableCollider.enabled = false;
                }

                // 속도 감소
                move *= m_CrouchSpeed;
				
			} else
			{
				if (m_wasCrouching)
				{
					m_wasCrouching = false;
                    // 앉기 상태가 아닐 경우 콜리더 설정
                    Axis.position = new Vector3(Axis.position.x, Axis.position.y+0.82f, Axis.position.z);
                    if (m_CrouchDisableCollider != null)
                    {
                        m_CrouchDisableCollider.enabled = true;
                    }
                    OnCrouchEventFalse.Invoke();//아이들 상태로 바꾸면서 인벤트 실행.
				}
			}
		}

        // 원하는 방향으로 속력을 추가해 이동
        Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
        // 움직임을 자연스럽게
        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        // 현재 땅에 붙어있고 점프 입력이 참일 경우
        if (m_Grounded && jump && !JumpUnder && !isJumpCooldown)
		{
			// 수직 힘을 줌
			m_Grounded = false; //땅에 붙어있는지는 실시간으로 확인

			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            StartCoroutine(CooldownJump());
            OnJumpEvent.Invoke();
            if (m_JumpDisableCollider != null)
            {
                m_JumpDisableCollider.enabled = false;
            }

            if (m_CrouchDisableCollider != null)
            {
                m_CrouchDisableCollider.enabled = true;
            }
        }
        
	}

    IEnumerator VerticalJump()
    {
        effector.rotationalOffset = 180f;
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.03f);
        }
        effector.rotationalOffset = 0f;
    }

    IEnumerator CooldownJump()
    {
        isJumpCooldown = true;
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.05f);
        }
        isJumpCooldown = false;
    }

    public void Flip(bool right)
	{
		m_FacingRight = right; //방향이 바뀌므로 보는 방향에 대한 정보가 바뀌어야 함
        transform.Rotate(0f, 180f, 0f);
	}
    /*
    bool isJump()
    {
        if (m_Const == 0)
        {
            return true;
        }
        return false;
    }
    */
}
