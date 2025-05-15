using UnityEngine;

public class ComboAttack : MonoBehaviour
{
	[SerializeField] GameObject m_sword;    //剣
	[SerializeField] GameObject[] m_skillEffect = new GameObject[(int)Attack.Length];   //技の見た目
	[SerializeField] AudioClip[] m_attackSE = new AudioClip[(int)Attack.Length];        //効果音
	
	enum Attack		//コンボ攻撃
	{
		Fast,			
		Second,     
		Third,
		Fourth,

		Length,
	}

	void Start()
    {
		m_sword.GetComponent<BoxCollider>().enabled = false;
	}

	//一段目
	public void OnAttack1()
	{
		//効果音の再生
		SoundManager.Play2D(m_attackSE[(int)Attack.Fast]);

		//エフェクトの生成と削除
		GameObject tmp = Instantiate(m_skillEffect[(int)Attack.Fast], transform.position, transform.rotation);
		tmp.transform.parent = m_sword.transform;
		Destroy(tmp, 0.6f);

		//当たり判定の出現
		m_sword.GetComponent<BoxCollider>().enabled = true;
	}
	public void OnAttackEnd1()
	{
		//当たり判定の非表示
		m_sword.GetComponent<BoxCollider>().enabled = false;
	}


	//二段目
	public void OnAttack2()
	{
		//効果音の再生
		SoundManager.Play2D(m_attackSE[(int)Attack.Second]);

		//エフェクトの生成と削除
		GameObject tmp = Instantiate(m_skillEffect[(int)Attack.Second], transform.position, transform.rotation);
		tmp.transform.parent = transform;
		Destroy(tmp, 0.4f);

		//当たり判定の出現
		m_sword.GetComponent<BoxCollider>().enabled = true;
	}
	public void OnAttackEnd2()
	{
		//当たり判定の非表示
		m_sword.GetComponent<BoxCollider>().enabled = false;
	}


	//三段目
	public void OnAttack3()
	{
		//効果音の再生
		SoundManager.Play2D(m_attackSE[(int)Attack.Third]);

		//エフェクトの生成と削除
		GameObject tmp = Instantiate(m_skillEffect[(int)Attack.Third], transform.position, transform.rotation);
		tmp.transform.parent = transform;
		Destroy(tmp, 0.4f);

		//当たり判定の出現
		m_sword.GetComponent<BoxCollider>().enabled = true;
	}
	public void OnAttackEnd3()
	{
		//当たり判定の非表示
		m_sword.GetComponent<BoxCollider>().enabled = false;
	}


	//四段目
	public void OnAttack4()
	{
		//効果音の再生
		SoundManager.Play2D(m_attackSE[(int)Attack.Fourth]);

		//攻撃エフェクトの生成と削除
		GameObject tmp = Instantiate(m_skillEffect[(int)Attack.Fourth], transform.position, transform.rotation);
		Destroy(tmp, 0.7f);
	}
	public void OnAttackEnd4()
	{

	}
}
