using TMPro;
using UnityEngine;

public class DamageUI : MonoBehaviour
{
	[SerializeField] float m_moveSpeed = 0.4f;    //ˆÚ“®—Ê
	[SerializeField] GameObject m_damegeUI;

	private TextMeshProUGUI m_textMeshProUGUI;

	void Start()
    {
		m_textMeshProUGUI = m_damegeUI.GetComponent<TextMeshProUGUI>();

		//ŠÔŒo‰ß‚Åíœ
		Destroy(gameObject, 1.2f);
	}

	void Update()
	{
		//ˆÚ“®
		transform.rotation = Camera.main.transform.rotation;
		transform.position += Vector3.up * m_moveSpeed * Time.deltaTime;
	}

	public void SetDamage(int damage)
	{
		m_textMeshProUGUI.text = damage.ToString();
	}
}