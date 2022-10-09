/**
 * AdmobAd.cs
 * 
 * A Singleton class encapsulating public access methods for Admob Ad processes.
 * 
 * Please read the code comments carefully, or visit 
 * http://www.neatplug.com/integration-guide-unity3d-admob-ad-plugin to find information how 
 * to use this program.
 * 
 * End User License Agreement: http://www.neatplug.com/eula
 * 
 * (c) Copyright 2012 NeatPlug.com All rights reserved.
 * 
 * @version 1.6.6
 * @sdk_version(android) google-play-services-4.3.23
 * @sdk_version(ios) 6.9.2
 *
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AdmobAd  {
	
	#region Fields
	
	public enum BannerAdType
	{
		Universal_SmartBanner = 0,
		Universal_Banner_320x50,
		Tablets_IAB_MRect_300x250,
		Tablets_IAB_Banner_468x60,
		Tablets_IAB_LeaderBoard_728x90,
		Tablets_IAB_SkyScraper_160x600
	};
		
	public enum AdLayout
	{
		Top_Centered = 0,
		Top_Left,
		Top_Right,
		Bottom_Centered,
		Bottom_Left,
		Bottom_Right,
		Middle_Centered,
		Middle_Left,
		Middle_Right
	};
	
	public enum TagForChildrenDirectedTreatment
	{
		Unset = 0,
		Yes = 1,
		No = 2
	};
	
#if UNITY_ANDROID
	
	private class AdmobAdNativeHelper : IAdmobAdNativeHelper {		
	
		private AndroidJavaObject _plugin = null;

		public AdmobAdNativeHelper()
		{
			
		}
		
		public void CreateInstance(string className, string instanceMethod)
		{
			AndroidJavaClass jClass = new AndroidJavaClass(className);
			_plugin = jClass.CallStatic<AndroidJavaObject>(instanceMethod);	
		}
		
		public void Call(string methodName, params object[] args)
		{
			_plugin.Call(methodName, args);
		}
		
		public void Call(string methodName, string signature, object arg)
		{
			var method = AndroidJNI.GetMethodID(_plugin.GetRawClass(), methodName, signature);			
			AndroidJNI.CallObjectMethod(_plugin.GetRawObject(), method, AndroidJNIHelper.CreateJNIArgArray(new object[] {arg}));
		}
		
		public ReturnType Call<ReturnType> (string methodName, params object[] args)
		{
			return _plugin.Call<ReturnType>(methodName, args);	
		}
	
	};	
	
#endif
	
	private static AdmobAd _instance = null;
	
	#endregion
	
	#region Functions
	
	/**
	 * Constructor.
	 */
	private AdmobAd()
	{	
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().SetNativeHelper(new AdmobAdNativeHelper());
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance();
#endif
#if UNITY_WP8
		AdmobAdWP.Instance();
#endif		
	}
	
	/**
	 * Instance method.
	 */
	public static AdmobAd Instance()
	{		
		if (_instance == null) 
		{
			_instance = new AdmobAd();
		}
		
		return _instance;
	}	
	
	/**
	 * Initialization, set the Ad Unit ID.
	 * 
	 * This function sets the bannerAdUnitId and interstitialAdUnitId to
	 * the specified value.
	 * 
	 * @param adUnitId
	 *            string - Your Ad Unit ID
	 */
	public void Init(string adUnitId)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().Init(adUnitId, adUnitId, false);		
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().Init(adUnitId, adUnitId, false);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().Init(adUnitId, adUnitId, false);
#endif		
	}
	
	/**
	 * Initialization, set the Ad Unit IDs.
	 * 
	 * This function is for the new Admob frond-end.
	 * NOTE: the Ad Unit ID is in the following form:
	 * ca-app-pub-XXXXXXXXXXXXXXXX/NNNNNNNNNN
	 * 
	 * @param bannerAdUnitId
	 *            string - Your Banner Ad Unit ID.
	 * 
	 * @param interstitialAdUnitId
	 *            string - Your Interstitial Ad Unit ID.
	 * 
	 */
	public void Init(string bannerAdUnitId, string interstitialAdUnitId)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().Init(bannerAdUnitId, interstitialAdUnitId, false);		
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().Init(bannerAdUnitId, interstitialAdUnitId, false);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().Init(bannerAdUnitId, interstitialAdUnitId, false);
#endif		
	}	
	
	/**
	 * Initialization, set the Ad Unit IDs.
	 * 
	 * This function is for the new Admob frond-end.
	 * NOTE: the Ad Unit ID is in the following form:
	 * ca-app-pub-XXXXXXXXXXXXXXXX/NNNNNNNNNN
	 * 
	 * @param bannerAdUnitId
	 *            string - Your Banner Ad Unit ID.
	 * 
	 * @param interstitialAdUnitId
	 *            string - Your Interstitial Ad Unit ID.
	 * 
	 * @param testMode
	 *            bool - True for test mode on, false for off.
	 * 
	 */
	public void Init(string bannerAdUnitId, string interstitialAdUnitId, bool testMode)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().Init(bannerAdUnitId, interstitialAdUnitId, testMode);		
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().Init(bannerAdUnitId, interstitialAdUnitId, testMode);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().Init(bannerAdUnitId, interstitialAdUnitId, testMode);
#endif		
	}
	
	/**
	 * Initialization, set the Ad Unit IDs.
	 * 
	 * This function is for the new Admob frond-end.
	 * NOTE: the Ad Unit ID is in the following form:
	 * ca-app-pub-XXXXXXXXXXXXXXXX/NNNNNNNNNN
	 * 
	 * @param bannerAdUnitIdAndroid
	 *            string - Your Banner Ad Unit ID (Android).
	 * 
	 * @param interstitialAdUnitIdAndroid
	 *            string - Your Interstitial Ad Unit ID (Android).
	 * 
	 * @param bannerAdUnitIdIOS
	 *            string - Your Banner Ad Unit ID (iOS).
	 * 
	 * @param interstitialAdUnitIdIOS
	 *            string - Your Interstitial Ad Unit ID (iOS).
	 * 
	 * @param bannerAdUnitIdWP
	 *            string - Your Banner Ad Unit ID (Windows Phone).
	 * 
	 * @param interstitialAdUnitIdWP
	 *            string - Your Interstitial Ad Unit ID (Windows Phone).
	 * 
	 * @param testMode
	 *            bool - True for test mode on, false for off.
	 * 
	 */
	public void Init(string bannerAdUnitIdAndroid, string interstitialAdUnitIdAndroid, 
	                 string bannerAdUnitIdIOS, string interstitialAdUnitIdIOS,
	                 string bannerAdUnitIdWP, string interstitialAdUnitIdWP,
	                 bool testMode)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().Init(bannerAdUnitIdAndroid, interstitialAdUnitIdAndroid, testMode);		
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().Init(bannerAdUnitIdIOS, interstitialAdUnitIdIOS, testMode);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().Init(bannerAdUnitIdWP, interstitialAdUnitIdWP, testMode);
#endif		
	}	
	
	/**
	 * Set test mode.
	 *
	 * @param testMode
	 *             bool - true for test mode On, false for test mode Off.
	 */
	public void SetTestMode(bool testMode)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().SetTestMode(testMode);		
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().SetTestMode(testMode);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().SetTestMode(testMode);
#endif		
	}
	
	/**
	 * Load a Banner Ad and show it it immediately once loaded.
	 * 
	 * @param adType
	 *           BannerAdType - type of the Ad.
	 * 
	 * @param layout
	 *           AdLayout - in which layout the Ad should display.
	 *	
	 */
	public void LoadBannerAd(BannerAdType adType, AdLayout layout)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().LoadBannerAd((int)adType, (int)layout, 0, 0, false, null, 0);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().LoadBannerAd((int)adType, (int)layout, 0, 0, false, null, 0);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().LoadBannerAd((int)adType, (int)layout, 0, 0, false, null, 0);
