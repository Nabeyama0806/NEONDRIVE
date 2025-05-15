using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
	[SerializeField] Slider m_healthSlider;     //HPを表示するためのスライダー
	[SerializeField] GameObject m_target;       //HPを表示するオブジェクト

	private Health m_health;

	void Start()
	{
		//オブジェクトが持っているHPを取得
		m_health = m_target.GetComponent<Health>();

		//スライダーの値をHPの値に変更
		m_healthSlider.maxValue = m_health.Value;
		m_healthSlider.value = m_health.Value;
	}

	void Update()
	{
		transform.rotation = Camera.main.transform.rotation;
		m_healthSlider.value = m_health.Value;
	}
}