/**
 * AdmobAdTest.cs
 * 
 * A Test script for demonstrating the usage of AdmobAd Plugin.
 * 
 * Please read the code comments carefully, or visit 
 * http://www.neatplug.com/integration-guide-unity3d-admob-ad-plugin to find information 
 * about how to integrate and use this program.
 * 
 * End User License Agreement: http://www.neatplug.com/eula
 * 
 * (c) Copyright 2013 NeatPlug.com All Rights Reserved. 
 *
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AdmobAdTest : MonoBehaviour {	
	
	private float _buttonWidth =  (Mathf.Min(Screen.height, Screen.width) - 40f) / 3f;	
	private float _buttonHeight = 60f;	
	private float _topMargin = 30f;
	
	void Awake() {
		GameObject go = GameObject.Find("AdmobAdAgent");
		if (go == null)
		{
			string levelName = Application.loadedLevelName;			
			if (!levelName.Equals("SampleScene", StringComparison.InvariantCultureIgnoreCase))
			{
				Application.LoadLevel("SampleScene");
			}
		}		
	}
	
	void OnGUI() {	
		
		UnityEngine.GUI.Label(new Rect(10f, 10f, Screen.width * 0.5f, _buttonHeight), Application.loadedLevelName);
		
		if (UnityEngine.GUI.Button (new Rect(10f, 100f + _topMargin, _buttonWidth, _buttonHeight), "Hide Banner")) {		
			AdmobAd.Instance().HideBannerAd();
		}
		
		if (UnityEngine.GUI.Button (new Rect(20f + _buttonWidth, 100f + _topMargin, _buttonWidth, _buttonHeight), "Show Banner")) {		
			AdmobAd.Instance().ShowBannerAd();
		}		
		
		if (UnityEngine.GUI.Button (new Rect(30f + 2f * _buttonWidth, 100f + _topMargin, _buttonWidth, _buttonHeight), "Destroy Banner")) {		
			AdmobAd.Instance().DestroyBannerAd();
		}
		
		if (UnityEngine.GUI.Button (new Rect(10f, 170f + _topMargin, _buttonWidth, _buttonHeight), "Load Banner (Bottom)")) {			 
			AdmobAd.Instance().LoadBannerAd(AdmobAd.BannerAdType.Universal_SmartBanner, AdmobAd.AdLayout.Bottom_Centered);	
		}		
		
		if (UnityEngine.GUI.Button (new Rect(20f + _buttonWidth, 170f + _topMargin, _buttonWidth, _buttonHeight), "Load Banner (Top)")) {			
			AdmobAd.Instance().LoadBannerAd(AdmobAd.BannerAdType.Universal_SmartBanner, AdmobAd.AdLayout.Top_Centered);
		}		
		
		if (UnityEngine.GUI.Button (new Rect(30f + 2f * _buttonWidth, 170f + _topMargin, _buttonWidth, _buttonHeight), "Load Banner\n(MRect Center)")) {		
			AdmobAd.Instance().LoadBannerAd(AdmobAd.BannerAdType.Tablets_IAB_MRect_300x250, AdmobAd.AdLayout.Middle_Centered);
		}		
		
		
		if (UnityEngine.GUI.Button (new Rect(10f, 240f + _topMargin, _buttonWidth, _buttonHeight), "Load Banner (Bottom)\nWith Offset (0, 100)")) {			 
			AdmobAd.Instance().LoadBannerAd(AdmobAd.BannerAdType.Universal_SmartBanner, AdmobAd.AdLayout.Bottom_Centered, new Vector2(0, 100), false);	
		}		
		
		if (UnityEngine.GUI.Button (new Rect(20f + _buttonWidth, 240f + _topMargin, _buttonWidth, _buttonHeight), "Load Banner (Top)\nWith Offset (0, 100)")) {			
			AdmobAd.Instance().LoadBannerAd(AdmobAd.BannerAdType.Universal_SmartBanner, AdmobAd.AdLayout.Top_Centered, new Vector2(0, 100), false);
		}		
			
		if (UnityEngine.GUI.Button (new Rect(30f + 2f * _buttonWidth, 240f + _topMargin, _buttonWidth, _buttonHeight), "Move to Top")) {		
			AdmobAd.Instance().RepositionBannerAd(AdmobAd.AdLayout.Top_Centered);
		}	
		
		if (UnityEngine.GUI.Button (new Rect(10f, 310f + _topMargin, _buttonWidth, _buttonHeight), "Load Banner\n(Custom Size)")) {			
			AdmobAd.Instance().LoadBannerAd(new Vector2(Screen.width * 0.8f, Screen.height * 0.1f), AdmobAd.AdLayout.Top_Centered);
		}
		
		if (UnityEngine.GUI.Button (new Rect(10f, 380f + _topMargin, _buttonWidth, _buttonHeight), "Load & Show\nInterstitial")) {			
			AdmobAd.Instance().LoadInterstitialAd(false);		
		}		
		
		if (UnityEngine.GUI.Button (new Rect(20f + _buttonWidth, 380f + _topMargin, _buttonWidth, _buttonHeight), "Load & Hide\nInterstitial")) {			
			AdmobAd.Instance().LoadInterstitialAd(true);
		}		
		
		if (UnityEngine.GUI.Button (new Rect(30f + 2f * _buttonWidth, 380f + _topMargin, _buttonWidth, _buttonHeight), "Show Interstitial")) {		
			AdmobAd.Instance().ShowInterstitialAd();
		}	
		
		if (UnityEngine.GUI.Button (new Rect(10f, 450f + _topMargin, _buttonWidth, _buttonHeight), "Disable Ad\n(Can be used with IAP)")) {			
			AdmobAd.Instance().DisableAd();		
		}
		
		if (UnityEngine.GUI.Button (new Rect(20f + _buttonWidth, 450f + _topMargin, _buttonWidth, _buttonHeight), "Enable Ad\n(Re-enable the Ads)")) {			
			AdmobAd.Instance().EnableAd();	
		}
		
		if (UnityEngine.GUI.Button (new Rect(30f + 2f * _buttonWidth, 450f + _topMargin, _buttonWidth, _buttonHeight), "Is Ad Enabled?")) {			
			Debug.Log("Is Ad Enabled? : " + AdmobAd.Instance().IsAdEnabled().ToString());
		}
		
		if (UnityEngine.GUI.Button (new Rect(10f, 520f + _topMargin, _buttonWidth, _buttonHeight), "Load Next Scene")) {
			
			string levelName = Application.loadedLevelName;
			string nextScene = null;
			
			if (levelName.Equals("SampleScene", StringComparison.InvariantCultureIgnoreCase))
			{
				nextScene = "SampleSceneNext1";
			}
			else if (levelName.Equals("SampleSceneNext1", StringComparison.InvariantCultureIgnoreCase))
			{
				nextScene = "SampleSceneNext2";
			}
			else if (levelName.Equals("SampleSceneNext2", StringComparison.InvariantCultureIgnoreCase))
			{
				nextScene = "SampleScene";
			}
			
			if (nextScene != null)
			{
				Application.LoadLevel(nextScene);	
			}
		}
		
		
		
	}
		
	// Quit test app when back button is tapped.
	void Update () {
	
		if (Input.GetKeyDown(KeyCode.Escape)) 
		{					
			Application.Quit();
		}	
	}
}