#endif		
	}

	/**
	 * Load a Banner Ad.
	 * 
	 * @param adType
	 *           BannerAdType - type of the Ad.
	 * 
	 * @param layout
	 *           AdLayout - in which layout the Ad should display.
	 * 
     * @param hide
	 *           bool - whether the ad should keep being invisible after loaded,
	 *                  true for making the ad hidden, false for showing the 
	 *                  ad immediately. if the parameter is set to true, then 
	 *                  you need to programmatically display the ad by calling 
	 *                  ShowBannerAd() after you get notified with event
	 *                  OnReceiveAd from AdmobAdListener.
	 *	
	 */
	public void LoadBannerAd(BannerAdType adType, AdLayout layout, bool hide)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().LoadBannerAd((int)adType, (int)layout, 0, 0, hide, null, 0);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().LoadBannerAd((int)adType, (int)layout, 0, 0, hide, null, 0);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().LoadBannerAd((int)adType, (int)layout, 0, 0, hide, null, 0);
#endif		
	}
	
	/**
	 * Load a Banner Ad.
	 * 
	 * @param adType
	 *           BannerAdType - Type of the Ad.
	 * 
	 * @param layout
	 *           AdLayout - In which layout the Ad should display.
	 * 
	 * @param offsetX
	 *           int - The horizontal offset of the Ad, if the layout is set to left, 
	 *                 the offset is from the left edge of screen to the left edge of 
	 *                 the Ad; if the layout is set to right, the offset is from the 
	 *                 right edge of screen to the right edge of the Ad. 
	 *                 The offset is in pixels.
	 * 
	 * @param offsetY
	 *           int - The vertical offset of the Ad, if the layout is set to top, 
	 *                 the offset is from the top edge of screen to the top edge of 
	 *                 the Ad; if the layout is set to bottom, the offset is from the 
	 *                 bottom edge of screen to the bottom edge of the Ad. 
	 *                 The offset is in pixels.                 
	 * 
     * @param hide
	 *           bool - whether the ad should keep being invisible after loaded,
	 *                  true for making the ad hidden, false for showing the 
	 *                  ad immediately. if the parameter is set to true, then 
	 *                  you need to programmatically display the ad by calling 
	 *                  ShowBannerAd() after you get notified with event
	 *                  OnReceiveAd from AdmobAdListener.
	 *	
	 */
	[Obsolete("LoadBannerAd(BannerAdType adType, AdLayout layout, int offsetX, int offsetY, bool hide) is deprecated, please call LoadBannerAd(BannerAdType adType, AdLayout layout, Vector2 offset, bool hide) instead.")]
	public void LoadBannerAd(BannerAdType adType, AdLayout layout, int offsetX, int offsetY, bool hide)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().LoadBannerAd((int)adType, (int)layout, offsetX, offsetY, hide, null, 0);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().LoadBannerAd((int)adType, (int)layout, offsetX, offsetY, hide, null, 0);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().LoadBannerAd((int)adType, (int)layout, offsetX, offsetY, hide, null, 0);
