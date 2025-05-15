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
		if (damage <= 0) return;    //負のダメージは回復してしまう
		if (m_health <= 0) return;  //死体蹴りはしない

		//ダメージ
		m_health -= damage;
		//if (m_damageUI)
		//{
		//	GameObject tmp = Instantiate(m_damageUI, transform.position, Camera.main.transform.rotation);
		//	tmp.GetComponent<DamageUI>().SetDamage(damage);
		//}

		//体力の確認
		if (m_health <= 0)
		{
			//死亡通知
			m_onDeath?.Invoke();
		}
		else
		{
			//被弾通知
			m_onDamage?.Invoke();
		}
	}

	public void Stun()
	{
		m_onStun?.Invoke();
	}
}