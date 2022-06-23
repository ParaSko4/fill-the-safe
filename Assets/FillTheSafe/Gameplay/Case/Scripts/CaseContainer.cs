using UnityEngine;
using Zenject;

namespace FillTheSafe.Gameplay
{
    public class CaseContainer : MonoBehaviour
    {
        [SerializeField]
        private float containerSpeed;
        [SerializeField]
        private Transform leftBorder;
        [SerializeField]
        private Transform rightBorder;
        [SerializeField]
        private Case[] cases;

        private Case currentSelectedCase;
        private Camera mainCamera;

        public Case CurrentSelectedCase => currentSelectedCase;

        [Inject]
        public void Construct(Camera mainCamera)
        {
            this.mainCamera = mainCamera;
        }

        private void Update()
        {
            SideMovement();
            DetectCaseTouch();
        }

        private void SideMovement()
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                var newPosition = Time.deltaTime * containerSpeed * Vector3.left + transform.position;

                if (leftBorder.position.x < newPosition.x)
                {
                    transform.position = newPosition;
                }
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                var newPosition = Time.deltaTime * containerSpeed * Vector3.right + transform.position;

                if (rightBorder.position.x > newPosition.x)
                {
                    transform.position = newPosition;
                }
            }
        }

        private void DetectCaseTouch()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Case selectedCase = null;

                    foreach (var c in cases)
                    {
                        if (hit.collider.gameObject == c.gameObject)
                        {
                            selectedCase = c;
                            break;
                        }
                    }

                    if (selectedCase == null)
                    {
                        return;
                    }

                    if (currentSelectedCase != null)
                    {
                        currentSelectedCase.BackCase();
                    }

                    selectedCase.TakeCase();
                    currentSelectedCase = selectedCase;
                }
            }
        }
    }
}