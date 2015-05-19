/// Copyright (C) 2012-2014 Soomla Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///      http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Soomla.Store;
 
	
	/// <summary>
	/// This class defines our game's economy, which includes virtual goods, virtual currencies
	/// and currency packs, virtual categories
	/// </summary>
	public class ColorJumpStoreAssets : IStoreAssets{
		
		/// <summary>
		/// see parent.
		/// </summary>
		public int GetVersion() {
			return 0;
		}
		
		/// <summary>
		/// see parent.
		/// </summary>
		public VirtualCurrency[] GetCurrencies() {
		return new VirtualCurrency[]{FREEGIFT_TOKEN};
		}
		
		/// <summary>
		/// see parent.
		/// </summary>
		public VirtualGood[] GetGoods() {
			return new VirtualGood[] {/*MUFFINCAKE_GOOD, PAVLOVA_GOOD,CHOCLATECAKE_GOOD, CREAMCUP_GOOD, */
			FREEGIFT_COUNTER,
			ACCUMULATED_ACTIVETIME,
			NO_ADS_LTVG,
			SKIN_01_LTVG,
			SKIN_02_LTVG,
			SKIN_03_LTVG,
			SKIN_04_LTVG,
			SKIN_05_LTVG,
			SKIN_06_LTVG,
			SKIN_07_LTVG,
			SKIN_08_LTVG,
			SKIN_09_LTVG,
			SKIN_10_LTVG,
			SKIN_11_LTVG,
			SKIN_12_LTVG};
		}
		
		/// <summary>
		/// see parent.
		/// </summary>
		public VirtualCurrencyPack[] GetCurrencyPacks() {
			return new VirtualCurrencyPack[] {ONE_FREEGIFT_TOKEN,TEN_FREEGIFT_TOKEN/*TENMUFF_PACK, FIFTYMUFF_PACK, FOURHUNDMUFF_PACK, THOUSANDMUFF_PACK*/};
		}
		
		/// <summary>
		/// see parent.
		/// </summary>
		public VirtualCategory[] GetCategories() {
			return new VirtualCategory[]{GENERAL_CATEGORY};
		}
		
		/** Static Final Members **/
		public const string FREEGIFT_TOKEN_ITEM_ID      = "colorjump_freegifttoken";

		//public const string MUFFIN_CURRENCY_ITEM_ID      = "currency_muffin";



	///////////////////////////////////////////////////////////////////////
		
		/*public const string TENMUFF_PACK_PRODUCT_ID      = "android.test.refunded";
		
		public const string FIFTYMUFF_PACK_PRODUCT_ID    = "android.test.canceled";
		
		public const string FOURHUNDMUFF_PACK_PRODUCT_ID = "android.test.purchased";
		
		public const string THOUSANDMUFF_PACK_PRODUCT_ID = "2500_pack";
		
		public const string MUFFINCAKE_ITEM_ID   = "fruit_cake";
		
		public const string PAVLOVA_ITEM_ID   = "pavlova";
		
		public const string CHOCLATECAKE_ITEM_ID   = "chocolate_cake";
		
		public const string CREAMCUP_ITEM_ID   = "cream_cup";*/
		
		public const string NO_ADS_LIFETIME_PRODUCT_ID = "colorjump_remove_ads";
		
		public const string COLORJUMP_SKIN_01_PRODUCT_ID = "colorjump_skin_01";
		public const string COLORJUMP_SKIN_02_PRODUCT_ID = "colorjump_skin_02";
		public const string COLORJUMP_SKIN_03_PRODUCT_ID = "colorjump_skin_03";
		public const string COLORJUMP_SKIN_04_PRODUCT_ID = "colorjump_skin_04";


		public const string COLORJUMP_SKIN_05_PRODUCT_ID = "colorjump_skin_05";
		public const string COLORJUMP_SKIN_06_PRODUCT_ID = "colorjump_skin_06";
		public const string COLORJUMP_SKIN_07_PRODUCT_ID = "colorjump_skin_07";
		public const string COLORJUMP_SKIN_08_PRODUCT_ID = "colorjump_skin_08";

		public const string COLORJUMP_SKIN_09_PRODUCT_ID = "colorjump_skin_09";
		public const string COLORJUMP_SKIN_10_PRODUCT_ID = "colorjump_skin_10";
		public const string COLORJUMP_SKIN_11_PRODUCT_ID = "colorjump_skin_11";
		public const string COLORJUMP_SKIN_12_PRODUCT_ID = "colorjump_skin_12";
		/** Virtual Currencies **/

		public static VirtualCurrency FREEGIFT_TOKEN = new VirtualCurrency(
			"FreeGiftToken",										// name
			"I am free gift token",												// description
			FREEGIFT_TOKEN_ITEM_ID							// item id
			);

		/*
		public static VirtualCurrency MUFFIN_CURRENCY = new VirtualCurrency(
			"Muffins",										// name
			"",												// description
			MUFFIN_CURRENCY_ITEM_ID							// item id
			);
		*/
		
		/** Virtual Currency Packs **/

		public static VirtualCurrencyPack ONE_FREEGIFT_TOKEN = new VirtualCurrencyPack(
			"1 token",                                   // name
			"give player one token",                       // description
			"freegift_token_one",                                   // item id
			1,												// number of currencies in the pack
			FREEGIFT_TOKEN_ITEM_ID,                        // the currency associated with this pack
			new PurchaseWithVirtualItem(FREEGIFT_TOKEN_ITEM_ID,0)
			);

		public static VirtualCurrencyPack TEN_FREEGIFT_TOKEN = new VirtualCurrencyPack(
			"10 tokens",                                   // name
			"give player 10 tokens",                       // description
			"freegift_token_ten",                                   // item id
			10,												// number of currencies in the pack
			FREEGIFT_TOKEN_ITEM_ID,                        // the currency associated with this pack
			new PurchaseWithVirtualItem(FREEGIFT_TOKEN_ITEM_ID,0)
			);
	/*
		public static VirtualCurrencyPack TENMUFF_PACK = new VirtualCurrencyPack(
			"10 Muffins",                                   // name
			"Test refund of an item",                       // description
			"muffins_10",                                   // item id
			10,												// number of currencies in the pack
			MUFFIN_CURRENCY_ITEM_ID,                        // the currency associated with this pack
			new PurchaseWithMarket(TENMUFF_PACK_PRODUCT_ID, 0.99)
			);
		
		public static VirtualCurrencyPack FIFTYMUFF_PACK = new VirtualCurrencyPack(
			"50 Muffins",                                   // name
			"Test cancellation of an item",                 // description
			"muffins_50",                                   // item id
			50,                                             // number of currencies in the pack
			MUFFIN_CURRENCY_ITEM_ID,                        // the currency associated with this pack
			new PurchaseWithMarket(FIFTYMUFF_PACK_PRODUCT_ID, 1.99)
			);
		
		public static VirtualCurrencyPack FOURHUNDMUFF_PACK = new VirtualCurrencyPack(
			"400 Muffins",                                  // name
			"Test purchase of an item",                 	// description
			"muffins_400",                                  // item id
			400,                                            // number of currencies in the pack
			MUFFIN_CURRENCY_ITEM_ID,                        // the currency associated with this pack
			new PurchaseWithMarket(FOURHUNDMUFF_PACK_PRODUCT_ID, 4.99)
			);
		
		public static VirtualCurrencyPack THOUSANDMUFF_PACK = new VirtualCurrencyPack(
			"1000 Muffins",                                 // name
			"Test item unavailable",                 		// description
			"muffins_1000",                                 // item id
			1000,                                           // number of currencies in the pack
			MUFFIN_CURRENCY_ITEM_ID,                        // the currency associated with this pack
			new PurchaseWithMarket(THOUSANDMUFF_PACK_PRODUCT_ID, 8.99)
			);
		*/
		/** Virtual Goods **/

		public static VirtualGood FREEGIFT_COUNTER = new SingleUseVG(
			"Free GIFT Counter",                                        		// name
			"Increase this counter when give away tokens",   	// description
			"freetoken_counter",                                        		// item id
			new PurchaseWithVirtualItem(FREEGIFT_TOKEN_ITEM_ID, 0));  // the way this virtual good is purchased

		public static VirtualGood ACCUMULATED_ACTIVETIME = new SingleUseVG(
			"Accumulated Game Active Time",                                        		// name
			"track how long player are activly play the game",   	// description
			"accumulated_activetime",                                        		// item id
			new PurchaseWithVirtualItem(FREEGIFT_TOKEN_ITEM_ID, 0));  // the way this virtual good is purchased
	/*
		public static VirtualGood MUFFINCAKE_GOOD = new SingleUseVG(
			"Fruit Cake",                                       		// name
			"Customers buy a double portion on each purchase of this cake", // description
			"fruit_cake",                                       		// item id
			new PurchaseWithVirtualItem(MUFFIN_CURRENCY_ITEM_ID, 225)); // the way this virtual good is purchased
		
		public static VirtualGood PAVLOVA_GOOD = new SingleUseVG(
			"Pavlova",                                         			// name
			"Gives customers a sugar rush and they call their friends", // description
			"pavlova",                                          		// item id
			new PurchaseWithVirtualItem(MUFFIN_CURRENCY_ITEM_ID, 175)); // the way this virtual good is purchased
		
		public static VirtualGood CHOCLATECAKE_GOOD = new SingleUseVG(
			"Chocolate Cake",                                   		// name
			"A classic cake to maximize customer satisfaction",	 		// description
			"chocolate_cake",                                   		// item id
			new PurchaseWithVirtualItem(MUFFIN_CURRENCY_ITEM_ID, 250)); // the way this virtual good is purchased
		
		
		public static VirtualGood CREAMCUP_GOOD = new SingleUseVG(
			"Cream Cup",                                        		// name
			"Increase bakery reputation with this original pastry",   	// description
			"cream_cup",                                        		// item id
			new PurchaseWithVirtualItem(MUFFIN_CURRENCY_ITEM_ID, 50));  // the way this virtual good is purchased
		*/
		
		/** Virtual Categories **/
		// The muffin rush theme doesn't support categories, so we just put everything under a general category.
		public static VirtualCategory GENERAL_CATEGORY = new VirtualCategory(
			"General", new List<string>(new string[] { })
			);
		
		
		/** LifeTimeVGs **/
		// Note: create non-consumable items using LifeTimeVG with PuchaseType of PurchaseWithMarket
		public static VirtualGood NO_ADS_LTVG = new LifetimeVG(
			"No Ads", 														// name
			"No More Ads!",				 									// description
			"no_ads",														// item id
			new PurchaseWithMarket(NO_ADS_LIFETIME_PRODUCT_ID, 0.99));	// the way this virtual good is purchased

		public static VirtualGood SKIN_01_LTVG = new LifetimeVG(
			"skin01", 														// name
			"skin01",				 									// description
			"skin01",														// item id
			new PurchaseWithMarket(COLORJUMP_SKIN_01_PRODUCT_ID, 0.99));	// the way this virtual good is purchased

		public static VirtualGood SKIN_02_LTVG = new LifetimeVG(
			"skin02", 														// name
			"skin02",				 									// description
			"skin02",														// item id
			new PurchaseWithMarket(COLORJUMP_SKIN_02_PRODUCT_ID, 0.99));	// the way this virtual good is purchased

		public static VirtualGood SKIN_03_LTVG = new LifetimeVG(
			"skin03", 														// name
			"skin03",				 									// description
			"skin03",														// item id
			new PurchaseWithMarket(COLORJUMP_SKIN_03_PRODUCT_ID, 0.99));	// the way this virtual good is purchased

		public static VirtualGood SKIN_04_LTVG = new LifetimeVG(
			"skin04", 														// name
			"skin04",				 									// description
			"skin04",														// item id
			new PurchaseWithMarket(COLORJUMP_SKIN_04_PRODUCT_ID, 0.99));	// the way this virtual good is purchased

		public static VirtualGood SKIN_05_LTVG = new LifetimeVG(
			"skin05", 														// name
			"skin05",				 									// description
			"skin05",														// item id
			new PurchaseWithMarket(COLORJUMP_SKIN_05_PRODUCT_ID, 0.99));	// the way this virtual good is purchased	

		public static VirtualGood SKIN_06_LTVG = new LifetimeVG(
			"skin06", 														// name
			"skin06",				 									// description
			"skin06",														// item id
			new PurchaseWithMarket(COLORJUMP_SKIN_06_PRODUCT_ID, 0.99));	// the way this virtual good is purchased

		public static VirtualGood SKIN_07_LTVG = new LifetimeVG(
			"skin07", 														// name
			"skin07",				 									// description
			"skin07",														// item id
			new PurchaseWithMarket(COLORJUMP_SKIN_07_PRODUCT_ID, 0.99));	// the way this virtual good is purchased

		public static VirtualGood SKIN_08_LTVG = new LifetimeVG(
			"skin08", 														// name
			"skin08",				 									// description
			"skin08",														// item id
			new PurchaseWithMarket(COLORJUMP_SKIN_08_PRODUCT_ID, 0.99));	// the way this virtual good is purchased

		public static VirtualGood SKIN_09_LTVG = new LifetimeVG(
			"skin09", 														// name
			"skin09",				 									// description
			"skin09",														// item id
			new PurchaseWithMarket(COLORJUMP_SKIN_09_PRODUCT_ID, 0.99));	// the way this virtual good is purchased

		public static VirtualGood SKIN_10_LTVG = new LifetimeVG(
			"skin10", 														// name
			"skin10",				 									// description
			"skin10",														// item id
			new PurchaseWithMarket(COLORJUMP_SKIN_10_PRODUCT_ID, 0.99));	// the way this virtual good is purchased

		public static VirtualGood SKIN_11_LTVG = new LifetimeVG(
			"skin11", 														// name
			"skin11",				 									// description
			"skin11",														// item id
			new PurchaseWithMarket(COLORJUMP_SKIN_11_PRODUCT_ID, 0.99));	// the way this virtual good is purchased

		public static VirtualGood SKIN_12_LTVG = new LifetimeVG(
			"skin12", 														// name
			"skin12",				 									// description
			"skin12",														// item id
			new PurchaseWithMarket(COLORJUMP_SKIN_12_PRODUCT_ID, 0.99));	// the way this virtual good is purchased
}
 