#endif			
	}
	
   /**
	 * Load a Banner Ad.
	 * 
	 * @param adType
	 *           BannerAdType - Type of the Ad.
	 * 
	 * @param layout
	 *           AdLayout - In which layout the Ad should display.
	 * 
	 * @param offset
	 *           Vector2 - The offset of the Ad.
	 * 
	 *           offset.x - The horizontal offset of the Ad, if the layout is set to left, 
	 *                 the offset is from the left edge of screen to the left edge of 
	 *                 the Ad; if the layout is set to right, the offset is from the 
	 *                 right edge of screen to the right edge of the Ad. 
	 *                 The offset is in pixels.	
	 *           offset.y - The vertical offset of the Ad, if the layout is set to top, 
	 *                 the offset is from the top edge of screen to the top edge of 
	 *                 the Ad; if the layout is set to bottom, the offset is from the 
	 *                 bottom edge of screen to the bottom edge of the Ad. 
	 *                 The offset is in pixels.                 
	 * 
     * @param hide
	 *           bool - whether the ad should keep being invisible after loaded,
	 *                  true for making the ad hidden, false for showing the 
	 *                  ad immediately. if the parameter is set to true, then 
	 *                  you need to programmatically display the ad by calling 
	 *                  ShowBannerAd() after you get notified with event
	 *                  OnReceiveAd from AdmobAdListener.
	 *	
	 */
	public void LoadBannerAd(BannerAdType adType, AdLayout layout, Vector2 offset, bool hide)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().LoadBannerAd((int)adType, (int)layout, offset, hide, null, 0);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().LoadBannerAd((int)adType, (int)layout, offset, hide, null, 0);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().LoadBannerAd((int)adType, (int)layout, offset, hide, null, 0);
#endif			
	}	
	
	/**
	 * Load a Banner Ad.
	 * 
	 * @param adType
	 *           BannerAdType - Type of the Ad.
	 * 
	 * @param layout
	 *           AdLayout - In which layout the Ad should display.
	 * 
	 * @param offsetX
	 *           int - The horizontal offset of the Ad, if the layout is set to left, 
	 *                 the offset is from the left edge of screen to the left edge of 
	 *                 the Ad; if the layout is set to right, the offset is from the 
	 *                 right edge of screen to the right edge of the Ad. 
	 *                 The offset is in pixels.
	 * 
	 * @param offsetY
	 *           int - The vertical offset of the Ad, if the layout is set to top, 
	 *                 the offset is from the top edge of screen to the top edge of 
	 *                 the Ad; if the layout is set to bottom, the offset is from the 
	 *                 bottom edge of screen to the bottom edge of the Ad. 
	 *                 The offset is in pixels.                 
	 * 
     * @param hide
	 *           bool - whether the ad should keep being invisible after loaded,
	 *                  true for making the ad hidden, false for showing the 
	 *                  ad immediately. if the parameter is set to true, then 
	 *                  you need to programmatically display the ad by calling 
	 *                  ShowBannerAd() after you get notified with event
	 *                  OnReceiveAd from AdmobAdListener.
	 * 
	 * @param extras
	 *           Dictionary<string, string> - The extra parameters of Ad request.
	 * 
	 * @param tagForChildren
	 *           TagForChildrenDirectedTreatment - Set TagForChildrenDirectedTreatment flag,
	 *           More information, please refer to: 	
	 *           https://developers.google.com/mobile-ads-sdk/docs/admob/additional-controls#play-coppa
	 * 
	 *           Valid values:
	 * 
	 *	         Unset - Ad requests will include no indication of how you would like your content treated 
	 *                   with respect to COPPA.
	 * 
	 *           Yes - You will indicate that your content should be treated as child-directed for 
	 *                 purposes of COPPA.
	 * 
	 *           No - You will indicate that your content should NOT be treated as child-directed for 
	 *                purposes of COPPA.
	 * 
	 */
	[Obsolete("LoadBannerAd(BannerAdType adType, AdLayout layout, int offsetX, int offsetY, bool hide, Dictionary<string, string> extras, TagForChildrenDirectedTreatment tagForChildren) is deprecated, please call LoadBannerAd(BannerAdType adType, AdLayout layout, Vector2 offset, bool hide, Dictionary<string, string> extras, TagForChildrenDirectedTreatment tagForChildren) instead.")]
	public void LoadBannerAd(BannerAdType adType, AdLayout layout, int offsetX, int offsetY, bool hide, 
	                         Dictionary<string, string> extras, TagForChildrenDirectedTreatment tagForChildren)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().LoadBannerAd((int)adType, (int)layout, offsetX, offsetY, hide, extras, (int)tagForChildren);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().LoadBannerAd((int)adType, (int)layout, offsetX, offsetY, hide, extras, (int)tagForChildren);
