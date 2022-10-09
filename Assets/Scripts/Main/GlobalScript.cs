using UnityEngine;
using System.Collections;

namespace GlobalInfo{
	public enum AIType{
		Idle = 0,
		Aggressive = 1,
		Patrol = 2,
		Guarder = 3,
	}
	public enum GameLevel{
		FirstStage = 0,
		SecondStage = 1,
		ThirdStage = 2,
		ForthStage = 3,
		FifthStage = 4,
	}
	public static class MainGameInfo{
        public static bool levelFailed = false;
        public static bool levelCompleted = false;
		public static int maxLevelIndex = 0;
		public static bool readyToShotFlag = false;
		public static bool specialCam = false;
		public static bool autoAim = false;
		public static bool pauseFlag = false;
		public static float spd = 0;
		public static float str = 0;
		public static float health = 300f;
		public static float score = 0;
		public static float enemyProgress = 0;
		public static Vector2 crossHair_Pos;
		public static bool aimReady = false;
		public static int currentMission = 0;
		public static int distanceToTarget = 0;
		public static bool soundFlag = true;
		public static bool musicFlag = true;
		public static bool miniMapFlag = false;
		public static bool tapFlag = false;
		public static bool patriotFlag = false;
	}
}