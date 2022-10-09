using UnityEngine;
using System.Collections;

public class CrawlerBehaviourEx : MonoBehaviour {
//	public Transform wheelParent;
	public float[] wheelRadius;
	public float crawlerWidth = 0.4f;
	public float crawlerThickness = 0.05f;
	public Transform wheelParent;

	private Transform[] leftWheel;
	private Transform[] rightWheel;
	private int wheelCount = 0;
	private Mesh leftCrawlerMesh;
	private Mesh rightCrawlerMesh;
	private Vector3[] crawlerVertics;
	private int[] crawlerTris;
	private Vector2[] crawlerUVs;
	private Renderer leftCrawler;
	private Renderer rightCrawler;

	private System.Collections.Generic.List<Vector3> tmpVert = new System.Collections.Generic.List<Vector3>();
	private System.Collections.Generic.List<int> tmpTris = new System.Collections.Generic.List<int>();
	private System.Collections.Generic.List<Vector2> tmpUV = new System.Collections.Generic.List<Vector2>();
	// Use this for initialization
	void Start () {
		leftCrawlerMesh = transform.Find ("LeftCrawler").GetComponent<MeshFilter> ().mesh;
		rightCrawlerMesh = transform.Find ("RightCrawler").GetComponent<MeshFilter> ().mesh;
		InitWheelInfo ();
	}
	
	// Update is called once per frame
	void Update () {
		MakeLeftCrawlerMesh ();
		leftCrawlerMesh.vertices = crawlerVertics;
		leftCrawlerMesh.triangles = crawlerTris;
		leftCrawlerMesh.uv = crawlerUVs;
		leftCrawlerMesh.RecalculateNormals ();
		MakeRightCrawlerMesh ();
		rightCrawlerMesh.vertices = crawlerVertics;
		rightCrawlerMesh.triangles = crawlerTris;
		rightCrawlerMesh.uv = crawlerUVs;
		rightCrawlerMesh.RecalculateNormals ();
	}

	void InitWheelInfo() {
		leftCrawler = transform.Find ("LeftCrawler").GetComponent<Renderer>();
		rightCrawler = transform.Find ("RightCrawler").GetComponent<Renderer>();
		wheelCount = wheelParent.childCount / 2;
		leftWheel = new Transform[wheelCount];
		rightWheel = new Transform[wheelCount];
		foreach(Transform obj in wheelParent) {
			string n = obj.name;
			n = n.Substring(5);
			if(n.Substring(0,1) == "L"){
				leftWheel[System.Int32.Parse(n.Substring(1)) - 1] = obj;
			}else{
				rightWheel[System.Int32.Parse(n.Substring(1)) - 1] = obj;
			}
		}
	}

