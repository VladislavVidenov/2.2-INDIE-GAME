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
        
		LoadLevel();
	}
	
	public void LoadLevel() {

		//open xml
		XmlReader xmlReader = XmlReader.Create(Application.streamingAssetsPath + "/" + levelName + ".tmx");

        rotations = new List<Vector3>();
        for (int i = 0; i < width * height; i++) {
            rotations.Add(new Vector3(0, 0, 0));
        }
		
		//keep reading until end-of-file
		while (xmlReader.Read()) {
		
			if (xmlReader.IsStartElement ()) {

				switch (xmlReader.Name) {

				case "tile":
					if (xmlReader ["id"] != null) {
						id = int.Parse (xmlReader ["id"]);
					} else{
						int index = int.Parse(xmlReader["gid"]) -1 ;
                        if (index == -1) {
                            index = 0;
                        }
						InstatiateGameObject(xmlReader, index);
					}

					break;

				case "property":
					string attribute = xmlReader ["name"];

					switch (attribute) {
					
					case "rotationX":
						rotationX = int.Parse (xmlReader ["value"]);
				

						break;

					case "rotationY":
						rotationY = int.Parse (xmlReader ["value"]);
			
						
						break;

					case "rotationZ":
						rotationZ = int.Parse (xmlReader ["value"]);
			

                        Vector3 temp = new Vector3(rotationX, rotationY, rotationZ);
                        rotations.Insert(id,temp);
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
		if (gameObjectIndex != 0){
            if (tiles[gameObjectIndex] != null)
            {
                Quaternion rot = rotations[gameObjectIndex] != null ? Quaternion.Euler(rotations[gameObjectIndex]) : Quaternion.Euler(0,0,0) ;
             
                GameObject go = GameObject.Instantiate(tiles[gameObjectIndex], new Vector3((width - x) * 3.863f, tileHeight * 2.4938f, z * 3.863f), rot) as GameObject;
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
         id = 0;

        rotationX=0;
        rotationY=0;
        rotationZ=0;

        x = 0;
       z = 0;

        tileHeight = 0;

		foreach(GameObject levelObject in levelObjects){
			DestroyImmediate (levelObject);
		}
        levelObjects.Clear(); 
	}

}
