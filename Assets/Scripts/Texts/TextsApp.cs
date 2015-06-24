using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Texts
{
	public class TextsApp : Phone.App
	{
		public bool buildFromSaveFile = false;
		public RectTransform contactScrollView;
		public GameObject contactBoxPrefab;
		public ContactBox[] activeContactBoxes;
		public GameObject convoBuilderPrefab;
		public ConversationBuilder[] activeConvoBuilders;


		private List<Conversation> convosInProgress;


		#region Init and teardown

		void Start()
		{
			
			if (buildFromSaveFile)
				PopulateApp();
		}


		private void PopulateApp()
		{
			LoadSaveFile();
			GenerateConversations();
			GenerateContactBoxes();
		}


		private void GenerateConversations()
		{
			// Wipe the existing conversation windows.
			if (activeConvoBuilders != null && activeConvoBuilders.Length > 0)
			{
				foreach (ConversationBuilder cb in activeConvoBuilders)
				{
					if (cb != null)
						Destroy(cb.gameObject);
				}
			}


			// Create a new array to hold the new conversation windows.
			activeConvoBuilders = new ConversationBuilder[convosInProgress.Count];


			// For each convo tracked by the save file, ...
			for (int i = 0; i < convosInProgress.Count; i++)
			{
				// Instantiate a new ConversationBuilder window and add it to the array.
				activeConvoBuilders[i] = (Instantiate(convoBuilderPrefab, convoBuilderPrefab.transform.position, Quaternion.identity) as GameObject).GetComponent<ConversationBuilder>();


				// Set it to be the child of the TextsApp.
				activeConvoBuilders[i].transform.SetParent(transform, false);


				// Build it with the instructions on the save file.
				activeConvoBuilders[i].Build(convosInProgress[i]);
			}
		}


		private void GenerateContactBoxes()
		{
			// Wipe the existing contactBox buttons. We'll need new ones to link to the new conversation windows.
			if (activeContactBoxes != null && activeContactBoxes.Length > 0)
			{
				foreach (ContactBox cb in activeContactBoxes)
				{
					Destroy(cb.gameObject);
				}
			}
			activeContactBoxes = new ContactBox[activeConvoBuilders.Length];


			// Add a box per active conversation.
			for (int i = 0; i < activeContactBoxes.Length; i++)
			{
				activeContactBoxes[i] = (Instantiate(contactBoxPrefab, contactBoxPrefab.transform.position, Quaternion.identity) as GameObject).GetComponent<ContactBox>();
				activeContactBoxes[i].transform.SetParent(contactScrollView.transform, false);
				activeContactBoxes[i].Build(activeConvoBuilders[activeConvoBuilders.Length - 1 - i]);
			}
		}
		#endregion


		#region Displaying content

		public void LoadConversation(Character.ID contact)
		{
			// TODO!!
		}
		#endregion


		#region SaveFile operations

		protected override void LoadSaveFile()
		{
			/*
			if (SaveManager.Current != null)
			{
				convosInProgress = new Conversation[SaveManager.Current.saveFile.conversations.Count];
				SaveManager.Current.saveFile.conversations.CopyTo(convosInProgress, 0);
			}
			else
				Debug.LogError("SaveManager.Current is null. TextsApp could not complete LoadSaveFile()");
			*/
			if (SaveManager.Current != null)
				convosInProgress = SaveManager.Current.saveFile.conversations;
			else
			{
				Debug.LogError("SaveManager.Current is null. TextsApp could not complete LoadSaveFile()");
				convosInProgress = new List<Conversation>();
			}
		}


		protected override void UpdateSaveFile()
		{
			// Make a container for the conversations going on.
			List<Conversation> newConvos = new List<Conversation>();


			// Add each packaged conversation into the container.
			for (int i = 0; i < activeConvoBuilders.Length; i++)
			{
				newConvos.Add(activeConvoBuilders[i].ExportPackage());
			}


			// Change the saveFile's conversations to the temporary pointer.
			SaveManager.Current.saveFile.conversations = newConvos;
		}
		#endregion
	}
}
