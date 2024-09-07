using System;
using CharacterSelection;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableCharacter : MonoBehaviourPunCallbacks , IPointerClickHandler
{
    private const string UPDTAE_STATUS_RPC = nameof(UpdateStatus);
    
    [SerializeField] Image image;
    [SerializeField] Outline outline;
    [SerializeField] private int id;
    [SerializeField] Material[] materials;
    [SerializeField] PlayerColor playerColor;

    public Color color;

    private bool isTaken;

    private void Start()
    {
        color = materials[(int)playerColor].color;
        image.color = color;
        outline.enabled = false;
    }

    public void Take()
    {
        if(CharacterSelectionManager.Instance.isReady)
            return;
        
        if (!isTaken)
        {
            isTaken = true;
            outline.enabled = true;
            outline.effectColor = Color.green;
            
            photonView.RPC(UPDTAE_STATUS_RPC, RpcTarget.Others,id,isTaken);

            if (CharacterSelectionManager.Instance.selectedCharacter != null)
            {
                CharacterSelectionManager.Instance.selectedCharacter.Release();
            }

            CharacterSelectionManager.Instance.selectedCharacter = this;
        }
        else
        {
            Debug.Log("another player has selected this character already");
        }
    }

    [PunRPC]
    private void UpdateStatus(int charId, bool taking)
    {
        if (id == charId)
        {
            isTaken = taking;
            
            if (isTaken)
            {
                outline.enabled = true;
                outline.effectColor = Color.red;
            }
            else
            {
                outline.enabled = false;
            }
        }
        
    }

    public void Release()
    {
        isTaken = false;
        outline.enabled = false;
        
        photonView.RPC(UPDTAE_STATUS_RPC, RpcTarget.Others,id,isTaken);
    }

    public enum PlayerColor
    {
        White,
        Red,
        Yellow,
        Blue,
        Purple,
        Green,
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Take();
    }
}