#endif	
#if UNITY_WP8
		AdmobAdWP.Instance().LoadBannerAd((int)adType, (int)layout, offsetX, offsetY, hide, extras, (int)tagForChildren);
#endif			
	}
	
    /**
	 * Load a Banner Ad.
	 * 
	 * @param adType
	 *           BannerAdType - Type of the Ad.
	 * 
	 * @param layout
	 *           AdLayout - In which layout the Ad should display.
	 * 
	 * @param offset
	 *           Vector2 - The offset of the Ad.
	 * 
	 *           offset.x - The horizontal offset of the Ad, if the layout is set to left, 
	 *                 the offset is from the left edge of screen to the left edge of 
	 *                 the Ad; if the layout is set to right, the offset is from the 
	 *                 right edge of screen to the right edge of the Ad. 
	 *                 The offset is in pixels.	
	 *           offset.y - The vertical offset of the Ad, if the layout is set to top, 
	 *                 the offset is from the top edge of screen to the top edge of 
	 *                 the Ad; if the layout is set to bottom, the offset is from the 
	 *                 bottom edge of screen to the bottom edge of the Ad. 
	 *                 The offset is in pixels.                 
	 * 
     * @param hide
	 *           bool - whether the ad should keep being invisible after loaded,
	 *                  true for making the ad hidden, false for showing the 
	 *                  ad immediately. if the parameter is set to true, then 
	 *                  you need to programmatically display the ad by calling 
	 *                  ShowBannerAd() after you get notified with event
	 *                  OnReceiveAd from AdmobAdListener.
	 * 
	 * @param extras
	 *           Dictionary<string, string> - The extra parameters of Ad request.
	 * 
	 * @param tagForChildren
	 *           TagForChildrenDirectedTreatment - Set TagForChildrenDirectedTreatment flag,
	 *           More information, please refer to: 	
	 *           https://developers.google.com/mobile-ads-sdk/docs/admob/additional-controls#play-coppa
	 * 
	 *           Valid values:
	 * 
	 *	         Unset - Ad requests will include no indication of how you would like your content treated 
	 *                   with respect to COPPA.
	 * 
	 *           Yes - You will indicate that your content should be treated as child-directed for 
	 *                 purposes of COPPA.
	 * 
	 *           No - You will indicate that your content should NOT be treated as child-directed for 
	 *                purposes of COPPA.
	 * 
	 */
	public void LoadBannerAd(BannerAdType adType, AdLayout layout, Vector2 offset, bool hide, 
	                         Dictionary<string, string> extras, TagForChildrenDirectedTreatment tagForChildren)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().LoadBannerAd((int)adType, (int)layout, offset, hide, extras, (int)tagForChildren);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().LoadBannerAd((int)adType, (int)layout, offset, hide, extras, (int)tagForChildren);
#endif	
#if UNITY_WP8
		AdmobAdWP.Instance().LoadBannerAd((int)adType, (int)layout, offset, hide, extras, (int)tagForChildren);
