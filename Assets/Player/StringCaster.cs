using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody),(typeof(LineRenderer)))]
public class StringCaster : MonoBehaviour
{
	[SerializeField] Animator animator;
	[SerializeField] float maximumDistance = 50.0f;     //����L�΂���ő勗��
	[SerializeField] LayerMask interactiveLayers;       //�������������郌�C���[
	[SerializeField] Vector3 casterCenter;				//�I�u�W�F�N�g�̃��[�J�����W�ŕ\�������̎ˏo�ʒu
	[SerializeField] float spring = 50.0f;              //SpringJoint��spring
	[SerializeField] float damper = 20.0f;              //SpringJoint��damper
	[SerializeField] float equilibriumLength = 1.0f;    //�����k�߂����̎��R��
	[SerializeField] float m_coolTime = 1.0f;			//�ˏo���邽�߂̕K�v����
	[SerializeField] GameObject playerHead;             //�v���C���[�̓��̈ʒu
	[SerializeField] GameObject m_shotEffect;			//�L�k���̃G�t�F�N�g
	[SerializeField] GameObject m_rollEffect;			//�L�k���̃G�t�F�N�g
	[SerializeField] Image[] reticleImage;				//�Ə��}�[�N
	[SerializeField] AudioClip[] m_line;                //���C���[�L�k�̌��ʉ�
	[SerializeField] Image m_icon;
	
	private PlayerInput m_playerInput;
	private Rigidbody rb;
	private Transform cameraTransform;
	private LineRenderer lineRenderer;
	private SpringJoint springJoint;

	private Vector3 cameraForward;
	private Ray cameraRay;
	private Ray aimingRay;

	private bool isCasting;             //�����ˏo�����ǂ���
	private bool needsUpdateSpring;     //SpringJoint�̍X�V���K�v���ǂ���
	private float stringLength;         //���݂̎��̒���
	private float m_elapsedTime;		//�N�[���^�C���̌o�ߎ���
	private Vector3 worldCasterCenter;  //casterCenter�����[���h���W�ɕϊ���������
	private readonly Vector3[] stringAnchor = new Vector3[2]; //SpringJoint�̃L�����N�^�[���Ɛڒ��_���̖��[

	enum Effects
	{
		Shot,
		Roll,
	}

	private void Awake()
	{
		//�R���|�[�l���g�̎擾
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
		isCasting = true;                       //�����ˏo���v�t���O�𗧂Ă�
		needsUpdateSpring = true;               //SpringJoint�v�X�V�v�t���O�𗧂Ă�
		stringAnchor[1] = aimingTarget.point;   //���̐ڒ��_���[��ݒ�
		stringLength = Vector3.Distance(worldCasterCenter, aimingTarget.point); //���̒�����ݒ�
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
		//�����k�߂�
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
		//���̎ˏo�������Z�o
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

		// ���̏�Ԃ��X�V����
		UpdateString();

		//�N�[���^�C�����A�C�R���ɔ��f
		if (m_elapsedTime < m_coolTime)
		{
			m_elapsedTime += Time.deltaTime;
		}
		m_icon.fillAmount = 1 - m_elapsedTime / m_coolTime;
	}

	private void UpdateString()
	{
		//����`��
		if (lineRenderer.enabled = isCasting)
		{
			//���̃L�����N�^�[�����[��ݒ�
			stringAnchor[0] = worldCasterCenter;

			//���̕`��ݒ���s��
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
			//SpringJoint�������Ă��Ȃ���Β���
			if (springJoint == null)
			{
				springJoint = gameObject.AddComponent<SpringJoint>();
				springJoint.autoConfigureConnectedAnchor = false;
				springJoint.anchor = casterCenter;
				springJoint.spring = spring;
				springJoint.damper = damper;
			}

			// SpringJoint�̎��R���Ɛڑ����ݒ肷��
			springJoint.maxDistance = stringLength;
			springJoint.connectedAnchor = stringAnchor[1];
		}
		else
		{
			//���ɂ������ς���N����Ȃ�����
			Destroy(springJoint);
			springJoint = null;
		}

		// �X�V�̏I��
		needsUpdateSpring = false;
	}

	public bool IsCasting()
	{
		return isCasting;
	}
}