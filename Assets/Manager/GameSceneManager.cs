using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] Slider m_slider;               //���������̕\��
	[SerializeField] Volume[] m_volume;
    [SerializeField] GameObject m_text;				//�Q�[���N���A�e�L�X�g
    [SerializeField] AudioClip m_clip;				//�Q�[���N���A���̌��ʉ�
	[SerializeField] Image m_fadePanel;             // �t�F�[�h�p��UI�p�l��
	[SerializeField] float m_fadeDuration = 1.0f;   // �t�F�[�h�̊����ɂ����鎞��
	[SerializeField] int m_nextSceneAmount;         //�J�ڐ�̃V�[���ԍ�

	void Start()
    {
		m_text.SetActive(false);

		m_volume[0].gameObject.SetActive(true);
		m_volume[1].gameObject.SetActive(false);
		m_slider.value = 0;
	}

    void Update()
    {
        //��������
        if (m_slider.value >= m_slider.maxValue)
        {
            SoundManager.Play2D(m_clip);
			m_volume[0].gameObject.SetActive(false);
			m_volume[1].gameObject.SetActive(true);
			m_volume[1].weight += Time.deltaTime;
			m_text.SetActive(true);
			StartCoroutine(FadeOutAndLoadScene());
		}
	}

	public IEnumerator FadeOutAndLoadScene()
	{
		m_fadePanel.enabled = true;                 // �p�l����L����
		float elapsedTime = 0.0f;                   // �o�ߎ��Ԃ�������
		Color startColor = m_fadePanel.color;       // �t�F�[�h�p�l���̊J�n�F���擾
		Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1.0f); // �t�F�[�h�p�l���̍ŏI�F��ݒ�

		// �t�F�[�h�A�E�g�A�j���[�V���������s
		while (elapsedTime < m_fadeDuration)
		{
			elapsedTime += Time.deltaTime;                              // �o�ߎ��Ԃ𑝂₷
			float t = Mathf.Clamp01(elapsedTime / m_fadeDuration);      // �t�F�[�h�̐i�s�x���v�Z
			m_fadePanel.color = Color.Lerp(startColor, endColor, t);    // �p�l���̐F��ύX���ăt�F�[�h�A�E�g
			yield return null;                                          // 1�t���[���ҋ@
		}

		m_fadePanel.color = endColor;               // �t�F�[�h������������ŏI�F�ɐݒ�
		SceneManager.LoadScene(m_nextSceneAmount);  // �V�[�������[�h���ă��j���[�V�[���ɑJ��
	}

	public void AddDefeatAmount(int amount)
    {
		m_slider.maxValue += amount;
	}

    public void AddDefeatRate()
    {
		m_slider.value++;
    }
}