#endif			
	}	
	
	/**
	 * Load a Banner Ad.
	 * 
	 * @param adType
	 *           BannerAdType - Type of the Ad.
	 * 
	 * @param layout
	 *           AdLayout - In which layout the Ad should display.
	 * 
	 * @param offsetX
	 *           int - The horizontal offset of the Ad, if the layout is set to left, 
	 *                 the offset is from the left edge of screen to the left edge of 
	 *                 the Ad; if the layout is set to right, the offset is from the 
	 *                 right edge of screen to the right edge of the Ad. 
	 *                 The offset is in pixels.
	 * 
	 * @param offsetY
	 *           int - The vertical offset of the Ad, if the layout is set to top, 
	 *                 the offset is from the top edge of screen to the top edge of 
	 *                 the Ad; if the layout is set to bottom, the offset is from the 
	 *                 bottom edge of screen to the bottom edge of the Ad. 
	 *                 The offset is in pixels.                 
	 * 
     * @param hide
	 *           bool - whether the ad should keep being invisible after loaded,
	 *                  true for making the ad hidden, false for showing the 
	 *                  ad immediately. if the parameter is set to true, then 
	 *                  you need to programmatically display the ad by calling 
	 *                  ShowBannerAd() after you get notified with event
	 *                  OnReceiveAd from AdmobAdListener.
	 * 
	 * @param extras
	 *           Dictionary<string, string> - The extra parameters of Ad request.
	 * 
	 */
	[Obsolete("LoadBannerAd(BannerAdType adType, AdLayout layout, int offsetX, int offsetY, bool hide, Dictionary<string, string> extras) is deprecated, please call LoadBannerAd(BannerAdType adType, AdLayout layout, Vector2 offset, bool hide, Dictionary<string, string> extras) instead.")]
	public void LoadBannerAd(BannerAdType adType, AdLayout layout, int offsetX, int offsetY, bool hide, 
	                         Dictionary<string, string> extras)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().LoadBannerAd((int)adType, (int)layout, offsetX, offsetY, hide, extras, 0);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().LoadBannerAd((int)adType, (int)layout, offsetX, offsetY, hide, extras, 0);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().LoadBannerAd((int)adType, (int)layout, offsetX, offsetY, hide, extras, 0);
#endif			
	}
	
	/**
	 * Load a Banner Ad.
	 * 
	 * @param adType
	 *           BannerAdType - Type of the Ad.
	 * 
	 * @param layout
	 *           AdLayout - In which layout the Ad should display.
	 * 
	 * @param offset
	 *           Vector2 - The offset of the Ad.
	 * 
	 *           offset.x - The horizontal offset of the Ad, if the layout is set to left, 
	 *                 the offset is from the left edge of screen to the left edge of 
	 *                 the Ad; if the layout is set to right, the offset is from the 
	 *                 right edge of screen to the right edge of the Ad. 
	 *                 The offset is in pixels.	
	 *           offset.y - The vertical offset of the Ad, if the layout is set to top, 
	 *                 the offset is from the top edge of screen to the top edge of 
	 *                 the Ad; if the layout is set to bottom, the offset is from the 
	 *                 bottom edge of screen to the bottom edge of the Ad. 
	 *                 The offset is in pixels.                  
	 * 
     * @param hide
	 *           bool - whether the ad should keep being invisible after loaded,
	 *                  true for making the ad hidden, false for showing the 
	 *                  ad immediately. if the parameter is set to true, then 
	 *                  you need to programmatically display the ad by calling 
	 *                  ShowBannerAd() after you get notified with event
	 *                  OnReceiveAd from AdmobAdListener.
	 * 
	 * @param extras
	 *           Dictionary<string, string> - The extra parameters of Ad request.
	 * 
	 */
	public void LoadBannerAd(BannerAdType adType, AdLayout layout, Vector2 offset, bool hide, 
	                         Dictionary<string, string> extras)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().LoadBannerAd((int)adType, (int)layout, offset, hide, extras, 0);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().LoadBannerAd((int)adType, (int)layout, offset, hide, extras, 0);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().LoadBannerAd((int)adType, (int)layout, offset, hide, extras, 0);
#endif			
	}
	
    /**
	 * Load a Banner Ad.
	 * 
	 * @param adSize
	 *           Vector2 - Customized size of the Ad.
	 *            
	 *           adSize.x - The width of the Ad, in pixels.
	 *           adSize.y - The height of the Ad, in pixels. 
	 * 
	 * @param layout
	 *           AdLayout - In which layout the Ad should display.	
	 */
	public void LoadBannerAd(Vector2 adSize, AdLayout layout)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().LoadBannerAd(adSize, (int)layout, Vector2.zero, false, null, 0);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().LoadBannerAd(adSize, (int)layout, Vector2.zero, false, null, 0);
#endif	
#if UNITY_WP8
		AdmobAdWP.Instance().LoadBannerAd(adSize, (int)layout, Vector2.zero, false, null, 0);
