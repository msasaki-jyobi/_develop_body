using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.LowLevel;
using System.Collections.Generic;
using develop_common;
using UniRx;
using DG.Tweening;
using Unity.VisualScripting;

public class PartAttachment : MonoBehaviour
{
    public UnitInstance UnitInstance;

    public Transform PlayerRoot; // �v���C���[�S�̂̃��[�g�I�u�W�F�N�g
    public Transform Entity; // ���[�g�I�u�W�F�N�g
    public Sprite FaceImage;

    [Space(10)]
    // �C���X�y�N�^�[�Ŏw�肷�镔�ʁi��FHeadCollider��HipsCollider�j
    public Transform PartToAttach; // �v���C���[�̓���̕��ʂ�ݒ�

    // �v���C���[�S�́i���[�g�I�u�W�F�N�g�j
    //public UnitParameter UnitParameter; // ���[�g�I�u�W�F�N�g

    // �^�[�Q�b�g�I�u�W�F�N�g�i��F�G�̋z�����݌��j
    public Transform TargetObject;

    //[Header("�Œ艻�Q�Ɨp")]
    //public List<StringKeyGameObjectValuePair> BodyTargets = new List<StringKeyGameObjectValuePair>();


    // ���W�Ɖ�]�̃I�t�Z�b�g�i�C���X�y�N�^�[�Œ����\�j
    public Vector3 PositionOffset;
    public Vector3 RotationOffset;

    // �f�o�b�O�p�t���O
    public bool IsDebugK;

    // �A�ő΍�̂��߂̃t���O
    private bool isAttaching = false;
    private CancellationTokenSource cancellationTokenSource;

    // 2��ڂ̏����̂��߂̃f�B���C�ims�P�ʁj
    public int delayBetweenSteps = 10;

    // �Œ艻����
    public bool IsPull;
    // ��������p�i��������Ȃ��Ă����������j
    public bool IsDown;
    private Tween rotationTween; // ��]�A�j���[�V�������Ǘ����邽�߂�Tween�I�u�W�F�N�g

    void OnDisable()
    {
        cancellationTokenSource?.Cancel();
    }
    private void Start()
    {

    }

    private void Update()
    {
        if (IsDebugK && Input.GetKeyDown(KeyCode.K))
        {
            AttachPlayerToTarget().Forget();
        }

        // �Œ艻����K�v������̂ɁA����Ă���ꍇ
        if (IsPull)
        {
            var playerPos = PlayerRoot.transform.position;
            var targetPos = TargetObject.transform.position + PositionOffset;
            if (Vector3.Distance(playerPos, targetPos) >= 0.1f)
                AttachPlayerToTarget().Forget();
        }

        //if (transform.parent != Entity &&
        //    UnitParameter.KousokuValue.Value > 0 && UnitParameter.ParentTimer <= 0)
        //{
        //    // ����Ă���Ď��s
        //    if (TargetObject != null)
        //        if (PartToAttach != null)
        //            if (Vector3.Distance(TargetObject.transform.position, PartToAttach.transform.position) < (PartToAttach.transform.position + PositionOffset).magnitude)
        //                AttachPlayerToTarget().Forget();
        //}
        //else if (UnitParameter.ParentTimer > 0)
        //{
        //    UnitParameter.SetEntityParent();
        //}

        //if (transform.parent != null && transform.parent.gameObject == null)
        //{
        //    // �e�I�u�W�F�N�g���j�󂳂ꂽ�ꍇ�ɁA������؂藣��
        //    transform.SetParent(Entity);
        //    Debug.Log("Parent destroyed, detaching A object.");
        //}


    }

    /// <summary>
    /// �I�u�W�F�N�g���㏑�����ăA�^�b�`����
    /// </summary>
    public async void AttachTarget(Transform partToAttach, string AttachName, Vector3 positionOffset = default, Vector3 rotationOffset = default)
    {
        foreach (var target in UnitInstance.InstanceBodys)
        {
            if (target.Key == AttachName)
                PartToAttach = target.Value.transform;
        }
        TargetObject = partToAttach;
        PositionOffset = positionOffset;
        RotationOffset = rotationOffset;

        IsPull = true;

        AttachPlayerToTarget().Forget();
        await UniTask.Delay(10);
        AttachPlayerToTarget().Forget();
        await UniTask.Delay(10);
        AttachPlayerToTarget().Forget();

    }

    public void ForgetAttach()
    {
        AttachPlayerToTarget().Forget();
    }

    // �v���C���[���^�[�Q�b�g�ɂ������郁�\�b�h
    public async UniTask AttachPlayerToTarget()
    {
        if (isAttaching) return; // �A�Ŗh�~
        isAttaching = true;
        cancellationTokenSource = new CancellationTokenSource();

        // 1��̏�����2�񕪂̃A�^�b�`�������s��
        await AttachInTwoStepsWithDelay(cancellationTokenSource.Token);

        isAttaching = false;
    }

    // 2��̏������s���A�ԂɃf�B���C������
    private async UniTask AttachInTwoStepsWithDelay(CancellationToken token)
    {
        // Step 1: �ŏ��̈ʒu���킹
        AttachDirectly();

        // �����̃f�B���C������i�����ł�100ms�j
        await UniTask.Delay(delayBetweenSteps, cancellationToken: token);

        // Step 2: �ēx�ʒu�Ɖ�]���C��
        AttachDirectly();
    }

