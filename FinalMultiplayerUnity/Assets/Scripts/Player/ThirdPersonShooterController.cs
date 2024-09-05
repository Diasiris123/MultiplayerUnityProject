using System;
using Unity.Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviourPunCallbacks
{
    [SerializeField] public GameObject cameraFollowTarget;
    [SerializeField] public CinemachineCamera aimVirtualCamera;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();

    [Space(10)]
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;

    private Camera _mainCam;
    [SerializeField] private Transform aimDebugTransform;
    private Vector2 _screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
    private Vector3 _mouseToWorldPos = Vector3.zero;
    
    
    private void Start()
    {
        _mainCam = Camera.main;
        
        
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;
        
       // SwitchToAimCamera();
        AimAtScreenCenter();
    }

    private void SwitchToAimCamera()
    {
        if (!photonView.IsMine)
            return;
        
        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetMouseSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);

            Vector3 aimTarget = _mouseToWorldPos;
            aimTarget.y = transform.position.y;
            Vector3 aimDir = (aimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 20f);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetRotateOnMove(true);
            thirdPersonController.SetMouseSensitivity(normalSensitivity);
        }
    }

    private void AimAtScreenCenter()
    {
        if (!photonView.IsMine)
            return;
        
        Ray ray = _mainCam.ScreenPointToRay(_screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, 999f, aimColliderLayerMask))
        {
            //aimDebugTransform.position = hit.point;
            _mouseToWorldPos = hit.point;
        }
    }
    
    
    
}
    