#endif			
	}	
	
    /**
	 * Load a Banner Ad.
	 * 
	 * @param adSize
	 *           Vector2 - Customized size of the Ad.
	 *            
	 *           adSize.x - The width of the Ad, in pixels.
	 *           adSize.y - The height of the Ad, in pixels. 
	 * 
	 * @param layout
	 *           AdLayout - In which layout the Ad should display.
	 * 
	 * @param offset
	 *           Vector2 - The offset of the Ad.
	 * 
	 *           offset.x - The horizontal offset of the Ad, if the layout is set to left, 
	 *                 the offset is from the left edge of screen to the left edge of 
	 *                 the Ad; if the layout is set to right, the offset is from the 
	 *                 right edge of screen to the right edge of the Ad. 
	 *                 The offset is in pixels.	
	 *           offset.y - The vertical offset of the Ad, if the layout is set to top, 
	 *                 the offset is from the top edge of screen to the top edge of 
	 *                 the Ad; if the layout is set to bottom, the offset is from the 
	 *                 bottom edge of screen to the bottom edge of the Ad. 
	 *                 The offset is in pixels.                 
	 * 
     * @param hide
	 *           bool - whether the ad should keep being invisible after loaded,
	 *                  true for making the ad hidden, false for showing the 
	 *                  ad immediately. if the parameter is set to true, then 
	 *                  you need to programmatically display the ad by calling 
	 *                  ShowBannerAd() after you get notified with event
	 *                  OnReceiveAd from AdmobAdListener.
	 * 
	 * @param extras
	 *           Dictionary<string, string> - The extra parameters of Ad request.
	 * 
	 * @param tagForChildren
	 *           TagForChildrenDirectedTreatment - Set TagForChildrenDirectedTreatment flag,
	 *           More information, please refer to: 	
	 *           https://developers.google.com/mobile-ads-sdk/docs/admob/additional-controls#play-coppa
	 * 
	 *           Valid values:
	 * 
	 *	         Unset - Ad requests will include no indication of how you would like your content treated 
	 *                   with respect to COPPA.
	 * 
	 *           Yes - You will indicate that your content should be treated as child-directed for 
	 *                 purposes of COPPA.
	 * 
	 *           No - You will indicate that your content should NOT be treated as child-directed for 
	 *                purposes of COPPA.
	 * 
	 */
	public void LoadBannerAd(Vector2 adSize, AdLayout layout, Vector2 offset, bool hide, 
	                         Dictionary<string, string> extras, TagForChildrenDirectedTreatment tagForChildren)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().LoadBannerAd(adSize, (int)layout, offset, hide, extras, (int)tagForChildren);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().LoadBannerAd(adSize, (int)layout, offset, hide, extras, (int)tagForChildren);
#endif	
#if UNITY_WP8
		AdmobAdWP.Instance().LoadBannerAd(adSize, (int)layout, offset, hide, extras, (int)tagForChildren);
#endif			
	}
	
	/**
	 * Load a Banner Ad and place it at specified absolute position, and 
	 * show it immediately once loaded.
	 * 
	 * Please note that the Ad won't display if the top or left applied
	 * makes the Ad display area outside the screen.
	 * 
	 * @param adType
	 * 			BannerAdType - the type of Ad.
	 * 
	 * @param top
	 * 			int - the top margin (in pixels) for placing Ad in absolute position.
	 * 
	 * @param left
	 * 			int - the left margin (in pixels) for placing Ad in absolute position. 
	 *	
	 */
	public void LoadBannerAd(BannerAdType adType, int top, int left)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().LoadBannerAd((int)adType, top, left, false, null, 0);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().LoadBannerAd((int)adType, top, left, false, null, 0);
#endif	
#if UNITY_WP8
		AdmobAdWP.Instance().LoadBannerAd((int)adType, top, left, false, null, 0);
#endif			
	}	
	
	/**
	 * Load a Banner Ad and place it at specified absolute position.
	 * 
	 * Please note that the Ad won't display if the top or left applied
	 * makes the Ad display area outside the screen.
	 * 
	 * @param adType
	 * 			BannerAdType - the type of Ad.
	 * 
	 * @param top
	 * 			int - the top margin (in pixels) for placing Ad in absolute position.
	 * 
	 * @param left
	 * 			int - the left margin (in pixels) for placing Ad in absolute position. 
	 * 
	 * @param hide
	 *           bool - whether the ad should keep being invisible after loaded,
	 *                  true for making the ad hidden, false for showing the 
	 *                  ad immediately. if the parameter is set to true, then 
	 *                  you need to programmatically display the ad by calling 
	 *                  ShowBannerAd() after you get notified with event
	 *                  OnReceiveAd from AdmobAdListener.
	 */
	public void LoadBannerAd(BannerAdType adType, int top, int left, bool hide)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().LoadBannerAd((int)adType, top, left, hide, null, 0);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().LoadBannerAd((int)adType, top, left, hide, null, 0);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().LoadBannerAd((int)adType, top, left, hide, null, 0);
