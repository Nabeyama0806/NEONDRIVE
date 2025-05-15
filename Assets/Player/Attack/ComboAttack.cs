using UnityEngine;

public class ComboAttack : MonoBehaviour
{
	[SerializeField] GameObject m_sword;    //��
	[SerializeField] GameObject[] m_skillEffect = new GameObject[(int)Attack.Length];   //�Z�̌�����
	[SerializeField] AudioClip[] m_attackSE = new AudioClip[(int)Attack.Length];        //���ʉ�
	
	enum Attack		//�R���{�U��
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

	//��i��
	public void OnAttack1()
	{
		//���ʉ��̍Đ�
		SoundManager.Play2D(m_attackSE[(int)Attack.Fast]);

		//�G�t�F�N�g�̐����ƍ폜
		GameObject tmp = Instantiate(m_skillEffect[(int)Attack.Fast], transform.position, transform.rotation);
		tmp.transform.parent = m_sword.transform;
		Destroy(tmp, 0.6f);

		//�����蔻��̏o��
		m_sword.GetComponent<BoxCollider>().enabled = true;
	}
	public void OnAttackEnd1()
	{
		//�����蔻��̔�\��
		m_sword.GetComponent<BoxCollider>().enabled = false;
	}


	//��i��
	public void OnAttack2()
	{
		//���ʉ��̍Đ�
		SoundManager.Play2D(m_attackSE[(int)Attack.Second]);

		//�G�t�F�N�g�̐����ƍ폜
		GameObject tmp = Instantiate(m_skillEffect[(int)Attack.Second], transform.position, transform.rotation);
		tmp.transform.parent = transform;
		Destroy(tmp, 0.4f);

		//�����蔻��̏o��
		m_sword.GetComponent<BoxCollider>().enabled = true;
	}
	public void OnAttackEnd2()
	{
		//�����蔻��̔�\��
		m_sword.GetComponent<BoxCollider>().enabled = false;
	}


	//�O�i��
	public void OnAttack3()
	{
		//���ʉ��̍Đ�
		SoundManager.Play2D(m_attackSE[(int)Attack.Third]);

		//�G�t�F�N�g�̐����ƍ폜
		GameObject tmp = Instantiate(m_skillEffect[(int)Attack.Third], transform.position, transform.rotation);
		tmp.transform.parent = transform;
		Destroy(tmp, 0.4f);

		//�����蔻��̏o��
		m_sword.GetComponent<BoxCollider>().enabled = true;
	}
	public void OnAttackEnd3()
	{
		//�����蔻��̔�\��
		m_sword.GetComponent<BoxCollider>().enabled = false;
	}


	//�l�i��
	public void OnAttack4()
	{
		//���ʉ��̍Đ�
		SoundManager.Play2D(m_attackSE[(int)Attack.Fourth]);

		//�U���G�t�F�N�g�̐����ƍ폜
		GameObject tmp = Instantiate(m_skillEffect[(int)Attack.Fourth], transform.position, transform.rotation);
		Destroy(tmp, 0.7f);
	}
	public void OnAttackEnd4()
	{

	}
}
