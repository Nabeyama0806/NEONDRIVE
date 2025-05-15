using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
	[SerializeField] Image m_fadePanel;             // フェード用のUIパネル
	[SerializeField] float m_fadeDuration = 1.0f;   // フェードの完了にかかる時間
	[SerializeField] int m_nextSceneAmount;         //遷移先のシーン番号
	[SerializeField] AudioClip m_fadeClip;

	private const float WaitTime = 0.6f;

	private PlayerInput m_playerInput;      //入力
	private float m_elapsedTime;            //経過時間
	private bool m_canPlaying;              //シーン遷移の必要時間

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
		m_fadePanel.enabled = true;                 // パネルを有効化
		float elapsedTime = 0.0f;                   // 経過時間を初期化
		Color startColor = m_fadePanel.color;       // フェードパネルの開始色を取得
		Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1.0f); // フェードパネルの最終色を設定

		// フェードアウトアニメーションを実行
		while (elapsedTime < m_fadeDuration)
		{
			elapsedTime += Time.deltaTime;                              // 経過時間を増やす
			float t = Mathf.Clamp01(elapsedTime / m_fadeDuration);      // フェードの進行度を計算
			m_fadePanel.color = Color.Lerp(startColor, endColor, t);    // パネルの色を変更してフェードアウト
			yield return null;                                          // 1フレーム待機
		}

		m_fadePanel.color = endColor;               // フェードが完了したら最終色に設定
		SceneManager.LoadScene(m_nextSceneAmount);  // シーンをロードしてメニューシーンに遷移
	}
}