using UnityEngine;
using System.Collections;
using System;

public class playAdbanerC : MonoBehaviour {

	private playAdbanerC s_Controller;
	private AndroidJavaObject jo1;
	private bool jflag = true , showflag = true , onceflag =true;
	private float showtime = 100f; 

	void Awake(){
		s_Controller = this;
		#if UNITY_ANDROID    
		jo1 = new AndroidJavaObject("com.example.googleplayplugin.playadsC");
		#endif
	}
	// Use this for initialization
	void Start () {
	
	}

	public void showBanner(){
		if(onceflag == true){
			jo1.Call("showAd");
		}

	}
	public void hideBanner(){
		if(onceflag == true){
			try{
			#if UNITY_ANDROID
				jo1.Call("hideAd");
			#endif
			}
			catch(Exception e){

			}
		}
	}
	void OnGUI()
	{
		//  	
		//    	if(jo.Call.<boolean>("isInterstitialLoaded"))
		//    	{
		//    		jo.Call("displayInterstitial");
		//    	}
	}
}






