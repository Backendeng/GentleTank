using UnityEngine;
using System.Collections;
public class GuiScale : MonoBehaviour {
	private int idx;
	int fsize;
//	public Font font;
//	private Vector2 localScale;
	void Start () {
		float fXScale = (float)Screen.width / 800;
		float fYScale = (float)Screen.height / 480;
		
		GUITexture[] allTexture;
		allTexture = gameObject.GetComponentsInChildren<GUITexture>();
		foreach (GUITexture tex in allTexture) {
			float fX = tex.pixelInset.x * fXScale;
			float fY = tex.pixelInset.y * fYScale;
			float fWidth = tex.pixelInset.width * fXScale;
			float fHeight = tex.pixelInset.height * fYScale;
            tex.pixelInset = new Rect(fX, fY, fWidth, fHeight);
        }

		GUIText[] allText;
		allText = gameObject.GetComponentsInChildren<GUIText>();
		foreach (GUIText te in allText) {
			float fX = te.pixelOffset.x * fXScale;
			float fY = te.pixelOffset.y * fYScale;
            te.pixelOffset = new Vector2(fX, fY);
			fsize = Mathf.FloorToInt(te.fontSize * fXScale);
			
//			te.font = font;
			te.fontSize = fsize;
//			if(te.fontStyle == FontStyle.Bold){
//			te.fontStyle = FontStyle.Bold;
//				te.material.color = Color.white;
//			}
//			else{
//				te.material.color = new Color(0.149f, 0.470f, 0.384f, 1);
//			}
//			if(te.name == "Txt_Count"){
//				te.material.color = new Color(0.9f, 0.3f, 0.2f, 1);
//				te.fontStyle = FontStyle.Bold;
//			}
		}
    }
}