#endif		
	}
	
	/**
	 * Load a Banner Ad and place it at specified absolute position.
	 * 
	 * Please note that the Ad won't display if the top or left applied
	 * makes the Ad display area outside the screen.
	 * 
	 * @param adType
	 * 			BannerAdType - the type of Ad.
	 * 
	 * @param top
	 * 			int - the top margin (in pixels) for placing Ad in absolute position.
	 * 
	 * @param left
	 * 			int - the left margin (in pixels) for placing Ad in absolute position. 
	 * 
	 * @param hide
	 *           bool - whether the ad should keep being invisible after loaded,
	 *                  true for making the ad hidden, false for showing the 
	 *                  ad immediately. if the parameter is set to true, then 
	 *                  you need to programmatically display the ad by calling 
	 *                  ShowBannerAd() after you get notified with event
	 *                  OnReceiveAd from AdmobAdListener.
	 * 
	 * @param extras
	 *           Dictionary<string, string> - The extra parameters of Ad request.
	 * 
	 * @param tagForChildren
	 *           TagForChildrenDirectedTreatment - Set TagForChildrenDirectedTreatment flag,
	 *           More information, please refer to: 	
	 *           https://developers.google.com/mobile-ads-sdk/docs/admob/additional-controls#play-coppa
	 * 
	 *           Valid values:
	 * 
	 *	         Unset - Ad requests will include no indication of how you would like your content treated 
	 *                   with respect to COPPA.
	 * 
	 *           Yes - You will indicate that your content should be treated as child-directed for 
	 *                 purposes of COPPA.
	 * 
	 *           No - You will indicate that your content should NOT be treated as child-directed for 
	 *                purposes of COPPA.
	 * 
	 */
	public void LoadBannerAd(BannerAdType adType, int top, int left, bool hide, 
	                         Dictionary<string, string> extras, 
	                         TagForChildrenDirectedTreatment tagForChildren)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().LoadBannerAd((int)adType, top, left, hide, extras, (int)tagForChildren);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().LoadBannerAd((int)adType, top, left, hide, extras, (int)tagForChildren);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().LoadBannerAd((int)adType, top, left, hide, extras, (int)tagForChildren);
#endif			
	}	
	
	/**
	 * Refresh the Banner Ad.
	 * 
	 * This initiates a new ad request to plugin.
	 */
	public void RefreshBannerAd()
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().RefreshBannerAd();
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().RefreshBannerAd();
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().RefreshBannerAd();
#endif		
	}
	
	/**
	 * Show the Banner Ad.
	 * 
	 * This sets the banner ad to be visible.
	 */
	public void ShowBannerAd()
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().ShowBannerAd();
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().ShowBannerAd();
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().ShowBannerAd();
#endif		
	}
	
	/**
	 * Hide the Banner Ad.
	 * 
	 * This sets the banner ad to be invisible again.
	 */
	public void HideBannerAd()
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().HideBannerAd();
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().HideBannerAd();
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().HideBannerAd();
#endif		
	}	
	
	/**
	 * Reposition the Banner Ad.
	 * 
	 * @param layout
	 *           AdLayout - In which layout the Ad should display.	
	 */
	public void RepositionBannerAd(AdLayout layout)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().RepositionBannerAd((int)layout, 0, 0);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().RepositionBannerAd((int)layout, 0, 0);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().RepositionBannerAd((int)layout, 0, 0);
#endif		
	}	
	
	/**
	 * Reposition the Banner Ad.
	 * 
	 * @param layout
	 *           AdLayout - In which layout the Ad should display.
	 * 
	 * @param offsetX
	 *           int - The horizontal offset of the Ad, if the layout is set to left, 
	 *                 the offset is from the left edge of screen to the left edge of 
	 *                 the Ad; if the layout is set to right, the offset is from the 
	 *                 right edge of screen to the right edge of the Ad. 
	 *                 The offset is in pixels.
	 * 
	 * @param offsetY
	 *           int - The vertical offset of the Ad, if the layout is set to top, 
	 *                 the offset is from the top edge of screen to the top edge of 
	 *                 the Ad; if the layout is set to bottom, the offset is from the 
	 *                 bottom edge of screen to the bottom edge of the Ad. 
	 *                 The offset is in pixels. 
	 */
	public void RepositionBannerAd(AdLayout layout, int offsetX, int offsetY)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().RepositionBannerAd((int)layout, offsetX, offsetY);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().RepositionBannerAd((int)layout, offsetX, offsetY);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().RepositionBannerAd((int)layout, offsetX, offsetY);
