using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
	[SerializeField] Slider m_healthSlider;     //HP��\�����邽�߂̃X���C�_�[
	[SerializeField] GameObject m_target;       //HP��\������I�u�W�F�N�g

	private Health m_health;

	void Start()
	{
		//�I�u�W�F�N�g�������Ă���HP���擾
		m_health = m_target.GetComponent<Health>();

		//�X���C�_�[�̒l��HP�̒l�ɕύX
		m_healthSlider.maxValue = m_health.Value;
		m_healthSlider.value = m_health.Value;
	}

	void Update()
	{
		transform.rotation = Camera.main.transform.rotation;
		m_healthSlider.value = m_health.Value;
	}
}