using UnityEngine;
using System.Collections;
using Soomla.Store;
public class FreeGiftController : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		gameObject.GetComponent<UnityEngine.UI.Button>().interactable = StoreInventory.GetItemBalance (ColorJumpStoreAssets.FREEGIFT_TOKEN_ITEM_ID) > 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void onFreeGiftButtonClicked()
	{
		
	}
}

