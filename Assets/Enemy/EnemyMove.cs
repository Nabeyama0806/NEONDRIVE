using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
	[SerializeField] int m_power;					//�U����
	[SerializeField] int m_knockBack;				//������΂���
	[SerializeField] float m_stunTime;				//�d������
	[SerializeField] GameObject[] m_deathEffect;	//���S���̃G�t�F�N�g
	[SerializeField] GameObject m_hitEffect;		//���S���̃G�t�F�N�g
	[SerializeField] AudioClip[] m_clip;

	private GameObject m_gameSceneManager;
	private Animator m_animator;
	private Rigidbody m_rb;
	private CapsuleCollider m_capsuleCollider;
	private NavMeshAgent m_agent;	//AI
	private Transform m_target;     //�ڕW�n�_
	private float m_elapsedTime;	//���ݒ��̌o�ߎ���
	private bool m_isStun;			//���ݒ�
	private bool m_isDeath;			//���S

	void Start()
	{
		//�q�G�����L�[��̃I�u�W�F�N�g���擾
		m_target = GameObject.FindWithTag("Player").transform;
		m_gameSceneManager = GameObject.FindWithTag("GameSceneManager");

		//���g�̃R���|�[�l���g���擾
		m_agent = GetComponent<NavMeshAgent>();
		m_rb = GetComponent<Rigidbody>();
		m_animator = GetComponent<Animator>();
		m_capsuleCollider = GetComponent<CapsuleCollider>();

		m_isStun = false;
		m_isDeath = false;
	}

	void FixedUpdate()
	{
		//�ڕW�Ɍ������Ĉړ�
		if(m_isStun || m_isDeath)
		{
			//��Ԉُ�̎��͖ڕW��ǂ�Ȃ�
			m_agent.SetDestination(transform.position);

			//���ݎ��Ԃ̉��Z
			m_elapsedTime += Time.deltaTime;
			if (m_elapsedTime < m_stunTime) return;
			m_isStun = false;
		}
		else
		{
			m_agent.SetDestination(m_target.position);
		}
	}

	public void OnDeath()
	{
		SoundManager.Play3D(m_clip[0], transform.position, 0.7f);
		m_animator.SetBool("Death", true);
		m_isDeath = true;

		//�����蔻����\���ɂ���
		m_capsuleCollider.enabled = false;

		//���S�G�t�F�N�g�̍Đ��ƍ폜
		GameObject tmp = Instantiate(m_deathEffect[0], transform.position, Quaternion.identity);
		Destroy(tmp, 0.8f);

		//��莞�Ԍ�Ɏ��g���폜
		Destroy(gameObject, 1);
	}

	public void OnDamage()
	{
		//������΂�
		m_rb.AddForce(-transform.forward * m_knockBack, ForceMode.Acceleration);

		m_animator.SetTrigger("Hit");
	}

	public void OnStun()
	{
		m_animator.SetTrigger("Hit");
		m_isStun = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		//�ڐG���肪Health�R���|�[�l���g�������Ă���΃_���[�W��^����
		if (other.TryGetComponent<Health>(out var health))
		{
			health.Damage(m_power, other.transform);
		}
	}

	private void OnDestroy()
	{
		SoundManager.Play2D(m_clip[1]);

		//�������̉��Z
		m_gameSceneManager.GetComponent<GameSceneManager>().AddDefeatRate();

		//���S�G�t�F�N�g�𐶐����č폜
		GameObject tmp = Instantiate(m_deathEffect[1], transform.position, Quaternion.identity);
		Destroy(tmp, 1.3f);
	}
}