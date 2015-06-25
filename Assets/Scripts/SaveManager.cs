using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

#region SaveFile class

[System.Serializable]
[XmlRoot("savefile")]
public class SaveFile
{
	[XmlArray("conversations")]
	[XmlArrayItem("texts")]
	public List<Texts.Conversation> conversations = new List<Texts.Conversation>();			// Holds only info to which the player has already had access.


	public void Save(string path)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(SaveFile));
		FileStream stream = new FileStream(path, FileMode.Create);

		serializer.Serialize(stream, this);

		stream.Close();
	}


	public static SaveFile Load(string path)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(SaveFile));
		FileStream stream = new FileStream(path, FileMode.Open);

		SaveFile container = serializer.Deserialize(stream) as SaveFile;

		stream.Close();

		return container;
	}
}
#endregion


public class SaveManager : MonoBehaviour
{
	public delegate void SaveDelegate();
	public SaveDelegate SaveAppProgress;
	
	
	public static SaveManager Current { get; private set; }


	public SaveFile saveFile;		// TODO: Once we have a blankSaveFile prepared, make this {get; private set;}
	public bool allowSaveTemplateEdits = false;


	Phone.Controller gameController;
	string savePath, blankSavePath;


	#region Init and teardown

	void Awake()
	{
		if (Current == null)
		{
			Current = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else if (Current != this)
			Destroy(this.gameObject);


#if UNITY_EDITOR
		savePath = Path.Combine(Application.dataPath, "Resources/save.xml");
#else
		savePath = Path.Combine(Application.persistentDataPath, "save.xml");
#endif
		blankSavePath = Path.Combine(Application.dataPath, "Resources/blankSave.xml");


		InitSaveFile();
	}


	void OnEnable()
	{
		Phone.Controller.OnControllerStart += AddGameController;
	}


	void OnDisable()
	{
		Phone.Controller.OnControllerStart -= AddGameController;
	}


	void InitSaveFile()
	{
		if (File.Exists(savePath))
			saveFile = SaveFile.Load(savePath);
		else if (File.Exists(blankSavePath))
			saveFile = SaveFile.Load(blankSavePath);
		else
		{
			Debug.Log("SaveManager found no save.xml and no blankSave.xml - saveFile is the one set at Editor time.");
		}
	}


	void AddGameController(Phone.Controller gc)
	{
		gameController = gc;
	}
	void AddGameController()
	{
		Phone.Controller gc = FindObjectOfType<Phone.Controller>();
		if (gc != null)
			AddGameController(gc);
	}
	#endregion


	void Update()
	{
		if (allowSaveTemplateEdits)
			CheckToEditSaveTemplates();
	}


	#region Runtime save file manipulation

	public void Save(string path) { saveFile.Save(path); }
	public void Save() { Save(savePath); }
	public void Save(Phone.App app)
	{
		if (app.YieldSaveControl())
		{
			SaveAppProgress();
			Save();
		}
		else
			Debug.LogError(app.type.ToString() + " failed to yield save control.");
	}
	#endregion


	#region Editor-time save template manipulation

	// Reads inputs to see whether to change the save file and/or the blank save file.
	void CheckToEditSaveTemplates()
	{
		if (Input.GetKeyUp(KeyCode.Alpha0))
			WipeSaveFile();

		if (Input.GetKeyUp(KeyCode.N))
			CreateNewBlankSaveFile();

		if (Input.GetKeyUp(KeyCode.R))
			ResetSaveFile();

		if (Input.GetKeyUp(KeyCode.S))
			CreateTestSaveFile();
	}


	// Deletes the game progress saved to persistentDataPath.
	void WipeSaveFile()
	{
		if (File.Exists(savePath))
		{
			File.Delete(savePath);
			Debug.Log("saveFile.xml wiped.");
		}
		else
			Debug.Log("saveFile.xml was not found in the savePath.");
	}


	// Saves the game state information in a new blank save file to be used when starting a new game in the production env.
	void CreateNewBlankSaveFile()
	{
		PopulateSaveFile();
		Save(blankSavePath);
	}


	void CreateTestSaveFile()
	{
		PopulateSaveFile();
		Save();
	}


	void PopulateSaveFile()
	{
		if (gameController == null)
			AddGameController();


		if (gameController != null)
		{
			saveFile.conversations = gameController.texts.GetConvosInProgress();
		}
		else
			Debug.LogError("SaveManager could not get a Phone.Controller script to update the saveFile.");
	}


	// Replaces the game progress saved to persistentDataPath with a blank save file (if it exists).
	void ResetSaveFile()
	{
		if (File.Exists(blankSavePath))
		{
			saveFile = SaveFile.Load(blankSavePath);
			saveFile.Save(savePath);
		}
		else
			Debug.Log("blankSaveFile.xml not found. Could not ResetSaveFile().");
	}
	#endregion
}
