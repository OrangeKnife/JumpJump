using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Text;
using System.Text.RegularExpressions;
namespace anysdk {
	public enum AnySDKType
	{
		Ads = 1,
		Analytics = 2,
		IAP = 3,
		Share = 4,
		User = 5,
		Social = 6,
		Push = 7,
	} ;
	public class AnySDKUtil 
	{
	#if UNITY_ANDROID
		public const string ANYSDK_PLATFORM = "anysdk";
	#else
		public const string ANYSDK_PLATFORM= "__Internal";
	#endif
		public const int MAX_CAPACITY_NUM = 1024;
		/**
     	@brief the Dictionary type change to the string type 
    	 @param Dictionary<string,string> 
    	 @return  string
    	*/
		public static string dictionaryToString( Dictionary<string,string> maps  ) 
		{
			StringBuilder param = new StringBuilder();
			if ( null != maps ) {  
				foreach (KeyValuePair<string, string> kv in maps){
					if ( param.Length == 0)  
					{  
						param.AppendFormat("{0}={1}",kv.Key,kv.Value);
					}  
					else  
					{  
						param.AppendFormat("&{0}={1}",kv.Key,kv.Value); 
					} 
				} 
			}  
//			byte[] tempStr = Encoding.UTF8.GetBytes (param.ToString ());
//			string msgBody = Encoding.Default.GetString(tempStr);
			return param.ToString ();			
		}

		/**
     	@brief the Dictionary type change to the string type 
    	 @param Dictionary
    	 @return  string
    	*/
		public static Dictionary<string,string> stringToDictionary( string message ) 
		{
			Dictionary<string,string> param = new Dictionary<string,string>();
			if ( null != message) {
				Regex regex = new Regex(@"code=(.*)&msg=(.*)");
				string[] tokens = regex.Split(message);
				string code = tokens[1];
				string msg = tokens[2];
				param.Add("code=",code);
				param.Add("msg=",msg);
			}  
			
			return param;				
		}

		/**
     	@brief the List type change to the string type 
    	 @param List<string> 
    	 @return  string
    	*/
		public static string ListToString( List<string> list  ) 
		{
			StringBuilder param = new StringBuilder();
			if ( null != list ) {  
				foreach (string kv in list){
					if ( param.Length == 0)  
					{  
						param.AppendFormat("{0}",kv);
					}  
					else  
					{  
						param.AppendFormat("&{0}",kv); 
					} 
				} 
			}  
//			byte[] tempStr = Encoding.UTF8.GetBytes (param.ToString ());
//			string msgBody = Encoding.Default.GetString(tempStr);
			return param.ToString ();			
		}

		/**
     	@brief the string type change to the List type 
    	 @param string 
    	 @return  List<string>
    	*/
		public static List<string>   StringToList( string value  ) 
		{
			string[] temp = value.Split('&');
			List<string> param = new  List<string>();
			if ( null != temp ) {  
				foreach (string kv in temp){
					param.Add(kv); 
				} 
			}  
			
			return param;			
		}

		#if UNITY_ANDROID		
		private static AndroidJavaClass mAndroidJavaClass;
		#endif

		public static void registerActionCallback(AnySDKType type,MonoBehaviour gameObject,string functionName)
		{
	#if UNITY_ANDROID		
			if (mAndroidJavaClass == null) 
			{
				mAndroidJavaClass = new AndroidJavaClass( "com.anysdk.framework.unity.MessageHandle" );
			}
			string gameObjectName = gameObject.gameObject.name;
			mAndroidJavaClass.CallStatic( "registerActionResultCallback", new object[]{(int)type,gameObjectName,functionName});
	#endif

		}




	}
}
