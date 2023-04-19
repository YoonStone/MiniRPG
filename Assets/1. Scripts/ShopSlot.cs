using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopSlot : MonoBehaviour
{
    public Item item;

    public Image slotImage;
    public TextMeshProUGUI itemTxt;
    public TextMeshProUGUI goldTxt;
    public Button buybtn;

    PlayUIManager manager;

    private void Start()
    {
        slotImage.sprite = item.itemSprite;
        itemTxt.text = item.itemName_kr;
        goldTxt.text = item.itemPrice.ToString() + " G";

        manager = PlayUIManager.instance;

        buybtn.onClick.AddListener(OnClick);
    }

    // 슬롯 클릭하면 아이템 구매
    public void OnClick()
    {
        // 골드가 부족하면 구매 불가
        if (manager.Gold < item.itemPrice) return;
        StartCoroutine(BuyItem());
    }

    // 팝업창 사용 (아이템 구매)
    IEnumerator BuyItem()
    {
        // 팔 수 없는 아이템이라면 실행 불가
        if (!item.isCanSell) yield break;

        bool isCanPopup = PlayUIManager.instance.PopupOpen("이 아이템을 구매하시겠습니까?", "예", "아니오");
       
        // 이미 팝업창이 열려있었다면 실행 금지
        if (!isCanPopup) yield break;

        // 예/아니오를 누를 때까지 기다리기
        PlayUIManager.instance.popupState = PopupState.None;
        yield return new WaitUntil(() => PlayUIManager.instance.popupState != PopupState.None);

        // 예를 눌렀다면 아이템 구매
        if (PlayUIManager.instance.popupState == PopupState.Left)
        {
            InventoryManager.instance.AddItem(item);
            manager.Gold -= item.itemPrice;
        }

        PlayUIManager.instance.popupState = PopupState.None;
    }
}
