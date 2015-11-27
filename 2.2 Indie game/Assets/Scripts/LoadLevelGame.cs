using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text.RegularExpressions;
using System;

public class LoadLevelGame : MonoBehaviour {
	public string levelName;
	
	/// <summary>
	/// The tileset, as an array.
	/// </summary>
	public GameObject[] tiles;
    Vector3[] rotations;

	List<GameObject> levelObjects = new List<GameObject>();

	int id;

	int rotationX;
	int rotationY;
	int rotationZ;

	int x=0;
	int z=0;

	int heightOffset= 0;

    [SerializeField]
	 int height = 25;
    [SerializeField]
	 int width = 25;
    [SerializeField]
    float tileWidth;
    [SerializeField]
    float tileHeight;

	//start loading the level at the start
	void Start () {
		LoadLevel();
	}
	
	public void LoadLevel() {

		//open xml
		XmlReader xmlReader = XmlReader.Create(Application.streamingAssetsPath + "/" + levelName + ".tmx");

        rotations = new Vector3[width*height];
 
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
                        rotations[id] = temp;
						break;
					}
					break;

				case "layer":
					heightOffset++;
					x=0;
					z=0;
					break;
				
				}
			}
		}

		heightOffset = 0;
	}

	void InstatiateGameObject (XmlReader xmlReader, int gameObjectIndex) {
		if (gameObjectIndex != 0){
            if (tiles[gameObjectIndex] != null)
            {
                Quaternion rot = rotations[gameObjectIndex] != null ? Quaternion.Euler(rotations[gameObjectIndex]) : Quaternion.Euler(0,0,0) ;
             
                GameObject go = GameObject.Instantiate(tiles[gameObjectIndex], new Vector3((width - x) * tileWidth, heightOffset * tileHeight, z * tileWidth), rot) as GameObject;
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
        rotations = new Vector3[0];
         id = 0;

        rotationX=0;
        rotationY=0;
        rotationZ=0;

        x = 0;
       z = 0;

        heightOffset = 0;

		foreach(GameObject levelObject in levelObjects){
			DestroyImmediate (levelObject);
		}
        levelObjects.Clear(); 
	}

}
