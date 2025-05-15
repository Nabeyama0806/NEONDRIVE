using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
	[SerializeField] Image m_fadePanel;             // �t�F�[�h�p��UI�p�l��
	[SerializeField] float m_fadeDuration = 1.0f;   // �t�F�[�h�̊����ɂ����鎞��
	[SerializeField] int m_nextSceneAmount;         //�J�ڐ�̃V�[���ԍ�
	[SerializeField] AudioClip m_fadeClip;

	private const float WaitTime = 0.6f;

	private PlayerInput m_playerInput;      //����
	private float m_elapsedTime;            //�o�ߎ���
	private bool m_canPlaying;              //�V�[���J�ڂ̕K�v����

	private void OnEnable()
	{
		m_playerInput.actions["NextScene"].performed += OnNextScene;
	}

	private void OnDisable()
	{
		m_playerInput.actions["NextScene"].performed -= OnNextScene;
	}

	void Awake()
	{
		m_elapsedTime = 0;
		m_canPlaying = false;

		m_playerInput = GetComponent<PlayerInput>();
	}

	void Update()
	{
		m_elapsedTime += Time.deltaTime;
		if (m_elapsedTime > WaitTime) m_canPlaying = true;

	}

	private void OnNextScene(InputAction.CallbackContext context)
	{
		if (!m_canPlaying) return;
		SoundManager.Play2D(m_fadeClip);

		StartCoroutine(FadeOutAndLoadScene());
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
}