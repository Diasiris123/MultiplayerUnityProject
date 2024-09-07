using System;
using System.Collections;
using Unity.Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviourPunCallbacks
{
    private static readonly int SHOOT_ANIM_ID = Animator.StringToHash("Shoot");
    private const string PROJECTILE_PREFAB_PATH = "Player/Projectile";
    private const int ANIMATOR_SHOOTING_LAYER = 1;
    private const int ANIMATOR_BASE_LAYER = 0;
    
    [SerializeField] public GameObject cameraFollowTarget;
    [SerializeField] public CinemachineCamera aimCamera;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Animator animator;

    [Space(10)]
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private Transform projectileSpawnPoint;

    private Camera _mainCam;
    [SerializeField] private Transform aimDebugTransform;
    private Vector2 _screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
    private Vector3 _mouseToWorldPos = Vector3.zero;
    
    
    
    private void Start()
    {
        _mainCam = Camera.main;
        
        if(photonView.AmOwner)
            playerInput.enabled = true;
    }

    private void Update()
    {
        if (!photonView.AmOwner)
            return;
        
        SwitchToAimCamera();
        //RotateToCameraForwardVector();
        AimAtScreenCenter();
        HandleShootInput();
    }

    private void SwitchToAimCamera()
    {
        
        if (starterAssetsInputs.aim)
        {
            aimCamera.gameObject.SetActive(true);
            thirdPersonController.SetMouseSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            animator.SetLayerWeight(ANIMATOR_SHOOTING_LAYER, Mathf.Lerp(animator.GetLayerWeight(ANIMATOR_SHOOTING_LAYER),1f, Time.deltaTime * 20f));

            Vector3 aimTarget = _mouseToWorldPos;
            aimTarget.y = transform.position.y;
            Vector3 aimDir = (aimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 20f);
        }
        else
        {
           
            aimCamera.gameObject.SetActive(false);
            thirdPersonController.SetRotateOnMove(true);
            thirdPersonController.SetMouseSensitivity(normalSensitivity);
            animator.SetLayerWeight(ANIMATOR_SHOOTING_LAYER, Mathf.Lerp(animator.GetLayerWeight(ANIMATOR_SHOOTING_LAYER),0f, Time.deltaTime * 20f));
        }
    }


  
    
    private void RotateToCameraForwardVector()
    {
        Vector3 aimTarget = _mouseToWorldPos;
        aimTarget.y = transform.position.y; 
        Vector3 aimDir = (aimTarget - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 20f);
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
        else
        {
            _mouseToWorldPos = ray.GetPoint(100f);
        }
    }

    private void HandleShootInput()
    {
        if (starterAssetsInputs.shoot && starterAssetsInputs.aim)
        {
            
                Vector3 aimDir = (_mouseToWorldPos - projectileSpawnPoint.position).normalized;
                PhotonNetwork.Instantiate(PROJECTILE_PREFAB_PATH, projectileSpawnPoint.position,
                    Quaternion.LookRotation(aimDir, Vector3.up), 0);

                starterAssetsInputs.shoot = false;
        }
        
        
        
    }

    
    
    
    
    
}
    