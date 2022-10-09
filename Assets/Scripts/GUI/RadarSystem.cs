/// <summary>
/// This is Radar System. using to detection an objects and showing on minimap by Tags[]
/// </summary>

using UnityEngine;
using System.Collections;

public enum AlignmentRadar { None,LeftTop, RightTop, LeftBot, RightBot ,MiddleTop ,MiddleBot}

public class RadarSystem : MonoBehaviour {
	
	private Vector2 inposition;
	public float Size = 400; // size of minimap
	public float Distance = 100;// maximum distance of objects
	public float Alpha = 0.5f;
	public Texture2D[] Navtexture;// textutes list
	public string[] EnemyTag;// object tags list
	public Texture2D NavCompass;// compass texture
	public Texture2D NavBG;// background texture
	public Vector2 PositionOffset = new Vector2(0,0);// minimap position offset
	public float Scale = 1;// mini map scale ( Scale < 1 = zoom in , Scale > 1 = zoom out)
	public AlignmentRadar PositionAlignment = AlignmentRadar.None;// position alignment
	public bool MapRotation;
	public GameObject Player;
	public bool Show = true;
	public Color ColorMult = Color.white;
	
	private float ringGUIWidth;
	private float ringGUIHeight;
	
	
	void Start () {
		Size = 140f;
		ringGUIWidth = Navtexture [0].width;
		ringGUIHeight = Navtexture [0].height;
		Size *= Screen.width / 800;
		PositionOffset = new Vector2 (PositionOffset.x * Screen.width/800 + 10f, PositionOffset.y*Screen.height/480 + 70f);
		
		if(!Player){
			Player = this.gameObject;
		}
		
		if(Scale<=0){
			Scale = 1;
		}
		
		switch(PositionAlignment){
		case AlignmentRadar.None:
			inposition = PositionOffset;
			break;
		case AlignmentRadar.LeftTop:
			inposition = Vector2.zero + PositionOffset;
			break;
		case AlignmentRadar.RightTop:
			inposition = new Vector2(Screen.width - Size,0) + PositionOffset;
			break;
		case AlignmentRadar.LeftBot:
			inposition = new Vector2(0,Screen.height - Size) + PositionOffset;
			break;
		case AlignmentRadar.RightBot:
			inposition = new Vector2(Screen.width - Size,Screen.height - Size) + PositionOffset;
			break;
		case AlignmentRadar.MiddleTop:
			inposition = new Vector2((Screen.width/2) - (Size/2),Size) + PositionOffset;
			break;
		case AlignmentRadar.MiddleBot:
			inposition = new Vector2((Screen.width/2) - (Size/2),Screen.height - Size) + PositionOffset;
			break;
		}
		
		//		Size *= Screen.height / 480;
		inposition = new Vector2 (inposition.x * Screen.width/ 800, inposition.y * Screen.height/480);
	}
	
	Vector2 ConvertToNavPosition(Vector3 pos){
		Vector2 res = Vector2.zero;
		if(Player){
			res.x = inposition.x + (((pos.x - Player.transform.position.x) + (Size * Scale)/2f) / Scale);
			res.y = inposition.y + ((-(pos.z - Player.transform.position.z) + (Size * Scale)/2f) / Scale);
		}
		return res;
	}
	
	
	void DrawNav(GameObject[] enemyLists){
		if(Player){
			for(int i=0;i<enemyLists.Length;i++){
				if(Vector3.Distance(Player.transform.position,enemyLists[i].transform.position)<= (Distance * Scale)){
					Vector2 pos = ConvertToNavPosition(enemyLists[i].transform.position);
					
					if(Vector2.Distance(pos,(inposition+ new Vector2(Size/2f,Size/2f))) + (ringGUIWidth/2) < (Size/2f)){
						float navscale = Scale;
						if(navscale<1){
							navscale = 2;
						}
						if(enemyLists[i].gameObject.CompareTag("Bunker")){
							GUI.DrawTexture(new Rect(pos.x - (ringGUIWidth/navscale)/2,pos.y - (ringGUIHeight/navscale)/2,ringGUIWidth/navscale,ringGUIHeight/navscale),Navtexture[0]);
						}else if(enemyLists[i].gameObject.CompareTag("RPG")){
							GUI.DrawTexture(new Rect(pos.x - (ringGUIWidth/navscale)/2,pos.y - (ringGUIHeight/navscale)/2,ringGUIWidth/navscale,ringGUIHeight/navscale),Navtexture[1]);
						}else if(enemyLists[i].gameObject.CompareTag("EnemyTank")){
							GUI.DrawTexture(new Rect(pos.x - (ringGUIWidth/navscale)/2,pos.y - (ringGUIHeight/navscale)/2,ringGUIWidth/navscale,ringGUIHeight/navscale),Navtexture[2]);
						}else if(enemyLists[i].gameObject.CompareTag("Helicopter")){
							GUI.DrawTexture(new Rect(pos.x - (ringGUIWidth/navscale)/2,pos.y - (ringGUIHeight/navscale)/2,ringGUIWidth/navscale,ringGUIHeight/navscale),Navtexture[2]);
						}
						else if(enemyLists[i].gameObject.CompareTag("F16")){
							GUI.DrawTexture(new Rect(pos.x - (ringGUIWidth/navscale)/2,pos.y - (ringGUIHeight/navscale)/2,ringGUIWidth/navscale,ringGUIHeight/navscale),Navtexture[2]);
						}
					}
				}
			}
		}
	}
	
	float[] list;
	void OnGUI(){
		Size = 140f;
		if(!Show)
			return;
		
		GUI.color = new Color(ColorMult.r,ColorMult.g,ColorMult.b,Alpha);
		if(MapRotation){
			GUIUtility.RotateAroundPivot (-(Player.transform.eulerAngles.y), inposition + new Vector2(Size/2f, Size/2f)); 
		}
		

		if(NavBG)
			GUI.DrawTexture(new Rect(inposition.x,inposition.y,Size,Size),NavBG);
		for(int i=0;i<EnemyTag.Length;i++){
			DrawNav(GameObject.FindGameObjectsWithTag(EnemyTag[i]));
		}
		GUIUtility.RotateAroundPivot ((Player.transform.eulerAngles.y), inposition + new Vector2(Size/2f, Size/2f)); 
		if(NavCompass)
			GUI.DrawTexture(new Rect(inposition.x + (Size/2f)-(NavCompass.width/2f),inposition.y + (Size/2f) - (NavCompass.height/2f),NavCompass.width,NavCompass.height),NavCompass);
		
	}
}




