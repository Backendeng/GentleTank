using UnityEngine;
using System.Collections;
using GlobalInfo;

public class MainMenuBehaviour : MonoBehaviour {
	private int menuState = 1;
	public GameObject springReallyExit;
	public GameObject springMainMenu;
	public GameObject springLevelMenu;
	public GameObject springBriefMenu;
	public GameObject soundsSprite;
	public GameObject musicSprite;
	public UIButton[] mission_But;
	public GameObject[] missionBrief;
	public GameObject mainCam;

	private bool soundFlag = true;

	// Use this for initialization
	void Start () {
		//small banner
		menuState = 1;
		if(GlobalInfo.MainGameInfo.soundFlag){
			AudioListener.volume = 1f;
		}else{
			AudioListener.volume = 0;
		}
		GlobalInfo.MainGameInfo.maxLevelIndex = PlayerPrefs.GetInt ("MaxLevel");
		GlobalInfo.MainGameInfo.maxLevelIndex = Mathf.Clamp (GlobalInfo.MainGameInfo.maxLevelIndex, 0, 6);
		AdmobAd.Instance().LoadBannerAd(AdmobAd.BannerAdType.Universal_Banner_320x50, AdmobAd.AdLayout.Bottom_Centered);	
		
	}


	void OnApplicationPause(){
		if (springMainMenu.GetComponent<TweenPosition> () != null) {
			Destroy (springMainMenu.GetComponent<TweenPosition>());
		}
		if (springLevelMenu.GetComponent<TweenPosition> () != null) {
			Destroy (springLevelMenu.GetComponent<TweenPosition>());
		}
		if (springReallyExit.GetComponent<TweenPosition> () != null) {
			Destroy (springReallyExit.GetComponent<TweenPosition>());
		}
		if (springBriefMenu.GetComponent<TweenPosition> () != null) {
			Destroy (springBriefMenu.GetComponent<TweenPosition>());
		}
		switch (menuState) {
			case 0:
				springMainMenu.transform.localPosition = new Vector3(1400,0,0);
				springReallyExit.transform.position = Vector3.zero;
				springBriefMenu.transform.localPosition = new Vector3 (1400, 0, 0);
				springLevelMenu.transform.localPosition = new Vector3 (1400, 0, 0);
				break;
			case 1:		
				springMainMenu.transform.position = Vector3.zero;
				springLevelMenu.transform.localPosition = new Vector3 (1400, 0, 0);
				springReallyExit.transform.localPosition = new Vector3 (-1400, 0, 0);
				springBriefMenu.transform.localPosition = new Vector3 (1400, 0, 0);
				break;
			case 2:
				springMainMenu.transform.localPosition = new Vector3(-1400,0,0);
				springBriefMenu.transform.localPosition = new Vector3 (1400, 0, 0);
				springReallyExit.transform.localPosition = new Vector3 (-1400, 0, 0);
				springLevelMenu.transform.position = Vector3.zero;
				break;
			case 3:
				springMainMenu.transform.position = new Vector3(-1400,0,0);
				springLevelMenu.transform.localPosition = new Vector3 (-1400, 0, 0);
				springReallyExit.transform.localPosition = new Vector3 (-1400, 0, 0);
				springBriefMenu.transform.position = Vector3.zero;
				break;	
			default:
				break;
		}
	}
	
	// Update is called once per frame
	void Update () {
//        print(menuState);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (menuState)
            {
                case 0:
                    OnReturnToMain();
                    menuState = 1;
                    break;
                case 1:
                    OnMainBack();
                    menuState = 0;
                    break;
                case 2:
                    OnLevelBackButtonPressed();
                    menuState = 1;
                    break;
                case 3:
                    OnBriefBackPressed();
                    menuState = 2;
                    break;
                default:
                    break;
            }
        }
		if(Input.GetKeyDown(KeyCode.Home)){
			return;
		}
		if(GlobalInfo.MainGameInfo.soundFlag){
			AudioListener.volume = 1f;
		}else{
			AudioListener.volume = 0;
		}

		if (GlobalInfo.MainGameInfo.soundFlag) {
			soundsSprite.GetComponent<UISprite>().spriteName = "SOUNDS";
		}else{
			soundsSprite.GetComponent<UISprite>().spriteName = "Sounds_Pressed";
		}
		if (GlobalInfo.MainGameInfo.musicFlag) {
			musicSprite.GetComponent<UISprite>().spriteName = "MUSIC";
			mainCam.GetComponent<AudioSource>().enabled = true;
		}else{
			musicSprite.GetComponent<UISprite>().spriteName = "Music_Pressed";
			mainCam.GetComponent<AudioSource>().enabled = false;
		}