    private void AttachDirectly()
    {
        // �^�[�Q�b�g�I�u�W�F�N�g�̍��W�E��]���擾
        Vector3 targetPosition = TargetObject.position + TargetObject.TransformDirection(PositionOffset);
        Quaternion targetRotation = TargetObject.rotation * Quaternion.Euler(RotationOffset);

        // ��ƂȂ镔�ʁiPartToAttach�j�̌��݂̃��[���h���W�Ɖ�]���擾
        Vector3 partWorldPosition = PartToAttach.position;
        Quaternion partWorldRotation = PartToAttach.rotation;

        // PlayerRoot�̈ʒu���ړ�������ۂɁA���ʂ̈ʒu���^�[�Q�b�g�ɂ҂����荇���悤�Ɉړ�
        Vector3 positionDifference = targetPosition - partWorldPosition;
        PlayerRoot.position += positionDifference;

        // ���ʂ̉�]���^�[�Q�b�g�̉�]�ɂ҂����荇���悤�ɁAPlayerRoot�̉�]���X�V
        Quaternion rotationDifference = targetRotation * Quaternion.Inverse(partWorldRotation);
        PlayerRoot.rotation = rotationDifference * PlayerRoot.rotation;

        // �^�[�Q�b�g�I�u�W�F�N�g�̎q�I�u�W�F�N�g�Ƃ��ăZ�b�g
        PlayerRoot.SetParent(TargetObject);
    }

    //// �C���X�y�N�^�[�ł̒������ɂ����Ɍ��ʂ�������悤�ɂ��邽��
    //private void OnValidate()
    //{
    //    if (PlayerRoot != null && TargetObject != null && PartToAttach != null)
    //    {
    //        AttachDirectly(); // �C���X�y�N�^�[�ł̒������ɑ����ɔ��f
    //    }
    //}

    public GameObject GetBody(string keyName)
    {
        if(UnitInstance == null)
        {
            Debug.LogError($"{keyName}, UnitInstance��Null�ł�. ");
            return null;
        }
        foreach (var target in UnitInstance.InstanceBodys)
        {
            if (target.Key == keyName)
                return target.Value;
        }
        return null;
    }
    /// <summary>
    /// �e�I�u�W�F�N�g����������
    /// PullOFF�̍U�����󂯂��^�C�~���O�ŌĂ΂�A�I�u�W�F�N�g�����������B
    /// �N���オ����ǂ����邩�H
    /// </summary>
    public async void SetEntityParent()
    {
        if (PlayerRoot == null) return;
        if (Entity == null) return;

        IsPull = false;
        PlayerRoot.transform.parent = Entity.transform;

        // ���łɉ�]���̃A�j���[�V����������ꍇ�̓L�����Z��
        rotationTween?.Kill();

        // �ڕW�̉�]�l��ݒ� (0, ���݂�Y��, 0)
        Vector3 targetRotation = new Vector3(0, PlayerRoot.transform.rotation.eulerAngles.y, 0);

        // DoTween�ŉ�]�A�j���[�V�������쐬
        rotationTween = PlayerRoot.transform.DORotate(targetRotation, 0.3f)
            .SetEase(Ease.Linear)    // ���`���
            .SetLink(PlayerRoot.gameObject);    // �Q�[���I�u�W�F�N�g�Ƀ����N�i�j�����Ɏ����L�����Z���j

        // �A�j���[�V��������������̂�ҋ@
        await rotationTween.AsyncWaitForCompletion();
    }

    // �G�̋Z�𔭓����郁�\�b�h�i�������g�p�j
    public void ActivateAbility(Transform playerHead, Transform enemyRoot, Transform hammer, Vector3 offset)
    {
        // B �� A�ɂ�������
        // ���@���@�|���@�@�ɂ�������
        var A = TargetObject;
        var B = PartToAttach;
        var C = PlayerRoot;

        A = playerHead;
        B = hammer;
        C = enemyRoot;

        Vector3 rot = C.transform.rotation.eulerAngles;

        // �^�[�Q�b�g�I�u�W�F�N�g�̍��W�E��]���擾
        Vector3 targetPosition = A.position + A.TransformDirection(PositionOffset);
        Quaternion targetRotation = A.rotation * Quaternion.Euler(RotationOffset);

        // ��ƂȂ镔�ʁiPartToAttach�j�̌��݂̃��[���h���W�Ɖ�]���擾
        Vector3 partWorldPosition = B.position;
        Quaternion partWorldRotation = B.rotation;

        // PlayerRoot�̈ʒu���ړ�������ۂɁA���ʂ̈ʒu���^�[�Q�b�g�ɂ҂����荇���悤�Ɉړ�
        Vector3 positionDifference = targetPosition - partWorldPosition;
        C.position += positionDifference;
        C.position += UtilityFunction.LocalLookPos(PlayerRoot.transform, offset);

        // ���ʂ̉�]���^�[�Q�b�g�̉�]�ɂ҂����荇���悤�ɁAPlayerRoot�̉�]���X�V
        Quaternion rotationDifference = targetRotation * Quaternion.Inverse(partWorldRotation);
        C.rotation = rotationDifference * C.rotation;
        C.rotation = Quaternion.Euler(0, C.transform.rotation.eulerAngles.y, 0);

        // �^�[�Q�b�g�I�u�W�F�N�g�̎q�I�u�W�F�N�g�Ƃ��ăZ�b�g
        //C.SetParent(A);

    }



















}
