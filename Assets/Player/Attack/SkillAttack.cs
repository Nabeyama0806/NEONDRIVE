using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillAttack : MonoBehaviour
{
	[SerializeField] Animator m_animator;
    [SerializeField] float m_skillCoolTime = 6;     //発動までの必要時間
    [SerializeField] float m_fallCoolTime = 6;		//発動までの必要時間
	[SerializeField] GameObject[] m_skillEffect = new GameObject[(int)Attack.Length];   //技の見た目
	[SerializeField] AudioClip[] m_attackSE = new AudioClip[(int)Attack.Length];        //効果音
	[SerializeField] Image[] m_attackIcon = new Image[(int)Attack.Length];				//スキルアイコン

	private PlayerInput m_playerInput;		//入力
	private PlayerMove m_playerMove;		//移動のスクリプト
	private StringCaster m_stringCaster;	//ワイヤーアクションのスクリプト
	private float m_skillWaitTime;			//クールタイムの経過時間
	private float m_fallWaitTime;           //クールタイムの経過時間

	enum Attack
	{
		FallAttack,		//落下攻撃
		Skill,			//特殊技
		
		Length,
	}

	private void Awake()
	{
		m_animator.GetComponent<Animator>();
		m_playerInput = GetComponent<PlayerInput>();
		m_stringCaster = GetComponent<StringCaster>();
		m_playerMove = GetComponent<PlayerMove>();
		m_skillWaitTime = 0;
	}

	private void OnEnable()
	{
		m_playerInput.actions["Attack"].performed += OnAttack;
		m_playerInput.actions["Skill"].performed += OnSkill;
	}

	private void OnDisable()
	{
		m_playerInput.actions["Attack"].performed -= OnAttack;
		m_playerInput.actions["Skill"].performed -= OnSkill;
	}

	private void OnSkill(InputAction.CallbackContext context)
	{
		if (m_skillWaitTime < m_skillCoolTime) return;

		SoundManager.Play2D(m_attackSE[(int)Attack.Skill], 0.2f);

		//自身を中心に8方向の攻撃範囲
		for (int i = 0; i < 8; ++i)
		{
			//エフェクトの生成と削除
			GameObject tmp = Instantiate(
				m_skillEffect[(int)Attack.Skill],
				transform.position,
				Quaternion.Euler(new Vector3(0, i * 45, 0))
				);
			Destroy(tmp, 4);
		}

		m_skillWaitTime = 0;
	}

	private void OnAttack(InputAction.CallbackContext context)
	{
		if (m_stringCaster.IsCasting()) return;
		m_animator.SetTrigger("Attack");
	}

	void FixedUpdate()
    {
		//技のクールタイム
		m_attackIcon[(int)Attack.Skill].fillAmount = 1 - m_skillWaitTime / m_skillCoolTime;
		if (m_skillWaitTime < m_skillCoolTime)
		{
			m_skillWaitTime += Time.deltaTime;
		}
		m_attackIcon[(int)Attack.FallAttack].fillAmount = 1 - m_fallWaitTime / m_fallCoolTime;
		if(m_fallWaitTime < m_skillCoolTime)
		{
			m_fallWaitTime += Time.deltaTime;
		}

		//落下攻撃
		if(m_playerMove.GetIsFallAttack() && m_fallWaitTime > m_fallCoolTime)
		{
			SoundManager.Play2D(m_attackSE[(int)Attack.FallAttack]);

			//エフェクトの生成と削除
			GameObject tmp = Instantiate(
				m_skillEffect[(int)Attack.FallAttack],
				transform.position,
				Quaternion.identity
				);
			Destroy(tmp, 0.5f);
			m_fallWaitTime = 0;
		}
		if(m_playerMove.GetIsFallAttack())
		{
			m_playerMove.SetIsFallAttack();
		}
    }
}