using UnityEngine;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

public class GeneratePoints : MonoBehaviour {
	
	public GameObject PointPrefab;
	public bool autoRotate = true;
	public enum _cubeDisplay {Outline, Glass, None};
	public _cubeDisplay cubeDisplay;
	public bool generateRandomData = false;
	public int numPoints = 100;
	public string _filePath = Application.streamingAssetsPath;
	public string fileName = "test0.csv";
	public bool autoDetectDataRange = true;
	public bool normalizeAxesIndividually = true;
	public float minX = 0;
	public float maxX = 1;
	public float minY = 0;
	public float maxY = 1;
	public float minZ = 0;
	public float maxZ = 1;
	
	public Material outlineMaterial;
	public Material glassMaterial;
	
	private ArrayList points;
	private string filePath;
	
	//GUI state vars
	private string notificationMessage = "";
	
	// Use this for initialization
	void Start () {
		//init vars
		filePath = System.IO.Path.Combine(_filePath, fileName);
		points = new ArrayList();
		
		//set material
		switch (cubeDisplay) {
			case _cubeDisplay.Outline:
				renderer.material = outlineMaterial;
				break;
			case _cubeDisplay.Glass:
				renderer.material = glassMaterial;
				break;
			case _cubeDisplay.None:
				renderer.enabled = false;
				break;
		}	
		
		
		if (generateRandomData) {
			//generate some dummy data
			for (int i = 0; i < numPoints; i++) {
				float x = Random.value; //0 (inclusive) to 1 (inclusive)
				float y = Random.value;
				float z = Random.value;
				points.Add (new Vector3(x, y, z));	
			}
		} else {
			bool hasHeaders = false;
			float autoMinX = float.PositiveInfinity;
			float autoMaxX = float.NegativeInfinity;
			float autoMinY = float.PositiveInfinity;
			float autoMaxY = float.NegativeInfinity;
			float autoMinZ = float.PositiveInfinity;
			float autoMaxZ = float.NegativeInfinity;
			
			//load data from file path 
			try {
				string[] lines = File.ReadAllLines(filePath);
	
				if (lines.Length > 0) {
					//check for headers
					Regex rgx = new Regex(@"[^\d\.,]"); //search for any non-digit or . , chars 
					hasHeaders = rgx.IsMatch(lines[0]);
					
					//assume that data is in x,y,z format
					int ii = hasHeaders ? 1 : 0;
					for(int i = ii; i < lines.Length; i++) {
						string s = lines[i];
						string[] values = s.Split(',');
						//add data point
						float x = float.Parse(values[0]);
						float y = float.Parse(values[1]);
						float z = float.Parse(values[2]) / 3;
						//update min and max values
						if (x < autoMinX) autoMinX = x;
						if (x > autoMaxX) autoMaxX = x;
						if (y < autoMinY) autoMinY = y;
						if (y > autoMaxY) autoMaxY = y;
						if (z < autoMinZ) autoMinZ = z;
						if (z > autoMaxZ) autoMaxZ = z;
						//add to point ArrayList
						points.Add (new Vector3(x, y, z));	
					}
					//set range
					if (autoDetectDataRange) {
						minX = autoMinX;
						maxX = autoMaxX;
						minY = autoMinY;
						maxY = autoMaxY;
						minZ = autoMinZ;
						maxZ = autoMaxZ;
						//set all ranges to largest min and max if normalize individually is off
						if (!normalizeAxesIndividually) {
							float min = minX;
							if (minY < min) min = minY;
							if (minZ < min) min = minZ;
							float max = maxX;
							if (maxY > max) max = maxY;
							if (maxZ > max) max = maxZ;
							minX = min;
							minY = min;
							minZ = min;
							maxX = max;
							maxY = max;
							maxZ = max;
						}
						//notify of computed range
						string ss = "Points were normalized to the following ranges (disable Auto Detect Data Range to override)\n";
						ss += "x: (" + minX + ", " + maxX + ")\n";
						ss += "y: (" + minY + ", " + maxY + ")\n";
						ss += "z: (" + minZ + ", " + maxZ + ")\n";
						//showNotification(ss);
					}
				} else {
					showNotification("Error loading file. No lines detected.");
				}
			} catch (FileNotFoundException f) {
				showNotification(f.Message);		
			}
		}
		
		//Draw points
		for (int i = 0; i < points.Count; i++) {
			//Normalize Point to -0.5 to 0.5 range
			Vector3 point = (Vector3) points[i];
			point.x = ((point.x - minX) / (maxX - minX)) - 0.5f;
			point.y = ((point.y - minY) / (maxY - minY)) - 0.5f;
			point.z = ((point.z - minZ) / (maxZ - minZ)) - 0.5f;
			//Draw Point
			GameObject go = (GameObject) Instantiate (PointPrefab);
			go.transform.parent = this.transform;
			go.transform.position = point;
			float red = (point.z + 0.50f);
			float green = 1f - (point.z + 0.50f);
			Debug.Log (red + ", " + green);
			go.renderer.material.color = new Color(red, green, 0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	/*void OnGUI () {
		if (!notificationMessage.Equals("")) {
			// Make a background box
			int width = 1000;
			int height = 300;
			int left = (Screen.width - width) / 2;
			int top = (Screen.height - height) / 2;
			int butWidth = 80;
			int butHeight = 20;
			int butLeft = left + width / 2 - butWidth / 2;
			int butTop = top + height - butHeight - 10;
			GUI.Box(new Rect(left,top,width,height), notificationMessage);
			if(GUI.Button(new Rect(butLeft,butTop,butWidth,butHeight), "OK")) {
				closeNotification();
			}
		}
	}*/
	
	public void showNotification(string s) {
		//notificationMessage = s;
		Debug.Log (s);
	}
	public void closeNotification() {
		notificationMessage = "";	
	}
}
