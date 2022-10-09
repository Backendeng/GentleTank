using UnityEngine;
using System.Collections;
using ChartboostSDK;
using GlobalInfo;

public class EventHandler : MonoBehaviour {
	private bool levelUpFlag = false;
	private Transform sightCamPos;
	private Transform normalCamPos;
	private Transform cannonSPPos;
	private Transform mainPlayer;
	private Vector3 crosshair_Pos;
	private Vector3 auto_Target;
	private float shot_Time_Counter;
	private float aimingUIAnimTime = 5f;
	private int aimingUIIndex = 0;
	private bool aimingUIInverseFlag = false;
	private bool damagedAction = false;
	private int originEnemyCount = 0;

	//Touch Input
	public TouchController myTouchController;
	//UI 
	public UIButton camChangeButton;
	public UIButton specialCamButton;
	public UIButton resumeButton;
	public UIButton patriotButton;
	public UISlider healthBar;
	public UISlider enemyProgress;
	public UISlider magnifySlider;
	public UISprite telescopeSprite;
	public UISprite autoAimSprite;
	public UILabel gameScore;
	public UISprite disabledShotSprite;
	public Texture2D[] normal_crossHair;
	public Texture2D locked_crossHair;
	public Texture2D auto_crossHair;
	public Texture2D enemy_Locked;
	public Transform patriotCamPos;
	public Transform patriotRotCamPos;
	public GameObject patriotOffset;
	public GameObject uiPanel;
	public GameObject pausePanel;
	public GameObject briefingPanel;
	public GameObject clearPanel;
	public GameObject finalClearPanel;
	public GameObject failPanel;
	public GameObject[] ui_briefs;
	public GameObject[] mission_Labels;

	public float shot_Time_Peroid = 5f;
	public float touchInterpolation = 5f;
	public GameObject specialCam;
	public GameObject distanceLabel;
	public GameObject bloodSprite;
	public GameObject noiseSprite;
	public GameObject noiseBloodSprite;
	public GameObject miniMap;
	public GameObject autoAim_But;
	public GameObject scope_But;
	public GameObject patriot_But;
	public GameObject miniMapButton;

	//Mission Objects
	public GameObject[] mission_Levels;
	public GameObject[] mission3_Hidden_Objects;
	public GameObject[] mission4_Hidden_Objects;
	public GameObject[] mission5_Hidden_Objects;
	public Transform[] initPos;
	public GoogleAnalyticsV3 googleAnalytics;
	Vector2 temp = Vector2.zero;
	// Use this for initialization
	void Awake(){
		mainPlayer = GameObject.Find ("MainPlayer").transform;
		mainPlayer.transform.position = initPos [GlobalInfo.MainGameInfo.currentMission].position;
		mainPlayer.transform.rotation = initPos [GlobalInfo.MainGameInfo.currentMission].rotation;

		sightCamPos = GameObject.Find ("MainPlayer/Head/Cannon/SightCamPos").transform;
		normalCamPos = GameObject.Find ("MainPlayer/Pos/NormalCamPos").transform;
		cannonSPPos = GameObject.Find ("MainPlayer/Head/Cannon/CannonSP").transform;
	}
	void Start () {
        MainGameInfo.levelCompleted = false;
        MainGameInfo.levelFailed = false;
		if (MainGameInfo.currentMission >= 5) {
			patriot_But.SetActive(true);
		}
		levelUpFlag = false;
		if (GlobalInfo.MainGameInfo.soundFlag) {
			AudioListener.volume = 1f;
		}else{
			AudioListener.volume = 0;
		}

		GlobalInfo.MainGameInfo.pauseFlag = false;
		GlobalInfo.MainGameInfo.crossHair_Pos = new Vector2 (Screen.width * 0.5f, 128f);
		GlobalInfo.MainGameInfo.autoAim = false;
		GlobalInfo.MainGameInfo.specialCam = false;
		GlobalInfo.MainGameInfo.score = 0;
		GlobalInfo.MainGameInfo.health = 500f;

		Application.targetFrameRate = 60;
		googleAnalytics.LogScreen ("Tank_" + GlobalInfo.MainGameInfo.currentMission.ToString ());
		StartCoroutine (GameStart ());
	}

