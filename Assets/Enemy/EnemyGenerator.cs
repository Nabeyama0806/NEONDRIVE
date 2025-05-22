using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
	[SerializeField] GameSceneManager m_gameSceneManager;
	[SerializeField] GameObject m_enemy;        //�v���n�u�������G
	[SerializeField] GameObject m_effect;       //������̃G�t�F�N�g
	[SerializeField] Collider m_collider;       //���g�̓����蔻��
	[SerializeField] int m_minSpawnAmount;      //�ŏ��X�|�[����
	[SerializeField] int m_maxSpawnAmount;      //�ő�X�|�[����
	[SerializeField] int m_spawnPosX;           //X���̃X�|�[���͈͂̔��a
	[SerializeField] int m_spawnPosZ;			//Z���̃X�|�[���͈͂̔��a
	[SerializeField] AudioClip m_spawnSE;       //�X�|�[�����̌��ʉ�
	[SerializeField] AudioClip m_clip;          //�����̌��ʉ�
	[SerializeField] GameObject m_spawnEffect;	//�����G�t�F�N�g

	private Vector3 m_spawnPos;
	private int m_spawnAmount;

	void Start()
	{
		m_spawnAmount = Random.Range(m_minSpawnAmount, m_maxSpawnAmount + 1);
		m_gameSceneManager.AddDefeatAmount(m_spawnAmount);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Spawn();
		}
	}

	private void Spawn()
	{
		SoundManager.Play2D(m_spawnSE);

		//�X�|�[�����̃G�t�F�N�g�̐����ƍ폜
		GameObject tmp = Instantiate(m_spawnEffect, transform.position, Quaternion.Euler(90, 0, 0));
		Destroy(tmp, 1.2f);

		//�X�|�[��
		m_spawnPos.x = Random.Range(-m_spawnPosX, m_spawnPosX + 1);
		m_spawnPos.z = Random.Range(-m_spawnPosZ, m_spawnPosZ + 1);

		for (int i = 0; i < m_spawnAmount; ++i)
		{
			//�G�l�~�[�̃C���X�^���X��
			GameObject enemy = Instantiate(m_enemy, transform.position + m_spawnPos, Quaternion.identity);
			enemy.GetComponent<EnemyMove>().SetGameSceneManager(m_gameSceneManager);
        }

		Destroy(gameObject);
	}

	private void OnDestroy()
	{
		SoundManager.Play2D(m_clip);

		//���S�G�t�F�N�g�𐶐����č폜
		GameObject tmp = Instantiate(m_effect, transform.position, Quaternion.identity);
		Destroy(tmp, 0.8f);
	}
}