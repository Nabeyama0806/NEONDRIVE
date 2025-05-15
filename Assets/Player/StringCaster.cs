using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody),(typeof(LineRenderer)))]
public class StringCaster : MonoBehaviour
{
	[SerializeField] Animator animator;
	[SerializeField] float maximumDistance = 50.0f;     //糸を伸ばせる最大距離
	[SerializeField] LayerMask interactiveLayers;       //糸をくっつけられるレイヤー
	[SerializeField] Vector3 casterCenter;				//オブジェクトのローカル座標で表した糸の射出位置
	[SerializeField] float spring = 50.0f;              //SpringJointのspring
	[SerializeField] float damper = 20.0f;              //SpringJointのdamper
	[SerializeField] float equilibriumLength = 1.0f;    //糸を縮めた時の自然長
	[SerializeField] float m_coolTime = 1.0f;			//射出するための必要時間
	[SerializeField] GameObject playerHead;             //プレイヤーの頭の位置
	[SerializeField] GameObject m_shotEffect;			//伸縮時のエフェクト
	[SerializeField] GameObject m_rollEffect;			//伸縮時のエフェクト
	[SerializeField] Image[] reticleImage;				//照準マーク
	[SerializeField] AudioClip[] m_line;                //ワイヤー伸縮の効果音
	[SerializeField] Image m_icon;
	
	private PlayerInput m_playerInput;
	private Rigidbody rb;
	private Transform cameraTransform;
	private LineRenderer lineRenderer;
	private SpringJoint springJoint;

	private Vector3 cameraForward;
	private Ray cameraRay;
	private Ray aimingRay;

	private bool isCasting;             //糸が射出中かどうか
	private bool needsUpdateSpring;     //SpringJointの更新が必要かどうか
	private float stringLength;         //現在の糸の長さ
	private float m_elapsedTime;		//クールタイムの経過時間
	private Vector3 worldCasterCenter;  //casterCenterをワールド座標に変換したもの
	private readonly Vector3[] stringAnchor = new Vector3[2]; //SpringJointのキャラクター側と接着点側の末端

	enum Effects
	{
		Shot,
		Roll,
	}

	private void Awake()
	{
		//コンポーネントの取得
		rb = GetComponent<Rigidbody>();
		lineRenderer = GetComponent<LineRenderer>();
		m_playerInput = GetComponent<PlayerInput>();
		cameraTransform = Camera.main.transform;
		worldCasterCenter = transform.TransformPoint(casterCenter);
		m_shotEffect.SetActive(false);
	}

	private void OnEnable()
	{
		m_playerInput.actions["Shot"].performed += OnShot;
		m_playerInput.actions["Shot"].canceled += OnShotCancel;

		m_playerInput.actions["Roll"].performed += OnRoll;
	}

	private void OnDisable()
	{
		m_playerInput.actions["Shot"].performed -= OnShot;
		m_playerInput.actions["Shot"].canceled -= OnShotCancel;

		m_playerInput.actions["Roll"].performed -= OnRoll;
	}

	private void OnShot(InputAction.CallbackContext context)
	{
		if (m_elapsedTime < m_coolTime) return;
		if (!Physics.Raycast(aimingRay, out var aimingTarget, maximumDistance, interactiveLayers)) return;

		m_shotEffect.SetActive(true);
		isCasting = true;                       //糸を射出中」フラグを立てる
		needsUpdateSpring = true;               //SpringJoint要更新」フラグを立てる
		stringAnchor[1] = aimingTarget.point;   //糸の接着点末端を設定
		stringLength = Vector3.Distance(worldCasterCenter, aimingTarget.point); //糸の長さを設定
		animator.SetBool("Run", false);
		animator.SetBool("Shot", true);
		SoundManager.Play2D(m_line[0]);
	}
	private void OnShotCancel(InputAction.CallbackContext context)
	{
		isCasting = false;
		needsUpdateSpring = true;
		rb.useGravity = true;
		m_shotEffect.SetActive(false);
		animator.SetBool("Shot", false);
	}

	private void OnRoll(InputAction.CallbackContext context)
	{
		//糸を縮める
		if (isCasting)
		{
			m_elapsedTime = 0;
			needsUpdateSpring = true;
			stringLength = equilibriumLength;
			SoundManager.Play2D(m_line[1]);

			GameObject tmp = Instantiate(m_rollEffect, stringAnchor[1], Quaternion.identity);
			Destroy(tmp, 1.3f);
		}
	}

	private void Update()
	{
		//糸の射出方向を算出
		worldCasterCenter = transform.TransformPoint(casterCenter);
		cameraForward = cameraTransform.forward;
		cameraRay = new Ray(cameraTransform.position, cameraForward);
		aimingRay = new Ray(
			worldCasterCenter,
			Physics.Raycast(cameraRay, out var focus, float.PositiveInfinity, interactiveLayers)
				? focus.point - worldCasterCenter
				: cameraForward);

		if (Physics.Raycast(aimingRay, out var aimingTarget, maximumDistance, interactiveLayers))
		{
			reticleImage[0].gameObject.SetActive(true);
			reticleImage[1].gameObject.SetActive(false);
		}
		else
		{
			reticleImage[0].gameObject.SetActive(false);
			reticleImage[1].gameObject.SetActive(true);
		}

		// 糸の状態を更新する
		UpdateString();

		//クールタイムをアイコンに反映
		if (m_elapsedTime < m_coolTime)
		{
			m_elapsedTime += Time.deltaTime;
		}
		m_icon.fillAmount = 1 - m_elapsedTime / m_coolTime;
	}

	private void UpdateString()
	{
		//糸を描画
		if (lineRenderer.enabled = isCasting)
		{
			//糸のキャラクター側末端を設定
			stringAnchor[0] = worldCasterCenter;

			//糸の描画設定を行う
			lineRenderer.SetPositions(stringAnchor);
			var gbValue = Mathf.Exp(
				springJoint != null
					? -Mathf.Max(Vector3.Distance(stringAnchor[0], stringAnchor[1]) - stringLength, 0.0f)
					: 0.0f);
		}
	}

	private void FixedUpdate()
	{
		if (!needsUpdateSpring) return;

		if (isCasting)
		{
			//SpringJointが張られていなければ張る
			if (springJoint == null)
			{
				springJoint = gameObject.AddComponent<SpringJoint>();
				springJoint.autoConfigureConnectedAnchor = false;
				springJoint.anchor = casterCenter;
				springJoint.spring = spring;
				springJoint.damper = damper;
			}

			// SpringJointの自然長と接続先を設定する
			springJoint.maxDistance = stringLength;
			springJoint.connectedAnchor = stringAnchor[1];
		}
		else
		{
			//糸による引っぱりを起こらなくする
			Destroy(springJoint);
			springJoint = null;
		}

		// 更新の終了
		needsUpdateSpring = false;
	}

	public bool IsCasting()
	{
		return isCasting;
	}
}