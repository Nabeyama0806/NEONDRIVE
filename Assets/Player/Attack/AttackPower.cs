using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPower : MonoBehaviour
{
    [SerializeField] int m_minPower = 10;
    [SerializeField] int m_maxPower = 10;
    [SerializeField] bool m_canStun = false;
    [SerializeField] bool m_isStunOnly = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy") && other.TryGetComponent<Health>(out var health))
		{
			int damage = Random.Range(m_minPower, m_maxPower);
			if(!m_isStunOnly) health.Damage(damage, other.transform);
			if(m_canStun || m_isStunOnly) health.Stun();
		}
	}
}