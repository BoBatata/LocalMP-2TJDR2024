using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using StarterAssets;
using UnityEngine;

public class PlayerShootController : MonoBehaviour
{
    private StarterAssetsInputs assetsInputs;
    private ThirdPersonController thirdPersonController;

    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private float rotationVelocity;
    [SerializeField] private float aimAnimVelocity;

    [SerializeField] private CinemachineVirtualCamera aimCamera;
    [SerializeField] private LayerMask aimColliderMask = new LayerMask();
    [SerializeField] private ParticleSystem shootParticle;
    private Animator animator;
    private Camera myCamera;
    private Vector3 mouseWorldPosition = Vector3.zero;
    private Vector3 worldAimTarget = Vector3.zero;
    //private RaycastHit shootRay = new RaycastHit();
    private Transform hittedObject;

    private void Awake()
    {
        assetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
        myCamera = transform.parent.gameObject.GetComponentInChildren<Camera>();
        aimCamera.gameObject.SetActive(false);
    }

    private void Update()
    {
        //if (assetsInputs.aim)
        //{
        //    aimCamera.gameObject.SetActive(true);
        //    thirdPersonController.SetSenstivity(aimSensitivity);
        //}
        //else
        //{
        //    aimCamera.gameObject.SetActive(false);
        //    thirdPersonController.SetSenstivity(normalSensitivity);
        //}
        CheckAimInput();
        HandleAimDirection();
        ShootInputHandler();
    }

    private void HandleAimDirection()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = myCamera.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit rayhit, 999f, aimColliderMask))
        {
            mouseWorldPosition = rayhit.point;
            hittedObject = rayhit.transform;
        }
    }

    private void CheckAimInput()
    {
        if (assetsInputs.aim)
        {
            thirdPersonController.SetLockRotation(true);
            aimCamera.gameObject.SetActive(true);
            thirdPersonController.SetSenstivity(aimSensitivity);

            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1, Time.deltaTime * aimAnimVelocity));

            Quaternion currentRotation = transform.rotation;
            worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            Quaternion directionToAim = Quaternion.LookRotation(aimDirection);

            transform.rotation = Quaternion.Slerp(currentRotation, directionToAim, rotationVelocity * Time.deltaTime);
        }
        else
        {
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * aimAnimVelocity));
            thirdPersonController.SetLockRotation(false);
            aimCamera.gameObject.SetActive(false);
            thirdPersonController.SetSenstivity(normalSensitivity);
        }
    }

    private void ShootInputHandler()
    {
        if (assetsInputs.shoot)
        {
            Instantiate(shootParticle, mouseWorldPosition, hittedObject.rotation);
            assetsInputs.shoot = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, mouseWorldPosition);
    }
}
