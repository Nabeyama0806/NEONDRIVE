using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
	[SerializeField] GameSceneManager m_gameSceneManager;
	[SerializeField] GameObject m_enemy;        //プレハブ化した敵
	[SerializeField] GameObject m_effect;       //生成後のエフェクト
	[SerializeField] Collider m_collider;       //自身の当たり判定
	[SerializeField] int m_minSpawnAmount;      //最小スポーン数
	[SerializeField] int m_maxSpawnAmount;      //最大スポーン数
	[SerializeField] int m_spawnPosX;           //X軸のスポーン範囲の半径
	[SerializeField] int m_spawnPosZ;			//Z軸のスポーン範囲の半径
	[SerializeField] AudioClip m_spawnSE;       //スポーン時の効果音
	[SerializeField] AudioClip m_clip;          //生成の効果音
	[SerializeField] GameObject m_spawnEffect;	//生成エフェクト

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

		//スポーン時のエフェクトの生成と削除
		GameObject tmp = Instantiate(m_spawnEffect, transform.position, Quaternion.Euler(90, 0, 0));
		Destroy(tmp, 1.2f);

		//スポーン
		m_spawnPos.x = Random.Range(-m_spawnPosX, m_spawnPosX + 1);
		m_spawnPos.z = Random.Range(-m_spawnPosZ, m_spawnPosZ + 1);

		for (int i = 0; i < m_spawnAmount; ++i)
		{
			//エネミーのインスタンス化
			GameObject enemy = Instantiate(m_enemy, transform.position + m_spawnPos, Quaternion.identity);
			enemy.GetComponent<EnemyMove>().SetGameSceneManager(m_gameSceneManager);
        }

		Destroy(gameObject);
	}

	private void OnDestroy()
	{
		SoundManager.Play2D(m_clip);

		//死亡エフェクトを生成して削除
		GameObject tmp = Instantiate(m_effect, transform.position, Quaternion.identity);
		Destroy(tmp, 0.8f);
	}
}