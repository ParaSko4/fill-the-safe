using DG.Tweening;
using UnityEngine;
using Zenject;

namespace FillTheSafe.Gameplay
{
    public class ObjectStacker : MonoBehaviour
    {
        [SerializeField]
        private float delay;
        [SerializeField]
        private AudioSource popClipPrefab;

        private Safe safe;
        private Camera mainCamera;
        private CaseContainer caseContainer;

        private float timeLeft;

        [Inject]
        public void Construct(Camera mainCamera, Safe safe, CaseContainer caseContainer)
        {
            this.mainCamera = mainCamera;
            this.safe = safe;
            this.caseContainer = caseContainer;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                timeLeft += Time.deltaTime;

                if (timeLeft < delay)
                {
                    return;
                }

                timeLeft = 0f;
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (safe.CurrentGridManager == null || caseContainer.CurrentSelectedCase == null)
                    {
                        return;
                    }

                    if (safe.CurrentGridManager.GetClosestPositionToObject(hit.point, out Vector3 closestPosition))
                    {
                        var caseValue = caseContainer.CurrentSelectedCase.TakeCaseValue();

                        if (caseValue == null)
                        {
                            return;
                        }

                        caseValue.parent = safe.CurrentGridManager.transform;
                        caseValue.localRotation = Quaternion.identity;
                        caseValue.localPosition = closestPosition;

                        Instantiate(popClipPrefab);

                        var startLocalScale = caseValue.localScale;
                        caseValue.localScale = closestPosition * 0.8f;
                        caseValue.DOScale(startLocalScale, 0.2f);
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                timeLeft = 0f;
            }
        }
    }
}