	IEnumerator GameStart(){
		mission_Levels [GlobalInfo.MainGameInfo.currentMission].SetActive (true);
		mission_Labels [GlobalInfo.MainGameInfo.currentMission].SetActive (true);
		mission_Labels [GlobalInfo.MainGameInfo.currentMission].AddComponent<TweenAlpha> ();
		mission_Labels [GlobalInfo.MainGameInfo.currentMission].GetComponent<TweenAlpha> ().from = 1;
		mission_Labels [GlobalInfo.MainGameInfo.currentMission].GetComponent<TweenAlpha> ().to = 0;
		mission_Labels [GlobalInfo.MainGameInfo.currentMission].GetComponent<TweenAlpha> ().duration = 5f;
		originEnemyCount = mission_Levels [GlobalInfo.MainGameInfo.currentMission].transform.childCount;
		GlobalInfo.MainGameInfo.enemyProgress = originEnemyCount;
		if (GlobalInfo.MainGameInfo.currentMission == 2) {
			yield return new WaitForSeconds(300f);
			foreach(GameObject gb in mission3_Hidden_Objects){
				gb.SetActive(true);
			}
		}
		if (GlobalInfo.MainGameInfo.currentMission == 3) {
			yield return new WaitForSeconds(350f);
			foreach(GameObject gb in mission4_Hidden_Objects){
				gb.SetActive(true);
			}
		}
		if (GlobalInfo.MainGameInfo.currentMission == 4) {
			yield return new WaitForSeconds(450f);
			foreach(GameObject gb in mission5_Hidden_Objects){
				gb.SetActive(true);
			}
		}
		yield return new WaitForEndOfFrame ();
	}
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (MainGameInfo.levelCompleted || MainGameInfo.levelFailed)
            {

            }
            else
            {
                if (!MainGameInfo.pauseFlag)
                {
                    OnPauseButtonPressed();
                }
                else
                {
                    OnResumeButtonPressed();
                }   
            }   
        }
		if (GlobalInfo.MainGameInfo.pauseFlag) {
			return;
		}
		if (GlobalInfo.MainGameInfo.specialCam && GlobalInfo.MainGameInfo.autoAim) {
			magnifySlider.value = 0;
			magnifySlider.gameObject.SetActive(false);
			distanceLabel.gameObject.SetActive(false);
		}
		if (mission_Levels [GlobalInfo.MainGameInfo.currentMission].transform.childCount == 0) {
			LevelClear();
		}
		if (GlobalInfo.MainGameInfo.health < 0) {
			GameOver();
		}

		GlobalInfo.MainGameInfo.spd = myTouchController.GetAxis ("Vertical");
		GlobalInfo.MainGameInfo.str = myTouchController.GetAxis ("Horizontal");

		GlobalInfo.MainGameInfo.tapFlag = myTouchController.GetButtonDown ("Fire1");
		/*
		if (GlobalInfo.MainGameInfo.tapFlag && !GlobalInfo.MainGameInfo.autoAim) {
			Vector2 temp = myTouchController.GetMousePos();
			if(temp.y > Screen.height * 0.4f){
				GlobalInfo.MainGameInfo.crossHair_Pos = new Vector2(temp.x,Screen.height - temp.y);
			}
		}*/

		//Calculate Touch Interpolation
		float xVal = myTouchController.GetAxis("Mouse X") * touchInterpolation * Time.deltaTime;
		float yVal = myTouchController.GetAxis("Mouse Y") * touchInterpolation * -Time.deltaTime;
		//Debug.Log (xVal);
		if (MainGameInfo.patriotFlag) {
			patriotRotCamPos.RotateAroundLocal(Vector3.up,xVal * 0.01f);
		}

		if(!GlobalInfo.MainGameInfo.autoAim){
			//Manual Aim Control
			aimingUIAnimTime += Time.deltaTime;
			temp = GlobalInfo.MainGameInfo.crossHair_Pos;
			temp += new Vector2 (xVal, yVal);
			temp.x = Mathf.Clamp (temp.x, 0, Screen.width);
			temp.y = Mathf.Clamp (temp.y, 0, Screen.height);
			GlobalInfo.MainGameInfo.crossHair_Pos = temp;
		}else{
			//When Auto Aim Control
			float dotVal = 0;
			foreach(Transform tf in mission_Levels[GlobalInfo.MainGameInfo.currentMission].transform){
				dotVal = Vector3.Dot(Camera.main.transform.forward,(tf.position - Camera.main.transform.position).normalized);
				if(dotVal > 0.8f){
					auto_Target = tf.position + new Vector3(0,1f,0);
					break;
				}
			}

			Vector3 temp = Camera.main.WorldToScreenPoint(auto_Target);
			temp = new Vector3(temp.x,Screen.height - temp.y,temp.z);
			if(temp.x < 0 || temp.x > Screen.width || temp.y < 0 || temp.y > Screen.height){
				temp = new Vector2 (Screen.width * 0.5f, 128f);
			}
			GlobalInfo.MainGameInfo.crossHair_Pos = Vector2.Lerp(GlobalInfo.MainGameInfo.crossHair_Pos,temp,Time.deltaTime);
		}
		if (GlobalInfo.MainGameInfo.specialCam && !GlobalInfo.MainGameInfo.autoAim) {
			//When In Special Cam State
			distanceLabel.GetComponent<UILabel>().text = GlobalInfo.MainGameInfo.distanceToTarget.ToString() + "M";
			GlobalInfo.MainGameInfo.crossHair_Pos = new Vector2(Screen.width * 0.5f,Screen.height * 0.5f);
			mainPlayer.RotateAroundLocal(Vector3.up,xVal * 0.003f);

			Vector2 tpPos = myTouchController.GetMousePos();
			if(tpPos.x > Screen.width * 0.65f && tpPos.x < Screen.width * 0.87f){
				yVal = 0;
			}
			Transform tf = Camera.main.transform.parent.parent;
			if(yVal > 0){
				if((yVal * 0.003f * Mathf.Rad2Deg + tf.localRotation.x * Mathf.Rad2Deg) > 5f){
					yVal = 0;
				}else{
					tf.RotateAroundLocal(Vector3.right,yVal * 0.003f);
				}
			}else if(yVal < 0){
				if((yVal * 0.003f * Mathf.Rad2Deg + tf.localRotation.x * Mathf.Rad2Deg) < -5f){
					yVal = 0;
				}else{
					tf.RotateAroundLocal(Vector3.right,yVal * 0.003f);
				}
			}



			//Zoom In or Out
			float zoomVal = magnifySlider.value;
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView,60f - zoomVal * 40f,Time.deltaTime);

		}

		//Display UI value
		gameScore.text = GlobalInfo.MainGameInfo.score.ToString();
		healthBar.value = GlobalInfo.MainGameInfo.health / 500f;

		int tp = MainGameInfo.currentMission;
		tp = Mathf.Clamp(tp,0,6);
		int count = mission_Levels [tp].transform.childCount;
		enemyProgress.value = ((float)count) / ((float)originEnemyCount);
		if (!GlobalInfo.MainGameInfo.readyToShotFlag) {
			disabledShotSprite.fillAmount -= Time.deltaTime * 0.25f;
		}else{
			disabledShotSprite.fillAmount = 0;
		}

		shot_Time_Counter += Time.deltaTime;
		if (shot_Time_Counter > shot_Time_Peroid) {
			GlobalInfo.MainGameInfo.readyToShotFlag = true;
		}


		#if UNITY_ANDROID
		Touch[] touches = Input.touches;
		foreach(Touch touch in touches){
			if(touch.phase == TouchPhase.Began){
				if (touch.position.x > Screen.width * 0.8f && touch.position.x < Screen.width && touch.position.y > Screen.height * 0.47f && touch.position.y < Screen.height * 0.87f) {
					miniMap.SetActive(false);
					GlobalInfo.MainGameInfo.miniMapFlag = false;
					miniMapButton.SetActive (true);
				}
			}
		}
		#endif
	}

	//When Game Over
	void GameOver(){
        MainGameInfo.levelFailed = true;
        AdmobAd.Instance().LoadInterstitialAd(false);
		Camera.main.transform.parent = null;
		GlobalInfo.MainGameInfo.pauseFlag = true;
		Time.timeScale = 0;
		miniMap.SetActive(false);
		GlobalInfo.MainGameInfo.miniMapFlag = false;
		myTouchController.gameObject.SetActive (false);
		uiPanel.SetActive (false);
		Camera.main.GetComponent<RadarSystem> ().enabled = false;
		mainPlayer.gameObject.SetActive (false);
		failPanel.SetActive (true);
	}

	//When Level Clear
	void LevelClear(){
        MainGameInfo.levelCompleted = true;
        AdmobAd.Instance().LoadInterstitialAd(false);
		Camera.main.transform.parent = null;
		levelUpFlag = true;
		GlobalInfo.MainGameInfo.pauseFlag = true;
		Time.timeScale = 0;
		miniMap.SetActive(false);
		GlobalInfo.MainGameInfo.miniMapFlag = false;
		myTouchController.gameObject.SetActive (false);
		uiPanel.SetActive (false);
		Camera.main.GetComponent<RadarSystem> ().enabled = false;
		mainPlayer.gameObject.SetActive (false);
		if(MainGameInfo.currentMission < 6){
			clearPanel.SetActive (true);
		}else{
			finalClearPanel.SetActive(true);
		}
		if(GlobalInfo.MainGameInfo.currentMission < 7){
			GlobalInfo.MainGameInfo.currentMission ++;
		}
		int tp = PlayerPrefs.GetInt ("MaxLevel");
		if(tp < GlobalInfo.MainGameInfo.currentMission){
			PlayerPrefs.SetInt ("MaxLevel", GlobalInfo.MainGameInfo.currentMission);
		}

	}

	public void OnCameraChange(){
		GlobalInfo.MainGameInfo.autoAim = !GlobalInfo.MainGameInfo.autoAim;
		if (GlobalInfo.MainGameInfo.autoAim) {
			if(!GlobalInfo.MainGameInfo.specialCam){
				autoAimSprite.gameObject.AddComponent<TweenAlpha>();
				autoAimSprite.gameObject.GetComponent<TweenAlpha>().from = 0.5f;
				autoAimSprite.gameObject.GetComponent<TweenAlpha>().to = 1f;
				autoAimSprite.gameObject.GetComponent<TweenAlpha>().duration = 0.5f;
				autoAimSprite.gameObject.GetComponent<TweenAlpha>().style = UITweener.Style.PingPong;
				camChangeButton.normalSprite = "JoJin_2";
			}else{
				magnifySlider.gameObject.SetActive(false);
				autoAimSprite.gameObject.AddComponent<TweenAlpha>();
				autoAimSprite.gameObject.GetComponent<TweenAlpha>().from = 0.5f;
				autoAimSprite.gameObject.GetComponent<TweenAlpha>().to = 1f;
				autoAimSprite.gameObject.GetComponent<TweenAlpha>().duration = 0.5f;
				autoAimSprite.gameObject.GetComponent<TweenAlpha>().style = UITweener.Style.PingPong;
				camChangeButton.normalSprite = "JoJin_2";
				OnEqualState();
			}
		}else{
			if(!GlobalInfo.MainGameInfo.specialCam){
				if(autoAimSprite.GetComponent<TweenAlpha>() != null){
					Destroy (autoAimSprite.GetComponent<TweenAlpha>());
				}
				GlobalInfo.MainGameInfo.crossHair_Pos = new Vector2 (Screen.width * 0.5f, 128f);
				camChangeButton.normalSprite = "JoJin_1";
			}else{
				if(autoAimSprite.GetComponent<TweenAlpha>() != null){
					Destroy (autoAimSprite.GetComponent<TweenAlpha>());
				}
				magnifySlider.gameObject.SetActive(true);
				distanceLabel.gameObject.SetActive(true);
				Camera.main.GetComponent<SmoothFollow>().enabled = false;
				Camera.main.transform.parent = sightCamPos;
				Camera.main.transform.localPosition = Vector3.zero;
				Camera.main.transform.localRotation = Quaternion.identity;
				mainPlayer.Find("Head/Cannon").transform.localRotation = Quaternion.identity;
				mainPlayer.Find("Head").transform.localRotation = Quaternion.identity;
				GlobalInfo.MainGameInfo.crossHair_Pos = new Vector2 (Screen.width * 0.5f, 128f);
				camChangeButton.normalSprite = "JoJin_1";
			}
		}
	}

	public void OnMiniMapDisplay(){
		miniMap.SetActive (true);
		GlobalInfo.MainGameInfo.miniMapFlag = true;
		miniMapButton.SetActive (false);
	}

	public void OnSpecialCam(){
		GlobalInfo.MainGameInfo.specialCam = !GlobalInfo.MainGameInfo.specialCam;
		if (GlobalInfo.MainGameInfo.specialCam) {
			if(!GlobalInfo.MainGameInfo.autoAim){

				telescopeSprite.gameObject.AddComponent<TweenAlpha>();
				telescopeSprite.gameObject.GetComponent<TweenAlpha>().from = 0.5f;
				telescopeSprite.gameObject.GetComponent<TweenAlpha>().to = 1f;
				telescopeSprite.gameObject.GetComponent<TweenAlpha>().duration = 0.5f;
				telescopeSprite.gameObject.GetComponent<TweenAlpha>().style = UITweener.Style.PingPong;
				mainPlayer.Find("Head/Cannon").transform.localRotation = Quaternion.identity;
				mainPlayer.Find("Shadow").gameObject.SetActive(false);
				magnifySlider.gameObject.SetActive(true);
				Camera.main.GetComponent<SmoothFollow>().enabled = false;
				Camera.main.transform.position = sightCamPos.position;
				Camera.main.transform.rotation = sightCamPos.rotation;
				Camera.main.transform.parent = sightCamPos;
				specialCamButton.normalSprite = "G_2";
				specialCam.SetActive(true);
				distanceLabel.SetActive(true);
			}else{
				magnifySlider.gameObject.SetActive(false);
				mainPlayer.Find("Head/Cannon").transform.localRotation = Quaternion.identity;
				mainPlayer.Find("Head").transform.localRotation = Quaternion.identity;
				mainPlayer.Find("Shadow").gameObject.SetActive(false);
				Camera.main.GetComponent<SmoothFollow>().enabled = false;
				Camera.main.transform.parent = sightCamPos;
				Camera.main.transform.localPosition = Vector3.zero;
				Camera.main.transform.localRotation = Quaternion.identity;
				specialCamButton.normalSprite = "G_2";
				specialCam.SetActive(true);
				telescopeSprite.gameObject.AddComponent<TweenAlpha>();
				telescopeSprite.gameObject.GetComponent<TweenAlpha>().from = 0.5f;
				telescopeSprite.gameObject.GetComponent<TweenAlpha>().to = 1f;
				telescopeSprite.gameObject.GetComponent<TweenAlpha>().duration = 0.5f;
				telescopeSprite.gameObject.GetComponent<TweenAlpha>().style = UITweener.Style.PingPong;

			}
		}else{
			if(!GlobalInfo.MainGameInfo.autoAim){
				if(telescopeSprite.gameObject.GetComponent<TweenAlpha>() != null){
					Destroy(telescopeSprite.gameObject.GetComponent<TweenAlpha>());
				}
				mainPlayer.Find("Shadow").gameObject.SetActive(true);
				Camera.main.fieldOfView = 60f;
				magnifySlider.gameObject.SetActive(false);
				GlobalInfo.MainGameInfo.crossHair_Pos = new Vector2 (Screen.width * 0.5f, 128f);
				Camera.main.GetComponent<SmoothFollow>().enabled = true;
				Camera.main.transform.position = normalCamPos.position;
				Camera.main.transform.rotation = normalCamPos.rotation;
				Camera.main.transform.parent = null;
				specialCamButton.normalSprite = "G_1";
				specialCam.SetActive(false);
				distanceLabel.SetActive(false);
			}else{
				Camera.main.transform.parent = null;
				Camera.main.GetComponent<SmoothFollow>().enabled = true;
				if(telescopeSprite.gameObject.GetComponent<TweenAlpha>() != null){
					Destroy(telescopeSprite.gameObject.GetComponent<TweenAlpha>());
				}
				mainPlayer.Find("Shadow").gameObject.SetActive(true);
				Camera.main.fieldOfView = 60f;
				magnifySlider.gameObject.SetActive(false);
				specialCamButton.normalSprite = "G_1";
				specialCam.SetActive(false);
				distanceLabel.SetActive(false);
			}
		}


	}

	void OnGUI(){
		if (GlobalInfo.MainGameInfo.pauseFlag || MainGameInfo.patriotFlag) {
			return;
		}
		if (GlobalInfo.MainGameInfo.specialCam && !GlobalInfo.MainGameInfo.autoAim) {
			return;
		}
		Rect rt = new Rect (GlobalInfo.MainGameInfo.crossHair_Pos.x - 64f, GlobalInfo.MainGameInfo.crossHair_Pos.y - 64f, 128f, 128f);
		if(GlobalInfo.MainGameInfo.aimReady){
			GUI.DrawTexture (rt, locked_crossHair);
		}else{
			if(GlobalInfo.MainGameInfo.autoAim){
				GUI.DrawTexture(rt,auto_crossHair);
			}else{
				//Aim Crosshair Animation
				GUI.DrawTexture(rt,normal_crossHair[aimingUIIndex]);
				if(aimingUIAnimTime >= 0.1f){
					aimingUIAnimTime = 0;
					if(!aimingUIInverseFlag){
						aimingUIIndex ++;
					}else{
						aimingUIIndex --;
					}
					if(aimingUIIndex > 8){
						aimingUIInverseFlag = !aimingUIInverseFlag;
						aimingUIIndex = 8;
					}
					if(aimingUIIndex < 0){
						aimingUIIndex = 0;
						aimingUIInverseFlag = !aimingUIInverseFlag;
					}
				}
			}
		}
	}

	public void OnShot(){

		if(GlobalInfo.MainGameInfo.readyToShotFlag){
			shot_Time_Counter = 0;
			GlobalInfo.MainGameInfo.readyToShotFlag = false;
			disabledShotSprite.fillAmount = 1f;
			mainPlayer.BroadcastMessage ("OnShot", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void OnDamage(){
		if (!damagedAction) {
			damagedAction = true;
//			Camera.main.GetComponent<MotionBlur>().enabled = true;
			StartCoroutine(DamageAction());
		}
	}

	
	IEnumerator DamageAction(){
		bloodSprite.SetActive (true);
		noiseSprite.SetActive (true);
		noiseBloodSprite.SetActive (true);
		bloodSprite.AddComponent<TweenAlpha> ();
		bloodSprite.GetComponent<TweenAlpha> ().from = 0.7f;
		bloodSprite.GetComponent<TweenAlpha> ().to = 0;
		bloodSprite.GetComponent<TweenAlpha> ().duration = 1.8f;

		noiseSprite.AddComponent<TweenAlpha> ();
		noiseSprite.GetComponent<TweenAlpha> ().from = 0.7f;
		noiseSprite.GetComponent<TweenAlpha> ().to = 0;
		noiseSprite.GetComponent<TweenAlpha> ().duration = 1.8f;

		noiseBloodSprite.AddComponent<TweenAlpha> ();
		noiseBloodSprite.GetComponent<TweenAlpha> ().from = 0.7f;
		noiseBloodSprite.GetComponent<TweenAlpha> ().to = 0;
		noiseBloodSprite.GetComponent<TweenAlpha> ().duration = 1.8f;

		yield return new WaitForSeconds (2f);
		Destroy(bloodSprite.GetComponent<TweenAlpha>());
		Destroy (noiseBloodSprite.GetComponent<TweenAlpha> ());
		Destroy (noiseSprite.GetComponent<TweenAlpha> ());
		bloodSprite.SetActive (false);
		noiseSprite.SetActive (false);
		noiseBloodSprite.SetActive (false);
//		Camera.main.GetComponent<MotionBlur>().enabled = false;
		yield return new WaitForEndOfFrame ();
		damagedAction = false;
	}

	//When user tap pause button in main game ui
	public void OnPauseButtonPressed(){
		AdmobAd.Instance().LoadInterstitialAd(false);		
		if (GlobalInfo.MainGameInfo.specialCam) {
//			GlobalInfo.MainGameInfo.specialCam = false;
			Camera.main.SendMessage("DisableNoise",SendMessageOptions.DontRequireReceiver);
			if(telescopeSprite.gameObject.GetComponent<TweenAlpha>() != null){
				Destroy(telescopeSprite.gameObject.GetComponent<TweenAlpha>());
			}
			mainPlayer.Find("Shadow").gameObject.SetActive(true);
			Camera.main.fieldOfView = 60f;
			magnifySlider.gameObject.SetActive(false);
			camChangeButton.gameObject.GetComponent<BoxCollider>().enabled = true;
			GlobalInfo.MainGameInfo.crossHair_Pos = new Vector2 (Screen.width * 0.5f, 128f);
			Camera.main.GetComponent<SmoothFollow>().enabled = true;
			Camera.main.transform.position = normalCamPos.position;
			Camera.main.transform.rotation = normalCamPos.rotation;
			Camera.main.transform.parent = null;
			specialCamButton.normalSprite = "G_1";
			specialCam.SetActive(false);
			distanceLabel.SetActive(false);
		}

		GlobalInfo.MainGameInfo.pauseFlag = true;
		AudioListener.volume = 0;
		Time.timeScale = 0;
		miniMap.SetActive(false);
		GlobalInfo.MainGameInfo.miniMapFlag = false;
		myTouchController.gameObject.SetActive (false);
		uiPanel.SetActive (false);
		pausePanel.SetActive (true);
		Camera.main.GetComponent<RadarSystem> ().enabled = false;
//		mainPlayer.gameObject.SetActive (false);

	}

	public void OnReplayButtonPressed(){
		int tp = PlayerPrefs.GetInt ("MaxLevel");
		if(tp < GlobalInfo.MainGameInfo.currentMission){
			PlayerPrefs.SetInt ("MaxLevel", GlobalInfo.MainGameInfo.currentMission);
		}	
		if (levelUpFlag && GlobalInfo.MainGameInfo.currentMission != 7) {
			GlobalInfo.MainGameInfo.currentMission -= 1;
		}else if(levelUpFlag && GlobalInfo.MainGameInfo.currentMission == 7){
			GlobalInfo.MainGameInfo.currentMission = 6;
		}
		Time.timeScale = 1f;
		Application.LoadLevel (2);
	}

	//When User tap resume button on pause panel
	public void OnResumeButtonPressed(){
		GlobalInfo.MainGameInfo.pauseFlag = false;
		Time.timeScale = 1;
		if (GlobalInfo.MainGameInfo.soundFlag) {
			AudioListener.volume = 1f;
		}else{
			AudioListener.volume = 0;
		}
		myTouchController.gameObject.SetActive (true);
		if (GlobalInfo.MainGameInfo.miniMapFlag) {
			miniMap.SetActive(true);
			miniMapButton.SetActive (false);
		}else{
			miniMap.SetActive(false);
			miniMapButton.SetActive(true);
		}
		uiPanel.SetActive (true);
		pausePanel.SetActive (false);
		Camera.main.GetComponent<RadarSystem> ().enabled = true;
		mainPlayer.gameObject.SetActive (true);
		if (GlobalInfo.MainGameInfo.specialCam) {
			Camera.main.SendMessage("EnableNoise",SendMessageOptions.DontRequireReceiver);

			telescopeSprite.gameObject.AddComponent<TweenAlpha>();
			telescopeSprite.gameObject.GetComponent<TweenAlpha>().from = 0.5f;
			telescopeSprite.gameObject.GetComponent<TweenAlpha>().from = 1f;
			telescopeSprite.gameObject.GetComponent<TweenAlpha>().duration= 0.5f;
			mainPlayer.Find("Shadow").gameObject.SetActive(false);

			magnifySlider.gameObject.SetActive(true);
			GlobalInfo.MainGameInfo.crossHair_Pos = new Vector2 (Screen.width * 0.5f, Screen.height * 0.5f);
			Camera.main.GetComponent<SmoothFollow>().enabled = false;
			Camera.main.transform.parent = sightCamPos;
			Camera.main.transform.localPosition = Vector3.zero;
			Camera.main.transform.localRotation = Quaternion.identity;
			specialCamButton.normalSprite = "G_2";
			specialCam.SetActive(true);
			distanceLabel.SetActive(true);
		}
	}

	public void OnMainmenuButtonPressed(){
		int tp = PlayerPrefs.GetInt("MaxLevel");
		if(tp < GlobalInfo.MainGameInfo.currentMission){
			PlayerPrefs.SetInt ("MaxLevel", GlobalInfo.MainGameInfo.currentMission);
		}
		Time.timeScale = 1f;
		Application.LoadLevel (1);
	}

	//When user tap next button on level clear panel
	public void OnNextPressed(){

		if (GlobalInfo.MainGameInfo.currentMission == 7) {
		}else{

				foreach (GameObject gb in ui_briefs) {
					gb.SetActive(false);
				}
				clearPanel.SetActive (false);
			
				ui_briefs [GlobalInfo.MainGameInfo.currentMission].SetActive (true);
				briefingPanel.SetActive (true);
		}
	}

	//When user tap play button on briefing panel
	public void OnPlayNextLevelPressed(){
        MainGameInfo.levelFailed = false;
        MainGameInfo.levelCompleted = false;
		Time.timeScale = 1f;
		Application.LoadLevel (2);
	}

	//When user tap back button on briefing panel
	public void OnBackToClearPanelPressed(){
		briefingPanel.SetActive (false);
		if (MainGameInfo.currentMission == 6) {
			finalClearPanel.SetActive(true);
		}else{
			clearPanel.SetActive (true);
		}
	}

	//When level fails
	void OnLevelFails(){
		AdmobAd.Instance().LoadInterstitialAd(false);		
		GlobalInfo.MainGameInfo.pauseFlag = true;
		Time.timeScale = 0;
		miniMap.SetActive(false);
		GlobalInfo.MainGameInfo.miniMapFlag = false;
		myTouchController.gameObject.SetActive (false);
		uiPanel.SetActive (false);
		pausePanel.SetActive (true);
		Camera.main.GetComponent<RadarSystem> ().enabled = false;
		mainPlayer.gameObject.SetActive (false);
		failPanel.SetActive (true);
	}

	void OnEqualState(){
		Camera.main.GetComponent<SmoothFollow> ().enabled = false; 
		Camera.main.transform.parent = sightCamPos;
		Camera.main.transform.localPosition = Vector3.zero;

	}

	//when user tap patriot button
	public void OnPatriotSelect(){
		MainGameInfo.patriotFlag = !MainGameInfo.patriotFlag;
		if (MainGameInfo.patriotFlag) {
			scope_But.SetActive(false);
			autoAim_But.SetActive(false);
			patriotButton.normalSprite = "M_2";
			Camera.main.GetComponent<SmoothFollow>().enabled = false;
			Camera.main.transform.parent = patriotCamPos.transform;
			Camera.main.transform.localPosition = Vector3.zero;
			Camera.main.transform.localRotation = Quaternion.identity;
			patriotOffset.SetActive(true);
			patriotOffset.AddComponent<UISpriteAnimation>();
			patriotOffset.GetComponent<UISprite>().name = "R_a";

			if(MainGameInfo.specialCam){
				Camera.main.SendMessage("DisableNoise",SendMessageOptions.DontRequireReceiver);
				if(telescopeSprite.gameObject.GetComponent<TweenAlpha>() != null){
					Destroy(telescopeSprite.gameObject.GetComponent<TweenAlpha>());
				}
				mainPlayer.Find("Shadow").gameObject.SetActive(true);
				Camera.main.fieldOfView = 60f;
				magnifySlider.gameObject.SetActive(false);
				camChangeButton.gameObject.GetComponent<BoxCollider>().enabled = true;
				GlobalInfo.MainGameInfo.crossHair_Pos = new Vector2 (Screen.width * 0.5f, 128f);
				specialCamButton.normalSprite = "G_1";
				specialCam.SetActive(false);
				distanceLabel.SetActive(false);
			}
		}else{
			if(patriotOffset.GetComponent<UISpriteAnimation>() != null){
				Destroy (patriotOffset.GetComponent<UISpriteAnimation>());
			}
			scope_But.SetActive(true);
			autoAim_But.SetActive(true);
			patriotOffset.SetActive(false);
			patriotButton.normalSprite = "M_1";
			Camera.main.transform.parent = null;
			Camera.main.GetComponent<SmoothFollow>().enabled = true;
			patriotRotCamPos.localRotation = Quaternion.identity;
			if(MainGameInfo.specialCam){

				Camera.main.SendMessage("EnableNoise",SendMessageOptions.DontRequireReceiver);
				
				telescopeSprite.gameObject.AddComponent<TweenAlpha>();
				telescopeSprite.gameObject.GetComponent<TweenAlpha>().from = 0.5f;
				telescopeSprite.gameObject.GetComponent<TweenAlpha>().from = 1f;
				telescopeSprite.gameObject.GetComponent<TweenAlpha>().duration= 0.5f;
				mainPlayer.Find("Shadow").gameObject.SetActive(false);
				
				magnifySlider.gameObject.SetActive(true);
				GlobalInfo.MainGameInfo.crossHair_Pos = new Vector2 (Screen.width * 0.5f, Screen.height * 0.5f);
				Camera.main.GetComponent<SmoothFollow>().enabled = false;
				Camera.main.transform.parent = sightCamPos;
				Camera.main.transform.localPosition = Vector3.zero;
				Camera.main.transform.localRotation = Quaternion.identity;
				specialCamButton.normalSprite = "G_2";
				specialCam.SetActive(true);
				distanceLabel.SetActive(true);

			}
		}
	}
}