	void MakeLeftCrawlerMesh() {
		float firstAng = 0.0f;
		int m = 0;
		int vertCount = 0;
		int trisCount = 0;
		int uvCount = 0;
		float curUVx = 0.0f;
		float uvStp = 0.8f / (wheelCount - 1);
		float lastAng = 0.0f;

		tmpVert.Clear ();
		tmpTris.Clear ();
		tmpUV.Clear ();
		Vector2 p1 = new Vector2(leftWheel[0].localPosition.z,leftWheel[0].localPosition.y);
		Vector2 p2 = new Vector2(leftWheel[1].localPosition.z,leftWheel[1].localPosition.y);
		Vector2 p3 = p2 - p1;
		
		float dist = p3.magnitude;
		float rDiff = wheelRadius[1] - wheelRadius[0];
		float alpha = Mathf.Asin(rDiff / dist);
		dist = Mathf.Asin(p3.y / p3.magnitude);
		dist -= (alpha + Mathf.PI / 2.0f);
		dist += Mathf.PI;
		firstAng = dist;
		Vector2 p = new Vector2(leftWheel[0].localPosition.z,leftWheel[0].localPosition.y);
		float stp = (firstAng + Mathf.PI / 2) / 5.0f;
		lastAng = firstAng;
		for(float ang = -Mathf.PI / 2;ang <firstAng - stp / 2;ang += stp) {
			p1 = p + new Vector2(wheelRadius[0] * Mathf.Cos(ang),-wheelRadius[0] * Mathf.Sin(ang));
			p2 = p + new Vector2(wheelRadius[0] * Mathf.Cos(ang + stp),-wheelRadius[0] * Mathf.Sin(ang + stp));
			tmpVert.Add(new Vector3(leftWheel[0].localPosition.x + crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(leftWheel[0].localPosition.x - crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(leftWheel[0].localPosition.x - crawlerWidth,p2.y,p2.x));
			tmpVert.Add(new Vector3(leftWheel[0].localPosition.x + crawlerWidth,p2.y,p2.x));
			p1 = p + new Vector2((wheelRadius[0] + crawlerThickness) * Mathf.Cos(ang),-(wheelRadius[0] + crawlerThickness) * Mathf.Sin(ang));
			p2 = p + new Vector2((wheelRadius[0] + crawlerThickness) * Mathf.Cos(ang + stp),-(wheelRadius[0] + crawlerThickness) * Mathf.Sin(ang + stp));
			tmpVert.Add(new Vector3(leftWheel[0].localPosition.x + crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(leftWheel[0].localPosition.x - crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(leftWheel[0].localPosition.x - crawlerWidth,p2.y,p2.x));
			tmpVert.Add(new Vector3(leftWheel[0].localPosition.x + crawlerWidth,p2.y,p2.x));
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 1);
			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 3);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount);
			
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 6);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 4);
			tmpTris.Add(vertCount + 5);
			
			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 1);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 6);
			
			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 4);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 3);
			trisCount += 24;
			vertCount += 8;
		}
		for(int n = 0;n < wheelCount - 1;n++) {
			m = n + 1;
			p1 = new Vector2(leftWheel[n].localPosition.z,leftWheel[n].localPosition.y);
			p2 = new Vector2(leftWheel[m].localPosition.z,leftWheel[m].localPosition.y);
			p3 = p2 - p1;
			bool flag = false;

			dist = p3.magnitude;
			rDiff = wheelRadius[m] - wheelRadius[n];
			float ang = Mathf.Asin(rDiff / dist);
			dist = Mathf.Asin(p3.y / p3.magnitude);
			dist -= (ang + Mathf.PI / 2.0f);
			dist += Mathf.PI;
			if(dist > lastAng) flag = true;
			p1 = p1 + new Vector2(wheelRadius[n] * Mathf.Cos(dist),-wheelRadius[n] * Mathf.Sin(dist));
			dist = Mathf.Asin(p3.y / p3.magnitude);
			dist += (Mathf.PI / 2.0f - ang);
			p2 = p2 + new Vector2(wheelRadius[m] * Mathf.Cos(dist),-wheelRadius[m] * Mathf.Sin(dist));

			tmpVert.Add(new Vector3(leftWheel[n].localPosition.x + crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(leftWheel[n].localPosition.x - crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(leftWheel[n].localPosition.x - crawlerWidth,p2.y,p2.x));
			tmpVert.Add(new Vector3(leftWheel[n].localPosition.x + crawlerWidth,p2.y,p2.x));
//			if(n == 0) {
//				tmpVert.Add(new Vector3(tmpVert[vertCount - 1].x ,tmpVert[vertCount - 1].y,tmpVert[vertCount - 1].z));
//				tmpVert.Add(new Vector3(tmpVert[vertCount - 2].x ,tmpVert[vertCount - 2].y,tmpVert[vertCount - 2].z));
//			}else{
				tmpVert.Add(new Vector3(leftWheel[n].localPosition.x + crawlerWidth,p1.y - crawlerThickness,p1.x));
				tmpVert.Add(new Vector3(leftWheel[n].localPosition.x - crawlerWidth,p1.y - crawlerThickness,p1.x));
//			}
			tmpVert.Add(new Vector3(leftWheel[n].localPosition.x - crawlerWidth,p2.y - crawlerThickness,p2.x));
			tmpVert.Add(new Vector3(leftWheel[n].localPosition.x + crawlerWidth,p2.y - crawlerThickness,p2.x));
			if(flag) {
				tmpTris.Add(vertCount - 6);
				tmpTris.Add(vertCount - 5);
				tmpTris.Add(vertCount);
				tmpTris.Add(vertCount - 6);
				tmpTris.Add(vertCount);
				tmpTris.Add(vertCount + 1);

				tmpTris.Add(vertCount + 4);
				tmpTris.Add(vertCount - 2);
				tmpTris.Add(vertCount + 5);
				tmpTris.Add(vertCount + 4);
				tmpTris.Add(vertCount - 1);
				tmpTris.Add(vertCount - 2);

				tmpTris.Add(vertCount + 5);
				tmpTris.Add(vertCount - 6);
				tmpTris.Add(vertCount + 1);
				tmpTris.Add(vertCount + 5);
				tmpTris.Add(vertCount - 2);
				tmpTris.Add(vertCount - 6);
				
				tmpTris.Add(vertCount);
				tmpTris.Add(vertCount - 1);
				tmpTris.Add(vertCount + 4);
				tmpTris.Add(vertCount);
				tmpTris.Add(vertCount - 5);
				tmpTris.Add(vertCount - 1);
				trisCount += 24;				
			}

			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 1);
			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 3);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount);

			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 6);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 4);
			tmpTris.Add(vertCount + 5);

			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 1);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 6);

			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 4);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 3);
			trisCount += 24;
			vertCount += 8;
			lastAng = dist;
		}
		p = new Vector2(leftWheel[wheelCount - 1].localPosition.z,leftWheel[wheelCount - 1].localPosition.y);
		stp = (Mathf.PI * 3 / 2 - lastAng) / 5.0f;
		for(float ang = lastAng;ang <Mathf.PI * 3 / 2 - stp / 2;ang += stp) {
			p1 = p + new Vector2(wheelRadius[wheelCount - 1] * Mathf.Cos(ang),-wheelRadius[wheelCount - 1] * Mathf.Sin(ang));
			p2 = p + new Vector2(wheelRadius[wheelCount - 1] * Mathf.Cos(ang + stp),-wheelRadius[wheelCount - 1] * Mathf.Sin(ang + stp));
			tmpVert.Add(new Vector3(leftWheel[wheelCount - 1].localPosition.x + crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(leftWheel[wheelCount - 1].localPosition.x - crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(leftWheel[wheelCount - 1].localPosition.x - crawlerWidth,p2.y,p2.x));
			tmpVert.Add(new Vector3(leftWheel[wheelCount - 1].localPosition.x + crawlerWidth,p2.y,p2.x));
			if(ang == lastAng){
				p1 = p + new Vector2(wheelRadius[wheelCount - 1] * Mathf.Cos(ang),-wheelRadius[wheelCount - 1] * Mathf.Sin(ang) - crawlerThickness);
			}else{
				p1 = p + new Vector2((wheelRadius[wheelCount - 1] + crawlerThickness) * Mathf.Cos(ang),-(wheelRadius[wheelCount - 1] + crawlerThickness) * Mathf.Sin(ang));
			}
			p2 = p + new Vector2((wheelRadius[wheelCount - 1] + crawlerThickness) * Mathf.Cos(ang + stp),-(wheelRadius[wheelCount - 1] + crawlerThickness) * Mathf.Sin(ang + stp));
			tmpVert.Add(new Vector3(leftWheel[wheelCount - 1].localPosition.x + crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(leftWheel[wheelCount - 1].localPosition.x - crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(leftWheel[wheelCount - 1].localPosition.x - crawlerWidth,p2.y,p2.x));
			tmpVert.Add(new Vector3(leftWheel[wheelCount - 1].localPosition.x + crawlerWidth,p2.y,p2.x));
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 1);
			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 3);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount);
			
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 6);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 4);
			tmpTris.Add(vertCount + 5);
			
			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 1);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 6);
			
			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 4);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 3);
			trisCount += 24;
			vertCount += 8;
		}
		float sum = 0.0f;
		
		for(int i = 0;i < vertCount;i+=8) {
			Vector3 tmp = tmpVert[i + 7] - tmpVert[i];
			sum += tmp.magnitude;
		}
		float curLen = 0.0f;
		
		for(int i = 0;i < vertCount;i+=8) {
			Vector3 tmp = tmpVert[i + 7] - tmpVert[i];
			tmpUV.Add(new Vector2(curLen / sum,0.2f));
			tmpUV.Add(new Vector2(curLen / sum,1.0f));
			tmpUV.Add(new Vector2((curLen + tmp.magnitude)/ sum,1.0f));
			tmpUV.Add(new Vector2((curLen + tmp.magnitude)/ sum,0.2f));
			tmpUV.Add(new Vector2(curLen / sum,0.2f));
			tmpUV.Add(new Vector2(curLen / sum,1.0f));
			tmpUV.Add(new Vector2((curLen + tmp.magnitude)/ sum,1.0f));
			tmpUV.Add(new Vector2((curLen + tmp.magnitude)/ sum,0.2f));
			uvCount += 8;
			curLen += tmp.magnitude;
		}

		crawlerVertics = new Vector3[vertCount];
		for(int i = 0;i < vertCount;i++){
			crawlerVertics[i] = tmpVert[i];
		}
		crawlerTris = new int[trisCount];
		for(int i = 0;i < trisCount;i++){
			crawlerTris[i] = tmpTris[i];
		}
		crawlerUVs = new Vector2[uvCount];
		for(int i = 0;i < uvCount;i++){
			crawlerUVs[i] = tmpUV[i];
		}
