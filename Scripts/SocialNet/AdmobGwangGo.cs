using UnityEngine;
using System.Collections;

public class AdmobGwangGo : MonoBehaviour {
	private AdmobGwangGo s_Controller;
	private AndroidJavaObject jo;
	public bool jflag = false;
	// Use this for initialization

	void Awake(){
		s_Controller = this;
		#if UNITY_ANDROID    
		
		jo = new AndroidJavaObject("com.example.googleplayplugin.playads");
		#endif
	}

	void OnGUI()
	{
		if(jflag == true){

			if(jo.Call<bool>("isInterstitialLoaded"))
			{
				jo.Call("displayInterstitial");
				jflag = false ;
			}
		}
		
	}
	// Update is called once per frame

}

//
//#pragma strict
//
//public class PlayAdController extends MonoBehaviour {
//	private static var s_Controller : PlayAdController;
//	private static var jo:AndroidJavaObject;
//	private static var jo1:AndroidJavaObject;
//	private var jflag  : boolean =true ;
//	
//	function Awake()
//	{
//		s_Controller = this;
//		#if UNITY_ANDROID    
//		
//		jo = new AndroidJavaObject("com.example.googleplayplugin.playads");
//		
//		jo1 = new AndroidJavaObject("com.example.googleplayplugin1.playads");
//		#endif
//	}
//	function OnGUI()
//	{
//		if(jflag == true){
//			if(jo.Call.<boolean>("isInterstitialLoaded"))
//			{
//				jo.Call("displayInterstitial");
//				jflag = false ;
//			}
//		}
//		
//	}
//}



