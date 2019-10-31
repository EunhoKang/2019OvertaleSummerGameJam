using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

public class CharacterController2D : MonoBehaviour
{
    //���ȣ ���� ������� �ν����� �������� ����
	[SerializeField] private float m_JumpForce = 400f;							// ���� ���� ����
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// ���� ���¿��� �̵� �ӵ�
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// �������� �ε巯�� ����
	[SerializeField] private LayerMask m_WhatIsGround;							// ������ ��޵Ǵ� ���̾��
    [SerializeField] private LayerMask m_WhatIsPlatform;                          // �������� �������� ��޵Ǵ� ���̾��
    [SerializeField] private Transform m_GroundCheck;							// �� �ٷ� ��
	[SerializeField] private Transform m_CeilingCheck;							// �Ӹ� �ٷ� ��
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// ���� ���� ĳ���� ũ�Ⱑ �۾����Ƿ�, �ݸ��� �� �� �� �ϳ��� ������� �ϴµ�, �̶� ������� �ݸ���
    [SerializeField] private Collider2D m_JumpDisableCollider;                  // ������ ��������

	const float k_GroundedRadius = .25f; // ������ �Լ��� ���Ǵ� ���� ������
    const float k_CeilingRadius = .1f;  // ���ϵ���
    private bool m_Grounded;            // �÷��̾ �� ���� ���ִ����� ��������Ŭ�� �Ǻ�
    private bool m_wasCrouching = false; // �ɱ�������� �Ǻ�
    private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // ĳ���Ͱ� �������� �������� Ȯ��
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
		m_Rigidbody2D = GetComponent<Rigidbody2D>(); //������ٵ� ���۷��� �ޱ�

        OnLandEvent += playermovement.OnLanding;
        OnJumpEvent += playermovement.OnJumping;
        OnCrouchEventTrue += playermovement.OnCrouchingTrue;
        OnCrouchEventFalse += playermovement.OnCrouchingFalse;
    }

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded; //������ ���� ���� ���������� �Ǻ�, �� ���� ���̾����� �Ǻ�
		m_Grounded = false;

        // �ֺ��� Ȯ��, �� ���̾ �ִٸ� �� ��ȯ
		if(Physics2D.OverlapCircle(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround)){ 
            m_Grounded = true;
            if (!wasGrounded) //�������̾��ٸ� idle ���·� �ٲ�� �ϹǷ� �̺�Ʈ invoke
            {
                OnLandEvent.Invoke();
            }
        }

	}


	public void Move(float move, bool crouch, bool jump)
	{
        bool JumpUnder = false;
        // ���� ���� �ִ��� �Ǻ�, �ɱ� ���� ���� �ʿ伺�� ����
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
            // �ɱ� ���¶�� �Է��� ���� ���
            if (crouch)
			{
                Collider2D promp = Physics2D.OverlapCircle(m_GroundCheck.position, k_GroundedRadius, m_WhatIsPlatform);
                if (promp && jump)
                {
                    effector=promp.GetComponent<PlatformEffector2D>();
                    JumpUnder = true;
                    StartCoroutine(VerticalJump());
                }
                if (!m_wasCrouching)//ĳ���� ���� ���� ���·� �ٲ��
				{
					m_wasCrouching = true;
                    // �ɱ� ������ ���� �ݸ��� ����
                    Axis.position = new Vector3(Axis.position.x, Axis.position.y-0.82f, Axis.position.z);
                    
                    OnCrouchEventTrue.Invoke();//�ɱ� ���·� �ٲٸ鼭 �̺�Ʈ ����.
				}
                if (m_CrouchDisableCollider != null)
                {
                    m_CrouchDisableCollider.enabled = false;
                }

                // �ӵ� ����
                move *= m_CrouchSpeed;
				
			} else
			{
				if (m_wasCrouching)
				{
					m_wasCrouching = false;
                    // �ɱ� ���°� �ƴ� ��� �ݸ��� ����
                    Axis.position = new Vector3(Axis.position.x, Axis.position.y+0.82f, Axis.position.z);
                    if (m_CrouchDisableCollider != null)
                    {
                        m_CrouchDisableCollider.enabled = true;
                    }
                    OnCrouchEventFalse.Invoke();//���̵� ���·� �ٲٸ鼭 �κ�Ʈ ����.
				}
			}
		}

        // ���ϴ� �������� �ӷ��� �߰��� �̵�
        Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
        // �������� �ڿ�������
        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        // ���� ���� �پ��ְ� ���� �Է��� ���� ���
        if (m_Grounded && jump && !JumpUnder && !isJumpCooldown)
		{
			// ���� ���� ��
			m_Grounded = false; //���� �پ��ִ����� �ǽð����� Ȯ��

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
		m_FacingRight = right; //������ �ٲ�Ƿ� ���� ���⿡ ���� ������ �ٲ��� ��
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
