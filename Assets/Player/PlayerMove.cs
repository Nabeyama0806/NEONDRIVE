using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
	[SerializeField] Animator m_animator;
	[SerializeField] GameObject m_playerModel;
	[SerializeField] GameObject m_hitEffect;	//被弾エフェクト
	[SerializeField] float m_moveSpeed = 7.5f;	//移動速度
	[SerializeField] float m_fallSpeed = 5.5f;  //落下の初速
	[SerializeField] float m_jumpPower = 5.5f;  //飛び上がる力
	[SerializeField] AudioClip m_fallSE;
	[SerializeField] Health m_health;

	private PlayerInput m_playerInput;
	private StringCaster m_stringCaster;
	private Rigidbody m_rb;
	private Vector3 m_moveVelocity;
	private Vector3 m_direction;
	private bool m_isGroundedPrev;
	private bool m_isGrounded;
	private bool m_isDeath;
	private bool m_isFall;
	private bool m_isFallAttack;

	private void Awake()
	{
		//マウスカーソルを非表示にする
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		m_rb = GetComponent<Rigidbody>();
		m_playerInput = m_rb.GetComponent<PlayerInput>();
		m_stringCaster = GetComponent<StringCaster>();
		m_isFall = false;
		m_isFallAttack = false;
		m_isGrounded = true;
		m_isDeath = false;
	}

	private void OnEnable()
	{
		m_playerInput.actions["Move"].performed += OnMove;
		m_playerInput.actions["Move"].canceled += OnMoveCancel;

		m_playerInput.actions["Fall"].performed += OnFall;
	}

	private void OnDisable()
	{
		m_playerInput.actions["Move"].performed -= OnMove;
		m_playerInput.actions["Move"].canceled -= OnMoveCancel;

		m_playerInput.actions["Fall"].performed -= OnFall;
	}

	private void OnMove(InputAction.CallbackContext context)
	{
		var inputMove = context.ReadValue<Vector2>();
		m_direction = new Vector3(inputMove.x, 0, inputMove.y);
		m_animator.SetBool("Shot", false);
		m_animator.SetBool("Run", !m_stringCaster.IsCasting());
	}

	private void OnMoveCancel(InputAction.CallbackContext context)
	{
		m_direction = Vector3.zero;
		m_animator.SetBool("Run", false);
	}

	private void OnFall(InputAction.CallbackContext context)
	{
		//地面にいると空中に飛び上がる
		if (m_isGrounded)
		{
			SoundManager.Play2D(m_fallSE);
			m_rb.AddForce(new Vector3(0, m_jumpPower, 0));
		}

		m_animator.SetTrigger("Jump");
		m_health.enabled = false;
		m_direction.y = -m_fallSpeed;
		m_isFall = true;
	}

	private void FixedUpdate()
	{
		if (m_isDeath) return;

		//接地判定の更新
		Vector3 rayOffset = new Vector3(0, 0.5f, 0);
		Ray ray = new Ray(transform.position + rayOffset, Vector3.down);
		m_isGrounded = Physics.Raycast(ray, 1.5f, 1 << 7);

		//接地した瞬間
		if (m_isGrounded && !m_isGroundedPrev)
		{
			//急降下の直後
			if (m_isFall)
			{
				m_isFall = false;
				m_isFallAttack = true;
				m_direction.y = 0;
				m_health.enabled = true;
			}
		}

		// カメラの正面ベクトルを作成
		Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

		//カメラの向きを考慮した移動量
		m_moveVelocity = cameraForward * m_direction.z + Camera.main.transform.right * m_direction.x;
		m_moveVelocity *= m_moveSpeed;

		//回転
		if (m_moveVelocity != Vector3.zero)
		{
			transform.rotation = Quaternion.Slerp(
				transform.rotation,
				Quaternion.LookRotation(m_moveVelocity.normalized),
				0.2f
				);
		}

		//移動
		m_moveVelocity.y = m_direction.y;
		m_rb.velocity = m_moveVelocity;

		//接地判定の保存
		m_isGroundedPrev = m_isGrounded;
	}

	public void OnDeath()
	{
		m_isDeath = true;
		m_animator.SetBool("Death", m_isDeath);

		//移動量を0にする
		m_rb.velocity = Vector3.zero;
		m_moveVelocity = Vector3.zero;
	}

	public void OnDamage()
	{
		GameObject tmp = Instantiate(m_hitEffect, transform.position + Vector3.up, Quaternion.identity);
		Destroy(tmp, 0.6f);
	}

	public bool GetIsFallAttack()
	{
		return m_isFallAttack;
	}
	public void SetIsFallAttack()
	{
		m_isFallAttack = false;
	}
}