//		lastAng = 1000.0f;
	}
	void MakeRightCrawlerMesh() {
		float firstAng = 0.0f;
		int m = 0;
		int vertCount = 0;
		int trisCount = 0;
		int uvCount = 0;
		float curUVx = 0.0f;
		float uvStp = 0.8f / (wheelCount - 1);
		float lastAng = 0.0f;
		
		tmpVert.Clear ();
		tmpTris.Clear ();
		tmpUV.Clear ();

		Vector2 p1 = new Vector2(rightWheel[0].localPosition.z,rightWheel[0].localPosition.y);
		Vector2 p2 = new Vector2(rightWheel[1].localPosition.z,rightWheel[1].localPosition.y);
		Vector2 p3 = p2 - p1;
		
		float dist = p3.magnitude;
		float rDiff = wheelRadius[1] - wheelRadius[0];
		float alpha = Mathf.Asin(rDiff / dist);
		dist = Mathf.Asin(p3.y / p3.magnitude);
		dist -= (alpha + Mathf.PI / 2.0f);
		dist += Mathf.PI;
		firstAng = dist;
		lastAng = firstAng;
		Vector2 p = new Vector2(rightWheel[0].localPosition.z,rightWheel[0].localPosition.y);
		float stp = (firstAng + Mathf.PI / 2) / 5.0f;
		
		for(float ang = -Mathf.PI / 2;ang <firstAng - stp / 2;ang += stp) {
			p1 = p + new Vector2(wheelRadius[0] * Mathf.Cos(ang),-wheelRadius[0] * Mathf.Sin(ang));
			p2 = p + new Vector2(wheelRadius[0] * Mathf.Cos(ang + stp),-wheelRadius[0] * Mathf.Sin(ang + stp));
			tmpVert.Add(new Vector3(rightWheel[0].localPosition.x + crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(rightWheel[0].localPosition.x - crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(rightWheel[0].localPosition.x - crawlerWidth,p2.y,p2.x));
			tmpVert.Add(new Vector3(rightWheel[0].localPosition.x + crawlerWidth,p2.y,p2.x));
			p1 = p + new Vector2((wheelRadius[0] + crawlerThickness) * Mathf.Cos(ang),-(wheelRadius[0] + crawlerThickness) * Mathf.Sin(ang));
			p2 = p + new Vector2((wheelRadius[0] + crawlerThickness) * Mathf.Cos(ang + stp),-(wheelRadius[0] + crawlerThickness) * Mathf.Sin(ang + stp));
			tmpVert.Add(new Vector3(rightWheel[0].localPosition.x + crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(rightWheel[0].localPosition.x - crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(rightWheel[0].localPosition.x - crawlerWidth,p2.y,p2.x));
			tmpVert.Add(new Vector3(rightWheel[0].localPosition.x + crawlerWidth,p2.y,p2.x));
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 1);
			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 3);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount);
			
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 6);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 4);
			tmpTris.Add(vertCount + 5);
			
			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 1);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 6);
			
			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 4);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 3);
			trisCount += 24;
			vertCount += 8;
		}
		for(int n = 0;n < wheelCount - 1;n++) {
			m = n + 1;
			p1 = new Vector2(rightWheel[n].localPosition.z,rightWheel[n].localPosition.y);
			p2 = new Vector2(rightWheel[m].localPosition.z,rightWheel[m].localPosition.y);
			p3 = p2 - p1;
			bool flag = false;
			
			dist = p3.magnitude;
			rDiff = wheelRadius[m] - wheelRadius[n];
			float ang = Mathf.Asin(rDiff / dist);
			dist = Mathf.Asin(p3.y / p3.magnitude);
			dist -= (ang + Mathf.PI / 2.0f);
			dist += Mathf.PI;
			if(dist > lastAng) flag = true;
			p1 = p1 + new Vector2(wheelRadius[n] * Mathf.Cos(dist),-wheelRadius[n] * Mathf.Sin(dist));
			dist = Mathf.Asin(p3.y / p3.magnitude);
			dist += (Mathf.PI / 2.0f - ang);
			p2 = p2 + new Vector2(wheelRadius[m] * Mathf.Cos(dist),-wheelRadius[m] * Mathf.Sin(dist));
			
			tmpVert.Add(new Vector3(rightWheel[n].localPosition.x + crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(rightWheel[n].localPosition.x - crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(rightWheel[n].localPosition.x - crawlerWidth,p2.y,p2.x));
			tmpVert.Add(new Vector3(rightWheel[n].localPosition.x + crawlerWidth,p2.y,p2.x));
			if(n == 0) {
				tmpVert.Add(new Vector3(tmpVert[vertCount - 1].x ,tmpVert[vertCount - 1].y,tmpVert[vertCount - 1].z));
				tmpVert.Add(new Vector3(tmpVert[vertCount - 2].x ,tmpVert[vertCount - 2].y,tmpVert[vertCount - 2].z));
			}else{
				tmpVert.Add(new Vector3(rightWheel[n].localPosition.x + crawlerWidth,p1.y - crawlerThickness,p1.x));
				tmpVert.Add(new Vector3(rightWheel[n].localPosition.x - crawlerWidth,p1.y - crawlerThickness,p1.x));
			}
			tmpVert.Add(new Vector3(rightWheel[n].localPosition.x - crawlerWidth,p2.y - crawlerThickness,p2.x));
			tmpVert.Add(new Vector3(rightWheel[n].localPosition.x + crawlerWidth,p2.y - crawlerThickness,p2.x));
			if(flag) {
				tmpTris.Add(vertCount - 6);
				tmpTris.Add(vertCount - 5);
				tmpTris.Add(vertCount);
				tmpTris.Add(vertCount - 6);
				tmpTris.Add(vertCount);
				tmpTris.Add(vertCount + 1);
				
				tmpTris.Add(vertCount + 4);
				tmpTris.Add(vertCount - 2);
				tmpTris.Add(vertCount + 5);
				tmpTris.Add(vertCount + 4);
				tmpTris.Add(vertCount - 1);
				tmpTris.Add(vertCount - 2);
				
				tmpTris.Add(vertCount + 5);
				tmpTris.Add(vertCount - 6);
				tmpTris.Add(vertCount + 1);
				tmpTris.Add(vertCount + 5);
				tmpTris.Add(vertCount - 2);
				tmpTris.Add(vertCount - 6);
				
				tmpTris.Add(vertCount);
				tmpTris.Add(vertCount - 1);
				tmpTris.Add(vertCount + 4);
				tmpTris.Add(vertCount);
				tmpTris.Add(vertCount - 5);
				tmpTris.Add(vertCount - 1);
				trisCount += 24;				
			}
			
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 1);
			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 3);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount);
			
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 6);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 4);
			tmpTris.Add(vertCount + 5);
			
			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 1);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 6);
			
			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 4);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 3);
			trisCount += 24;
			vertCount += 8;
			lastAng = dist;
		}
		p = new Vector2(rightWheel[wheelCount - 1].localPosition.z,rightWheel[wheelCount - 1].localPosition.y);
		stp = (Mathf.PI * 3 / 2 - lastAng) / 5.0f;
		for(float ang = lastAng;ang <Mathf.PI * 3 / 2 - stp / 2;ang += stp) {
			p1 = p + new Vector2(wheelRadius[wheelCount - 1] * Mathf.Cos(ang),-wheelRadius[wheelCount - 1] * Mathf.Sin(ang));
			p2 = p + new Vector2(wheelRadius[wheelCount - 1] * Mathf.Cos(ang + stp),-wheelRadius[wheelCount - 1] * Mathf.Sin(ang + stp));
			tmpVert.Add(new Vector3(rightWheel[wheelCount - 1].localPosition.x + crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(rightWheel[wheelCount - 1].localPosition.x - crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(rightWheel[wheelCount - 1].localPosition.x - crawlerWidth,p2.y,p2.x));
			tmpVert.Add(new Vector3(rightWheel[wheelCount - 1].localPosition.x + crawlerWidth,p2.y,p2.x));
			if(ang == lastAng){
				p1 = p + new Vector2(wheelRadius[wheelCount - 1] * Mathf.Cos(ang),-wheelRadius[wheelCount - 1] * Mathf.Sin(ang) - crawlerThickness);
			}else{
				p1 = p + new Vector2((wheelRadius[wheelCount - 1] + crawlerThickness) * Mathf.Cos(ang),-(wheelRadius[wheelCount - 1] + crawlerThickness) * Mathf.Sin(ang));
			}
			p2 = p + new Vector2((wheelRadius[wheelCount - 1] + crawlerThickness) * Mathf.Cos(ang + stp),-(wheelRadius[wheelCount - 1] + crawlerThickness) * Mathf.Sin(ang + stp));
			tmpVert.Add(new Vector3(rightWheel[wheelCount - 1].localPosition.x + crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(rightWheel[wheelCount - 1].localPosition.x - crawlerWidth,p1.y,p1.x));
			tmpVert.Add(new Vector3(rightWheel[wheelCount - 1].localPosition.x - crawlerWidth,p2.y,p2.x));
			tmpVert.Add(new Vector3(rightWheel[wheelCount - 1].localPosition.x + crawlerWidth,p2.y,p2.x));
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 1);
			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 3);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount);
			
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 6);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 4);
			tmpTris.Add(vertCount + 5);
			
			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 1);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 5);
			tmpTris.Add(vertCount + 2);
			tmpTris.Add(vertCount + 6);
			
			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 4);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount);
			tmpTris.Add(vertCount + 7);
			tmpTris.Add(vertCount + 3);
			trisCount += 24;
			vertCount += 8;
		}
		float sum = 0.0f;
		
		for(int i = 0;i < vertCount;i+=8) {
			Vector3 tmp = tmpVert[i + 7] - tmpVert[i];
			sum += tmp.magnitude;
		}
		float curLen = 0.0f;
		
		for(int i = 0;i < vertCount;i+=8) {
			Vector3 tmp = tmpVert[i + 7] - tmpVert[i];
			tmpUV.Add(new Vector2(curLen / sum,0.2f));
			tmpUV.Add(new Vector2(curLen / sum,1.0f));
			tmpUV.Add(new Vector2((curLen + tmp.magnitude)/ sum,1.0f));
			tmpUV.Add(new Vector2((curLen + tmp.magnitude)/ sum,0.2f));
			tmpUV.Add(new Vector2(curLen / sum,0.2f));
			tmpUV.Add(new Vector2(curLen / sum,1.0f));
			tmpUV.Add(new Vector2((curLen + tmp.magnitude)/ sum,1.0f));
			tmpUV.Add(new Vector2((curLen + tmp.magnitude)/ sum,0.2f));
			uvCount += 8;
			curLen += tmp.magnitude;
		}

		crawlerVertics = new Vector3[vertCount];
		for(int i = 0;i < vertCount;i++){
			crawlerVertics[i] = tmpVert[i];
		}
		crawlerTris = new int[trisCount];
		for(int i = 0;i < trisCount;i++){
			crawlerTris[i] = tmpTris[i];
		}
		crawlerUVs = new Vector2[uvCount];
		for(int i = 0;i < uvCount;i++){
			crawlerUVs[i] = tmpUV[i];
		}
	}
}
