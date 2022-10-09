/**
 * AdmobAdListener.cs
 * 
 * AdmobAdListener listens to the Admob Ad events.
 * File location: Assets/Scripts/NeatPlug/Ads/AdmobAd/AdmobAdListener.cs
 * 
 * Please read the code comments carefully, or visit 
 * http://www.neatplug.com/integration-guide-unity3d-admob-ad-plugin to find information 
 * about how to integrate and use this program.
 * 
 * End User License Agreement: http://www.neatplug.com/eula
 * 
 * (c) Copyright 2012 NeatPlug.com All Rights Reserved.
 * 
 * @version 1.6.6
 * @sdk_version(android) google-play-services-4.3.23
 * @sdk_version(ios) 6.9.2
 *
 */

using UnityEngine;
using System.Collections;
using ChartboostSDK;

public class AdmobAdListener : MonoBehaviour {
	
	// Don't forget to switch the debug off before building for app store submission.
	public bool debug = true;	
	
	private static bool _instanceFound = false;
	
	void Awake()
	{
		// Do not modify the codes below.		
		AdmobAdAgent.RetainGameObject(ref _instanceFound, gameObject, null);
		AdmobAd.Instance();
	}
	
	void OnEnable()
	{
		// Register the Ad events.
		// Do not modify the codes below.	
		AdmobAdAgent.OnReceiveAd += OnReceiveAd;
		AdmobAdAgent.OnFailedToReceiveAd += OnFailedToReceiveAd;
		AdmobAdAgent.OnPresentScreen += OnPresentScreen;
		AdmobAdAgent.OnDismissScreen += OnDismissScreen;
		AdmobAdAgent.OnLeaveApplication += OnLeaveApplication;
		AdmobAdAgent.OnReceiveAdInterstitial += OnReceiveAdInterstitial;
		AdmobAdAgent.OnFailedToReceiveAdInterstitial += OnFailedToReceiveAdInterstitial;
		AdmobAdAgent.OnPresentScreenInterstitial += OnPresentScreenInterstitial;
		AdmobAdAgent.OnDismissScreenInterstitial += OnDismissScreenInterstitial;
		AdmobAdAgent.OnLeaveApplicationInterstitial += OnLeaveApplicationInterstitial;	
		AdmobAdAgent.OnAdShown += OnAdShown;
		AdmobAdAgent.OnAdHidden += OnAdHidden;		
	}

	void OnDisable()
	{
		// Unregister the Ad events.
		// Do not modify the codes below.		
		AdmobAdAgent.OnReceiveAd -= OnReceiveAd;
		AdmobAdAgent.OnFailedToReceiveAd -= OnFailedToReceiveAd;
		AdmobAdAgent.OnPresentScreen -= OnPresentScreen;
		AdmobAdAgent.OnDismissScreen -= OnDismissScreen;
		AdmobAdAgent.OnLeaveApplication -= OnLeaveApplication;
		AdmobAdAgent.OnReceiveAdInterstitial -= OnReceiveAdInterstitial;
		AdmobAdAgent.OnFailedToReceiveAdInterstitial -= OnFailedToReceiveAdInterstitial;
		AdmobAdAgent.OnPresentScreenInterstitial -= OnPresentScreenInterstitial;
		AdmobAdAgent.OnDismissScreenInterstitial -= OnDismissScreenInterstitial;
		AdmobAdAgent.OnLeaveApplicationInterstitial -= OnLeaveApplicationInterstitial;
		AdmobAdAgent.OnAdShown -= OnAdShown;
		AdmobAdAgent.OnAdHidden -= OnAdHidden;		
	}	
	
	/**
	 * Fired when a banner Ad is received successfully.
	 */
	void OnReceiveAd()
	{
		if (debug)
			Debug.Log (this.GetType().ToString() + " - OnReceiveAd() Fired.");
		
		/// Your code here...
	}
	
	/**
	 * Fired when a banner Ad fails to be received.
	 * 
	 * @param err
	 *          string - The error string
	 */
	void OnFailedToReceiveAd(string err)
	{
		if (debug)
			Debug.Log (this.GetType().ToString() + " - OnFailedToReceiveAd() Fired. Error: " + err);
		
		/// Your code here...

	}
	
	/**
	 * Fired when a Banner Ad screen is presented.
	 */
	void OnPresentScreen()
	{
		if (debug)
			Debug.Log (this.GetType().ToString() + " - OnPresentScreen() Fired.");
		
		/// Your code here...
	}
	
	/**
	 * Fired when a Banner Ad screen is dismissed.
	 */
	void OnDismissScreen()
	{
		if (debug)
			Debug.Log (this.GetType().ToString() + " - OnDismissScreen() Fired.");
		
		/// Your code here...
	}	
	
	/**
	 * Fired when the App loses focus after a Banner Ad is clicked.
	 */
	void OnLeaveApplication()
	{
		if (debug)
			Debug.Log (this.GetType().ToString() + " - OnLeaveApplication() Fired.");
		
		/// Your code here...
	}
	
	/**
	 * Fired when an Interstitial Ad is received successfully.
	 */
	void OnReceiveAdInterstitial()
	{
		if (debug)
			Debug.Log (this.GetType().ToString() + " - OnReceiveAdInterstitial() Fired.");
		
		/// Your code here...
	}
	
	/**
	 * Fired when an Interstitial Ad fails to be received.
	 * 
	 *  @param err
	 *          string - The error string
	 */
	void OnFailedToReceiveAdInterstitial(string err)
	{
		if (debug)
			Debug.Log (this.GetType().ToString() + " - OnFailedToReceiveAdInterstitial() Fired. Error: " + err);
		
		/// Your code here...
		Chartboost.showInterstitial(CBLocation.Default);
	}
	
	/**
	 * Fired when an Interstitial Ad screen is presented.
	 */
	void OnPresentScreenInterstitial()
	{
		if (debug)
			Debug.Log (this.GetType().ToString() + " - OnPresentScreenInterstitial() Fired.");
		
		/// Your code here...
	}
	
	/**
	 * Fired when an Interstitial Ad screen is dismissed.
	 */
	void OnDismissScreenInterstitial()
	{
		if (debug)
			Debug.Log (this.GetType().ToString() + " - OnDismissScreenInterstitial() Fired.");
		
		/// Your code here...
	}
	
	/**
	 * Fired when the App loses focus after an Interstitial Ad is clicked.
	 */
	void OnLeaveApplicationInterstitial()
	{
		if (debug)
			Debug.Log (this.GetType().ToString() + " - OnLeaveApplicationInterstitial() Fired.");
		
		/// Your code here...
	}
	
	/**
	 * Fired when the banner Ad is shown.
	 */
	void OnAdShown()
	{
		if (debug)
			Debug.Log (this.GetType().ToString() + " - OnAdShown() Fired.");
		
		/// Your code here...
	}
	
	/**
	 * Fired when the banner Ad is hidden.
	 */
	void OnAdHidden()
	{
		if (debug)
			Debug.Log (this.GetType().ToString() + " - OnAdHidden() Fired.");
		
		/// Your code here...
	}
	
}
