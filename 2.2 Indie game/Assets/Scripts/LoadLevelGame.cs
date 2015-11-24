using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text.RegularExpressions;

public class LoadLevelGame : MonoBehaviour {
	public string levelName;
	
	/// <summary>
	/// The tileset, as an array.
	/// </summary>
	public GameObject[] tiles;

	List<GameObject> levelObjects = new List<GameObject>();

	int id= 0;

	int rotationX;
	int rotationY;
	int rotationZ;

	//int [,] array;

	int x=0;
	int z=0;

	int tileHeight= 0;

	public int height = 25;
	public int width = 25;
	
	//start loading the level at the start
	void Start () {
		//array = new int[width, height];
		LoadLevel();
	}
	
	public void LoadLevel() {

		//open xml
		XmlReader xmlReader = XmlReader.Create(Application.streamingAssetsPath + "/" + levelName + ".tmx");
		
		//keep reading until end-of-file
		while (xmlReader.Read()) {
		
			if (xmlReader.IsStartElement ()) {

				switch (xmlReader.Name) {

				case "tile":
					if (xmlReader ["id"] != null) {
						id = int.Parse (xmlReader ["id"]) -1;
                        if (id == -1) {
                            id = 0;
                        }
						Debug.Log (id);
					} else{
						int index = int.Parse(xmlReader["gid"])-1;
						InstatiateGameObject(xmlReader, index);
					}

					break;

				case "property":
					string attribute = xmlReader ["name"];

					Debug.Log (attribute);

					switch (attribute) {
					
					case "rotationX":
						rotationX = int.Parse (xmlReader ["value"]);
						Debug.Log (rotationX);

						break;

					case "rotationY":
						rotationY = int.Parse (xmlReader ["value"]);
						Debug.Log (rotationY);
						
						break;

					case "rotationZ":
						rotationZ = int.Parse (xmlReader ["value"]);
						Debug.Log (rotationZ);
						
						break;

					}

					tiles[id].transform.rotation = Quaternion.Euler(rotationX,rotationY,rotationZ);
					break;

				case "layer":
					tileHeight++;
					x=0;
					z=0;
					Debug.Log(x);
					break;
				
				}

			}


		}

		tileHeight = 0;
	}

	void InstatiateGameObject (XmlReader xmlReader, int gameObjectIndex) {
		if (int.Parse(xmlReader["gid"]) != 0){
            GameObject go = GameObject.Instantiate(tiles[gameObjectIndex], new Vector3((width - x) * 3.863f, tileHeight, z * 3.863f), tiles[gameObjectIndex].transform.rotation) as GameObject;
			go.transform.SetParent(this.transform);
			levelObjects.Add(go);
		}
		x++;
		if(x>width-1) {
			x=0;
			z++;
		}
	}

	public void DeleteScene () {
		foreach(GameObject levelObject in levelObjects){
			DestroyImmediate (levelObject);
		}
	}

}
