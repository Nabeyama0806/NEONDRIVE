using TMPro;
using UnityEngine;

public class DamageUI : MonoBehaviour
{
	[SerializeField] float m_moveSpeed = 0.4f;    //移動量
	[SerializeField] GameObject m_damegeUI;

	private TextMeshProUGUI m_textMeshProUGUI;

	void Start()
    {
		m_textMeshProUGUI = m_damegeUI.GetComponent<TextMeshProUGUI>();

		//時間経過で削除
		Destroy(gameObject, 1.2f);
	}

	void Update()
	{
		//移動
		transform.rotation = Camera.main.transform.rotation;
		transform.position += Vector3.up * m_moveSpeed * Time.deltaTime;
	}

	public void SetDamage(int damage)
	{
		m_textMeshProUGUI.text = damage.ToString();
	}
}