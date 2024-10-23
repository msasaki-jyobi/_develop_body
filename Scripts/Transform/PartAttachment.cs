using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class PartAttachment : MonoBehaviour
{
    // �C���X�y�N�^�[�Ŏw�肷�镔�ʁi��FHeadCollider��HipsCollider�j
    public Transform PartToAttach; // �v���C���[�̓���̕��ʂ�ݒ�

    // �v���C���[�S�́i���[�g�I�u�W�F�N�g�j
    public Transform PlayerRoot; // �v���C���[�S�̂̃��[�g�I�u�W�F�N�g

    // �^�[�Q�b�g�I�u�W�F�N�g�i��F�G�̋z�����݌��j
    public Transform TargetObject;

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

    void OnDisable()
    {
        cancellationTokenSource?.Cancel();
    }

    private void Update()
    {
        if (IsDebugK && Input.GetKeyDown(KeyCode.K))
        {
            AttachPlayerToTarget().Forget();
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g���㏑�����ăA�^�b�`����
    /// </summary>
    public void AttachTarget()
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
}
