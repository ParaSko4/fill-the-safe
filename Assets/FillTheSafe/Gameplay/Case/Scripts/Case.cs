using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace FillTheSafe.Gameplay
{
    public class Case : MonoBehaviour
    {
        [SerializeField]
        private Transform cap;
        [SerializeField]
        private float capOpeningDuration;
        [SerializeField]
        private float capRotation;
        [SerializeField]
        private Transform container;
        [SerializeField]
        private float caseValueSpawDuration;
        [SerializeField]
        private float selectedYCoordinate;

        private Queue<Transform> caseValue;
        private Camera mainCamera;
        private Tween shakeTween;
        private Tween upCoordinateTween;
        private float startYCoordinate;
        private float startXRotation;

        [Inject]
        public void Construct(Camera mainCamera)
        {
            this.mainCamera = mainCamera;
        }

        private void Awake()
        {
            startYCoordinate = transform.position.y;
            startXRotation = transform.localEulerAngles.x;

            var valueList = new List<Transform>();

            foreach (Transform child in container)
            {
                valueList.Add(child);
            }

            caseValue = new Queue<Transform>(valueList
                .OrderBy(t => Vector3.Distance(mainCamera.transform.position, t.position)));
        }

        public void OpenCase()
        {
            var newRotation = cap.localRotation.eulerAngles.ChangeX(capRotation);
            cap.DOLocalRotate(newRotation, capOpeningDuration).SetEase(Ease.OutBounce);
        }

        public void CloseCase()
        {
            var newRotation = cap.localRotation.eulerAngles.ChangeX(startXRotation);
            cap.DOLocalRotate(newRotation, capOpeningDuration).SetEase(Ease.Linear);
        }

        public void TakeCase()
        {
            upCoordinateTween?.Kill();
            upCoordinateTween = transform.DOMoveY(selectedYCoordinate, 1f);
        }

        public void BackCase()
        {
            upCoordinateTween?.Kill();
            upCoordinateTween = transform.DOMoveY(startYCoordinate, 1f);
        }

        public Transform TakeCaseValue()
        {
            if (caseValue.Count == 0)
            {
                if (shakeTween.IsActive() == false)
                {
                    shakeTween = transform.DOShakeRotation(1f, strength: 15);
                }

                return null;
            }

            return caseValue.Dequeue();
        }
    }
}