#endif		
	}	
	
	/**
	 * Destroy the Banner Ad.
	 */
	public void DestroyBannerAd()
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().DestroyBannerAd();
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().DestroyBannerAd();
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().DestroyBannerAd();
#endif		
	}
	
	/**
	 * Load an Interstitial Ad and show it immediately once loaded. 	 
	 */
	public void LoadInterstitialAd()
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().LoadInterstitialAd(false, null, 0);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().LoadInterstitialAd(false, null, 0);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().LoadInterstitialAd(false, null, 0);
#endif		
	}	
	
	/**
	 * Load an Interstitial Ad.	 
	 * 
	 * @param hide
	 *           bool - whether the ad should keep being invisible after loaded,
	 *                  true for making the ad hidden, false for showing the 
	 *                  ad immediately. if the parameter is set to true, then 
	 *                  you need to programmatically display the ad by calling 
	 *                  ShowInterstitialAd() after you get notified with event
	 *                  OnReceiveAdInterstitial from AdmobAdListener.
	 */
	public void LoadInterstitialAd(bool hide)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().LoadInterstitialAd(hide, null, 0);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().LoadInterstitialAd(hide, null, 0);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().LoadInterstitialAd(hide, null, 0);
#endif		
	}
	
	/**
	 * Load an Interstitial Ad.	 
	 * 
	 * @param hide
	 *           bool - whether the ad should keep being invisible after loaded,
	 *                  true for making the ad hidden, false for showing the 
	 *                  ad immediately. if the parameter is set to true, then 
	 *                  you need to programmatically display the ad by calling 
	 *                  ShowInterstitialAd() after you get notified with event
	 *                  OnReceiveAdInterstitial from AdmobAdListener.
	 * 
	 * @param extras
	 *           Dictionary<string, string> - The extra parameters of Ad request.
	 */
	public void LoadInterstitialAd(bool hide, Dictionary<string, string> extras, 
	                               TagForChildrenDirectedTreatment tagForChildren)
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().LoadInterstitialAd(hide, extras, (int)tagForChildren);
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().LoadInterstitialAd(hide, extras, (int)tagForChildren);
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().LoadInterstitialAd(hide, extras, (int)tagForChildren);
#endif		
	}	
	
	/**
	 * Show the Interstitial Ad.
	 * 
	 * This sets the Interstitial ad to be visible.
	 */
	public void ShowInterstitialAd()
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().ShowInterstitialAd();
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().ShowInterstitialAd();
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().ShowInterstitialAd();
#endif		
	}
	
	/**
	 * Disable all Ads (Banners and Interstitials).
	 * 
	 * This function is very useful when you want to disable the Ads after
	 * the user makes an In-App Purchase. In this case, you should call
	 * this function in OnPurchaseCompleted() event handler of an IAP
	 * plugin. 
	 * 
	 * This function also persists the state on the user's device so you
	 * don't need to set any variable in the PlayerPrefs.	
	 */
	public void DisableAd()
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().DisableAd();
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().DisableAd();
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().DisableAd();
#endif		
	}
	
	/**
	 * Enable Ads (Banners and Interstitials).
	 * 
	 * This function is for re-enabling the Ads after you called DisableAd().
	 * After calling this function, you will be able to call LoadBannerAd() or
	 * LoadInterstitialAd() then. And if the "Auto Load" switch is turned on, the
	 * Ad will continue to be served then.
	 * Persistent state is also taken care of, you don't need to set any variables
	 * in the PlayerPrefs either.	
	 */
	public void EnableAd()
	{
#if UNITY_ANDROID		
		AdmobAdAndroid.Instance().EnableAd();
#endif	
#if UNITY_IPHONE
		AdmobAdIOS.Instance().EnableAd();
#endif
#if UNITY_WP8
		AdmobAdWP.Instance().EnableAd();
#endif		
	}
	
	/**
	 * Check if Ad is enabled or not.
	 * 
	 * This function is for getting the current state of the Ad.
	 * 
	 * @result bool - True if the Ad is enabled, false if the Ad has 
	 *                been disabled by calling DisableAd().
	 */
	public bool IsAdEnabled()
	{
		bool result = true;
#if UNITY_ANDROID		
		result = AdmobAdAndroid.Instance().IsAdEnabled();
#endif	
#if UNITY_IPHONE
		result = AdmobAdIOS.Instance().IsAdEnabled();
#endif
#if UNITY_WP8
		result = AdmobAdWP.Instance().IsAdEnabled();
#endif		
		return result;
	}	

	/**
	 * Get the size of Banner Ad in pixels.
	 * 
	 * This function is for getting the banner's actual size in pixels.
	 * 
	 * @result Vector2 - The width and height, x for width, y for height.
	 */
	public Vector2 GetAdSizeInPixels(BannerAdType adType)
	{
		Vector2 result = Vector2.zero;
#if UNITY_ANDROID		
		result = AdmobAdAndroid.Instance().GetAdSizeInPixels((int)adType);
#endif	
#if UNITY_IPHONE
		result = AdmobAdIOS.Instance().GetAdSizeInPixels((int)adType);
#endif
#if UNITY_WP8
		result = AdmobAdWP.Instance().GetAdSizeInPixels((int)adType);
#endif		
		return result;
	}

	#endregion
}
