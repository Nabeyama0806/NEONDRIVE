using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] Slider m_slider;               //討伐割合の表示
	[SerializeField] Volume[] m_volume;
    [SerializeField] GameObject m_text;				//ゲームクリアテキスト
    [SerializeField] AudioClip m_clip;				//ゲームクリア時の効果音
	[SerializeField] Image m_fadePanel;             // フェード用のUIパネル
	[SerializeField] float m_fadeDuration = 1.0f;   // フェードの完了にかかる時間
	[SerializeField] int m_nextSceneAmount;         //遷移先のシーン番号

	void Start()
    {
		m_text.SetActive(false);

		m_volume[0].gameObject.SetActive(true);
		m_volume[1].gameObject.SetActive(false);
		m_slider.value = 0;
	}

    void Update()
    {
        //討伐完了
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

	public void AddDefeatAmount(int amount)
    {
		m_slider.maxValue += amount;
	}

    public void AddDefeatRate()
    {
		m_slider.value++;
    }
}