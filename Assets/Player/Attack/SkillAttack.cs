using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillAttack : MonoBehaviour
{
	[SerializeField] Animator m_animator;
    [SerializeField] float m_skillCoolTime = 6;     //�����܂ł̕K�v����
    [SerializeField] float m_fallCoolTime = 6;		//�����܂ł̕K�v����
	[SerializeField] GameObject[] m_skillEffect = new GameObject[(int)Attack.Length];   //�Z�̌�����
	[SerializeField] AudioClip[] m_attackSE = new AudioClip[(int)Attack.Length];        //���ʉ�
	[SerializeField] Image[] m_attackIcon = new Image[(int)Attack.Length];				//�X�L���A�C�R��

	private PlayerInput m_playerInput;		//����
	private PlayerMove m_playerMove;		//�ړ��̃X�N���v�g
	private StringCaster m_stringCaster;	//���C���[�A�N�V�����̃X�N���v�g
	private float m_skillWaitTime;			//�N�[���^�C���̌o�ߎ���
	private float m_fallWaitTime;           //�N�[���^�C���̌o�ߎ���

	enum Attack
	{
		FallAttack,		//�����U��
		Skill,			//����Z
		
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

		//���g�𒆐S��8�����̍U���͈�
		for (int i = 0; i < 8; ++i)
		{
			//�G�t�F�N�g�̐����ƍ폜
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
		//�Z�̃N�[���^�C��
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

		//�����U��
		if(m_playerMove.GetIsFallAttack() && m_fallWaitTime > m_fallCoolTime)
		{
			SoundManager.Play2D(m_attackSE[(int)Attack.FallAttack]);

			//�G�t�F�N�g�̐����ƍ폜
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