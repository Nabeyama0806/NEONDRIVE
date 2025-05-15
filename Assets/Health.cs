using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
	[SerializeField] int m_health;
	[SerializeField] UnityEvent m_onDeath;
	[SerializeField] UnityEvent m_onDamage;
	[SerializeField] UnityEvent m_onStun;
	[SerializeField] GameObject m_damageUI;

	public int Value
	{
		get => m_health;
	}

	public void Damage(int damage, Transform transform)
	{
		if (damage <= 0) return;    //���̃_���[�W�͉񕜂��Ă��܂�
		if (m_health <= 0) return;  //���̏R��͂��Ȃ�

		//�_���[�W
		m_health -= damage;
		//if (m_damageUI)
		//{
		//	GameObject tmp = Instantiate(m_damageUI, transform.position, Camera.main.transform.rotation);
		//	tmp.GetComponent<DamageUI>().SetDamage(damage);
		//}

		//�̗͂̊m�F
		if (m_health <= 0)
		{
			//���S�ʒm
			m_onDeath?.Invoke();
		}
		else
		{
			//��e�ʒm
			m_onDamage?.Invoke();
		}
	}

	public void Stun()
	{
		m_onStun?.Invoke();
	}
}