using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
	[SerializeField] int m_power;					//攻撃力
	[SerializeField] int m_knockBack;				//吹き飛ばす力
	[SerializeField] float m_stunTime;				//硬直時間
	[SerializeField] GameObject[] m_deathEffect;	//死亡時のエフェクト
	[SerializeField] GameObject m_hitEffect;		//死亡時のエフェクト
	[SerializeField] AudioClip[] m_clip;

	private GameObject m_gameSceneManager;
	private Animator m_animator;
	private Rigidbody m_rb;
	private CapsuleCollider m_capsuleCollider;
	private NavMeshAgent m_agent;	//AI
	private Transform m_target;     //目標地点
	private float m_elapsedTime;	//怯み中の経過時間
	private bool m_isStun;			//怯み中
	private bool m_isDeath;			//死亡

	void Start()
	{
		//ヒエラルキー上のオブジェクトを取得
		m_target = GameObject.FindWithTag("Player").transform;
		m_gameSceneManager = GameObject.FindWithTag("GameSceneManager");

		//自身のコンポーネントを取得
		m_agent = GetComponent<NavMeshAgent>();
		m_rb = GetComponent<Rigidbody>();
		m_animator = GetComponent<Animator>();
		m_capsuleCollider = GetComponent<CapsuleCollider>();

		m_isStun = false;
		m_isDeath = false;
	}

	void FixedUpdate()
	{
		//目標に向かって移動
		if(m_isStun || m_isDeath)
		{
			//状態異常の時は目標を追わない
			m_agent.SetDestination(transform.position);

			//怯み時間の加算
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

		//当たり判定を非表示にする
		m_capsuleCollider.enabled = false;

		//死亡エフェクトの再生と削除
		GameObject tmp = Instantiate(m_deathEffect[0], transform.position, Quaternion.identity);
		Destroy(tmp, 0.8f);

		//一定時間後に自身を削除
		Destroy(gameObject, 1);
	}

	public void OnDamage()
	{
		//吹き飛ばし
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
		//接触相手がHealthコンポーネントを持っていればダメージを与える
		if (other.TryGetComponent<Health>(out var health))
		{
			health.Damage(m_power, other.transform);
		}
	}

	private void OnDestroy()
	{
		SoundManager.Play2D(m_clip[1]);

		//討伐数の加算
		m_gameSceneManager.GetComponent<GameSceneManager>().AddDefeatRate();

		//死亡エフェクトを生成して削除
		GameObject tmp = Instantiate(m_deathEffect[1], transform.position, Quaternion.identity);
		Destroy(tmp, 1.3f);
	}
}