		if(GlobalInfo.MainGameInfo.maxLevelIndex != 6){
			for(int i = 0; i < GlobalInfo.MainGameInfo.maxLevelIndex + 1; i ++){
				mission_But[i].state = UIButtonColor.State.Normal;
			}
			for(int i = GlobalInfo.MainGameInfo.maxLevelIndex + 1; i < 7; i ++){
				mission_But[i].state = UIButtonColor.State.Disabled;
			}
		}else{
			for(int i = 0; i < GlobalInfo.MainGameInfo.maxLevelIndex + 1; i ++){
				mission_But[i].state = UIButtonColor.State.Normal;
			}
		}
	}
	public void OnMainBack(){
		StartCoroutine(MainMenuBackButtonPressed());
		menuState = 0;
	}
	public void OnPlayReleased(){
		AdmobAd.Instance().LoadInterstitialAd(false);		
		//big banner
		menuState = 2;
//		Camera.main.GetComponent<AdmobGwangGo> ().jflag = true;
		StartCoroutine (PlayButtonAction ());
	}

	public void OnReturnToMain(){
		menuState = 1;
		StartCoroutine (ReturnToMainMenu ());
	}

	public void OnSoundsPressed(){
		soundFlag = !soundFlag;
		GlobalInfo.MainGameInfo.soundFlag = !GlobalInfo.MainGameInfo.soundFlag;
	}

	public void OnMusicPressed(){
		GlobalInfo.MainGameInfo.musicFlag = !GlobalInfo.MainGameInfo.musicFlag;

	}

	public void OnExitGame(){
		Application.Quit ();
	}

	public void OnLevelBackButtonPressed(){
		menuState = 1;
		StartCoroutine (LevelBackButtonAction ());
	}

	IEnumerator PlayButtonAction(){
		if (springLevelMenu.GetComponent<TweenPosition> () != null) {
			Destroy(springLevelMenu.GetComponent<TweenPosition>());
		}
		if (springMainMenu.GetComponent<TweenPosition> () != null) {
			Destroy(springMainMenu.GetComponent<TweenPosition>());
		}
		yield return new WaitForEndOfFrame ();
		springMainMenu.AddComponent<TweenPosition> ();
		springMainMenu.GetComponent<TweenPosition> ().from = Vector3.zero;
		springMainMenu.GetComponent<TweenPosition> ().to = new Vector3 (-1400, 0, 0);
		springMainMenu.GetComponent<TweenPosition> ().duration = 0.7f;
		springMainMenu.GetComponent<TweenPosition> ().enabled = true;
		
		springLevelMenu.AddComponent<TweenPosition>();
		springLevelMenu.GetComponent<TweenPosition> ().from = new Vector3 (1400, 0, 0);
		springLevelMenu.GetComponent<TweenPosition>().to = Vector3.zero;
		springLevelMenu.GetComponent<TweenPosition> ().duration = 0.7f;
		springLevelMenu.GetComponent<TweenPosition> ().enabled = true;
	}

	IEnumerator LevelBackButtonAction(){
		Destroy(springMainMenu.GetComponent<TweenPosition>());
		Destroy(springLevelMenu.GetComponent<TweenPosition>());
		yield return new WaitForEndOfFrame ();
		springMainMenu.AddComponent<TweenPosition> ();
		springMainMenu.GetComponent<TweenPosition> ().from = new Vector3 (-1400, 0, 0);
		springMainMenu.GetComponent<TweenPosition> ().to =Vector3.zero;
		springMainMenu.GetComponent<TweenPosition> ().duration = 0.7f;
		springMainMenu.GetComponent<TweenPosition> ().enabled = true;

		springLevelMenu.AddComponent<TweenPosition> ();
		springLevelMenu.GetComponent<TweenPosition> ().from = Vector3.zero;
		springLevelMenu.GetComponent<TweenPosition> ().to = new Vector3 (1400, 0, 0);
		springLevelMenu.GetComponent<TweenPosition> ().duration = 0.7f;
		springLevelMenu.GetComponent<TweenPosition> ().enabled = true;

	}

	IEnumerator MainMenuBackButtonPressed(){
		if (springMainMenu.GetComponent<TweenPosition> () != null) {
			Destroy(springMainMenu.GetComponent<TweenPosition>());
		}
		if (springReallyExit.GetComponent<TweenPosition> () != null) {
			Destroy(springReallyExit.GetComponent<TweenPosition>());
		}
		yield return new WaitForEndOfFrame();
		springMainMenu.AddComponent<TweenPosition> ();
		springMainMenu.GetComponent<TweenPosition> ().from = Vector3.zero;
		springMainMenu.GetComponent<TweenPosition> ().to = new Vector3(1400,0,0);
		springMainMenu.GetComponent<TweenPosition> ().duration = 0.7f;
		springMainMenu.GetComponent<TweenPosition> ().enabled = true;
		springReallyExit.AddComponent<TweenPosition> ();
		springReallyExit.GetComponent<TweenPosition> ().from = new Vector3(-1400,0,0);
		springReallyExit.GetComponent<TweenPosition> ().to = Vector3.zero;
		springReallyExit.GetComponent<TweenPosition> ().duration = 0.7f;
		springReallyExit.GetComponent<TweenPosition> ().enabled = true;
	}

	IEnumerator ReturnToMainMenu(){
		if (springMainMenu.GetComponent<TweenPosition> () != null) {
			Destroy(springMainMenu.GetComponent<TweenPosition>());
		}
		if (springReallyExit.GetComponent<TweenPosition> () != null) {
			Destroy(springReallyExit.GetComponent<TweenPosition>());
		}
		yield return new WaitForEndOfFrame();
		springReallyExit.AddComponent<TweenPosition> ();
		springReallyExit.GetComponent<TweenPosition> ().from = Vector3.zero;
		springReallyExit.GetComponent<TweenPosition> ().to = new Vector3(-1400,0,0);
		springReallyExit.GetComponent<TweenPosition> ().duration = 0.7f;
		springReallyExit.GetComponent<TweenPosition> ().enabled = true;

		springMainMenu.AddComponent<TweenPosition> ();
		springMainMenu.GetComponent<TweenPosition> ().from = new Vector3 (1400, 0, 0);
		springMainMenu.GetComponent<TweenPosition> ().to =Vector3.zero;
		springMainMenu.GetComponent<TweenPosition> ().duration = 0.7f;
		springMainMenu.GetComponent<TweenPosition> ().enabled = true;

	}

	IEnumerator OnBrief(){
		menuState = 3;
		if (springLevelMenu.GetComponent<TweenPosition> () != null) {
			Destroy(springLevelMenu.GetComponent<TweenPosition>());
		}
		if (springBriefMenu.GetComponent<TweenPosition> () != null) {
			Destroy(springBriefMenu.GetComponent<TweenPosition>());
		}
		
		yield return new WaitForEndOfFrame ();
		springLevelMenu.AddComponent<TweenPosition> ();
		springLevelMenu.GetComponent<TweenPosition> ().from = Vector3.zero;
		springLevelMenu.GetComponent<TweenPosition> ().to = new Vector3(-1400f,0,0);
		springLevelMenu.GetComponent<TweenPosition> ().duration = 0.7f;
		springLevelMenu.GetComponent<TweenPosition> ().enabled = true;
		
		springBriefMenu.AddComponent<TweenPosition> ();
		springBriefMenu.GetComponent<TweenPosition> ().from = new Vector3 (1400, 0, 0);
		springBriefMenu.GetComponent<TweenPosition> ().to =Vector3.zero;
		springBriefMenu.GetComponent<TweenPosition> ().duration = 0.7f;
		springBriefMenu.GetComponent<TweenPosition> ().enabled = true;

	}
	public void OnMission_1(){
		GlobalInfo.MainGameInfo.score = 0;
		GlobalInfo.MainGameInfo.currentMission = 0;
		for (int i = 0; i < 7; i ++) {
			if(i != 0){
				missionBrief[i].SetActive(false);
			}else{
				missionBrief[i].SetActive(true);
			}
		}
		StartCoroutine (OnBrief ());

	}

	public void OnMission_2(){
		GlobalInfo.MainGameInfo.score = 0;
		GlobalInfo.MainGameInfo.currentMission = 1;
		if ((int)GlobalInfo.MainGameInfo.currentMission > GlobalInfo.MainGameInfo.maxLevelIndex) {
			return;
		}
		for (int i = 0; i < 7; i ++) {
			if(i != 1){
				missionBrief[i].SetActive(false);
			}else{
				missionBrief[i].SetActive(true);
			}
		}
		StartCoroutine (OnBrief ());
	}

	public void OnMission_3(){
		GlobalInfo.MainGameInfo.score = 0;
		GlobalInfo.MainGameInfo.currentMission = 2;
		if ((int)GlobalInfo.MainGameInfo.currentMission > GlobalInfo.MainGameInfo.maxLevelIndex) {
			return;
		}
		for (int i = 0; i < 7; i ++) {
			if(i != 2){
				missionBrief[i].SetActive(false);
			}else{
				missionBrief[i].SetActive(true);
			}
		}
		StartCoroutine (OnBrief ());
	}

	public void OnMission_4(){
		GlobalInfo.MainGameInfo.score = 0;
		GlobalInfo.MainGameInfo.currentMission = 3;
		if ((int)GlobalInfo.MainGameInfo.currentMission > GlobalInfo.MainGameInfo.maxLevelIndex) {
			return;
		}
		for (int i = 0; i < 7; i ++) {
			if(i != 3){
				missionBrief[i].SetActive(false);
			}else{
				missionBrief[i].SetActive(true);
			}
		}
		StartCoroutine (OnBrief ());
	}

	public void OnMission_5(){
		GlobalInfo.MainGameInfo.score = 0;
		GlobalInfo.MainGameInfo.currentMission = 4;
		if ((int)GlobalInfo.MainGameInfo.currentMission > GlobalInfo.MainGameInfo.maxLevelIndex) {
			return;
		}
		for (int i = 0; i < 7; i ++) {
			if(i != 4){
				missionBrief[i].SetActive(false);
			}else{
				missionBrief[i].SetActive(true);
			}
		}
		StartCoroutine (OnBrief ());
	}

	public void OnMission_6(){

		GlobalInfo.MainGameInfo.score = 0;
		GlobalInfo.MainGameInfo.currentMission = 5;
		if ((int)GlobalInfo.MainGameInfo.currentMission > GlobalInfo.MainGameInfo.maxLevelIndex) {
			return;
		}
		for (int i = 0; i < 7; i ++) {
			if(i != 5){
				missionBrief[i].SetActive(false);
			}else{
				missionBrief[i].SetActive(true);
			}
		}
		StartCoroutine (OnBrief ());
	}

	public void OnMission_7(){
		GlobalInfo.MainGameInfo.score = 0;
		GlobalInfo.MainGameInfo.currentMission = 6;
		if ((int)GlobalInfo.MainGameInfo.currentMission > GlobalInfo.MainGameInfo.maxLevelIndex) {
			return;
		}
		for (int i = 0; i < 7; i ++) {
			if(i != 6){
				missionBrief[i].SetActive(false);
			}else{
				missionBrief[i].SetActive(true);
			}
		}
		StartCoroutine (OnBrief ());
	}

	public void OnBriefBackPressed(){
		menuState = 2;
		StartCoroutine (BriefBack ());
	}

	IEnumerator BriefBack(){
		if (springBriefMenu.GetComponent<TweenPosition> () != null) {
			Destroy(springLevelMenu.GetComponent<TweenPosition>());
		}
		if (springLevelMenu.GetComponent<TweenPosition> () != null) {
			Destroy(springBriefMenu.GetComponent<TweenPosition>());
		}
		
		yield return new WaitForEndOfFrame ();
		springLevelMenu.AddComponent<TweenPosition> ();
		springLevelMenu.GetComponent<TweenPosition> ().from = new Vector3(-1400,0,0);
		springLevelMenu.GetComponent<TweenPosition> ().to = Vector3.zero;
		springLevelMenu.GetComponent<TweenPosition> ().duration = 0.7f;
		springLevelMenu.GetComponent<TweenPosition> ().enabled = true;
		
		springBriefMenu.AddComponent<TweenPosition> ();
		springBriefMenu.GetComponent<TweenPosition> ().from = Vector3.zero;
		springBriefMenu.GetComponent<TweenPosition> ().to = new Vector3 (1400, 0, 0);
		springBriefMenu.GetComponent<TweenPosition> ().duration = 0.7f;
		springBriefMenu.GetComponent<TweenPosition> ().enabled = true;
	}
	public void OnMoreButtonPressed(){
		Application.OpenURL ("https://play.google.com/store/apps/developer?id=3Dee+Space");
	}

	public void OnRateButtonPressed(){
		Application.OpenURL ("https://play.google.com/store/apps/developer?id=3Dee+Space");
	}

	public void OnMainGamePlay(){
		AdmobAd.Instance().DestroyBannerAd();
//		Camera.main.GetComponent<playAdbanerC> ().hideBanner ();
		Application.LoadLevel (2);
	}


}
