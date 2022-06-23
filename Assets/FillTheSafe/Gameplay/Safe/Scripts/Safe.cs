using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace FillTheSafe.Gameplay
{
    public class Safe : MonoBehaviour
    {
        [SerializeField]
        private Transform combinationLock;
        [SerializeField]
        private Transform doorParent;
        [SerializeField]
        private float lockDuration;
        [SerializeField] 
        private float doorDuration;
        [SerializeField]
        private Vector3 lockerRotation;
        [SerializeField]
        private Vector3 doorRotation;
        [SerializeField]
        private Transform containerPlace;
        [SerializeField]
        private Transform[] containers;
        [SerializeField]
        private float containerMovementDuration;

        private GridManager currentGridManager;
        private Transform currentContainer;
        private AudioSource lockAudio;
        private Vector3 startContainerPosition;
        private Vector3 startCombinationLockRotation;
        private Vector3 startDoorRotation;
        private int currentContainerIndex;
        private float partDuration;

        public GridManager CurrentGridManager => currentGridManager;

        private void Awake()
        {
            partDuration = containerMovementDuration / 2f;
            startCombinationLockRotation = combinationLock.localRotation.eulerAngles;
            startDoorRotation = doorParent.localRotation.eulerAngles;

            lockAudio = GetComponent<AudioSource>();
        }

        public async UniTask OpenSafe()
        {
            PlayLockAudioWithDelay();

            await combinationLock.DOLocalRotate(lockerRotation, lockDuration).AsyncWaitForCompletion();

            doorParent.DOLocalRotate(doorRotation, doorDuration);
        }

        public async void TakeContainer()
        {
            if (currentContainer != null)
            {
                await BackContainer(partDuration);

                if (containers.Length <= currentContainerIndex)
                {
                    currentContainer = null;
                    currentGridManager = null;

                    return;
                }
            }

            currentContainer = containers[currentContainerIndex];
            startContainerPosition = currentContainer.position;

            await currentContainer.DOMoveZ(containerPlace.position.z, partDuration).AsyncWaitForCompletion();

            currentContainer.DOLocalRotate(containerPlace.rotation.eulerAngles, partDuration);

            await currentContainer.DOMove(containerPlace.position, partDuration).AsyncWaitForCompletion();

            currentGridManager = currentContainer.GetComponentInChildren<GridManager>();
            currentContainerIndex++;
        }

        public async UniTask CloseSafe()
        {
            await BackContainer(partDuration);

            currentContainer = null;
            currentGridManager = null;

            await doorParent.DOLocalRotate(startDoorRotation, doorDuration).AsyncWaitForCompletion();

            combinationLock.DOLocalRotate(startCombinationLockRotation, lockDuration);
            PlayLockAudioWithDelay();
        }

        private async UniTask BackContainer(float duration)
        {
            currentContainer.DOLocalRotate(Vector3.zero, duration);

            await currentContainer.DOMoveY(startContainerPosition.y, duration).AsyncWaitForCompletion();
            await currentContainer.DOMove(startContainerPosition, duration).AsyncWaitForCompletion();
        }

        private async void PlayLockAudioWithDelay()
        {
            await UniTask.Delay(400);

            lockAudio.Play();
        }
    }
}