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
    List<Vector3> rotations;

	List<GameObject> levelObjects = new List<GameObject>();

	int id= 0;

	int rotationX;
	int rotationY;
	int rotationZ;

	int x=0;
	int z=0;

	int tileHeight= 0;

	public int height = 25;
	public int width = 25;
	
	//start loading the level at the start
	void Start () {
        rotations = new List<Vector3>();
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
					} else{
						int index = int.Parse(xmlReader["gid"])-1;
						InstatiateGameObject(xmlReader, index);
					}

					break;

				case "property":
					string attribute = xmlReader ["name"];

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

                        Vector3 temp = new Vector3(rotationX, rotationY, rotationZ);
                        rotations.Add(temp);
						
						break;

					}
                        
					break;

				case "layer":
					tileHeight++;
					x=0;
					z=0;
					break;
				
				}

			}


		}

		tileHeight = 0;
	}

	void InstatiateGameObject (XmlReader xmlReader, int gameObjectIndex) {
		if (int.Parse(xmlReader["gid"]) != 0){
            Debug.Log(rotations[gameObjectIndex]);
            if (tiles[gameObjectIndex] != null)
            {
                GameObject go = GameObject.Instantiate(tiles[gameObjectIndex], new Vector3((width - x) * 3.863f, tileHeight * 2.4938f, z * 3.863f), Quaternion.Euler(rotations[gameObjectIndex])) as GameObject;
                //  go.transform.rotation = new Quaternion rotations[gameObjectIndex];
                go.transform.SetParent(this.transform);
                levelObjects.Add(go);
            }
		}
		x++;
		if(x>width-1) {
			x=0;
			z++;
		}
	}

	public void DeleteScene () {
        rotations.Clear();
		foreach(GameObject levelObject in levelObjects){
			DestroyImmediate (levelObject);
		}
	}

}
