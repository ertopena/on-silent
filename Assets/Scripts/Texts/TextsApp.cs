using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Texts
{
	public class TextsApp : Phone.App
	{
		public event Phone.Controller.GameEvent OnAnimStart;
		public event Phone.Controller.GameEvent OnAnimEnd;

		
		public bool buildFromSaveFile = false;
		public RectTransform contactScrollView;
		public GameObject contactBoxPrefab;
		public ContactBox[] activeContactBoxes;
		public RectTransform convoHolder;
		public GameObject convoBuilderPrefab;
		public ConversationBuilder[] activeConvoBuilders;
		public SignpostText signpostText;
		public RectTransform convoRenderingArea;
		public bool convoLoaded { get; private set; }
		public bool inputAreaDeployed { get; private set; }


		private Phone.Controller controller;
		private List<Conversation> convosInProgress;
		private float slideDuration = 0.4f;
		private Vector2 convoAreaHiddenPos, convoAreaVisiblePos;
		//Turn these on in Awake() and CoToggleInputArea(bool shouldDeploy)
		private Vector2 convoHolderFullSize, convoHolderSizeWithKeyboard;


		#region Init and teardown

		void Awake()
		{
			convoLoaded = false;
			inputAreaDeployed = false;


			convoAreaHiddenPos = convoRenderingArea.anchoredPosition;
			convoAreaVisiblePos = new Vector2(convoAreaHiddenPos.x, 0f);

			convoHolderFullSize = convoHolder.sizeDelta;
			convoHolderSizeWithKeyboard = new Vector2(convoHolderFullSize.x, 310f);
		}
		
		
		void OnEnable()
		{
			Phone.HotCorner.OnBack += GoBack;
		}


		void OnDisable()
		{
			Phone.HotCorner.OnBack -= GoBack;
		}


		void Start()
		{
			controller = GetComponentInParent<Phone.Controller>();


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


			// Arrange the conversations by date of the most recent message.
			convosInProgress.Sort();


			// For each convo tracked by the save file, ...
			for (int i = 0; i < convosInProgress.Count; i++)
			{
				// Instantiate a new ConversationBuilder window and add it to the array.
				activeConvoBuilders[i] = (Instantiate(convoBuilderPrefab, convoBuilderPrefab.transform.position, Quaternion.identity) as GameObject).GetComponent<ConversationBuilder>();


				// Set it to be the child of the TextsApp.
				activeConvoBuilders[i].transform.SetParent(convoHolder.transform, false);


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
			if (controller.AllowingInput)
				StartCoroutine(CoLoadConvo(contact));
		}


		IEnumerator CoLoadConvo(Character.ID contact)
		{
			if (OnAnimStart != null)
				OnAnimStart();


			// Get the signpost text switching to the nickname of the contact.
			signpostText.SwitchToCharacter(contact);


			// Calculate the iterations to slide the conversation into place.
			int iterations = GetSlideIterations();


			// Find the end position.
			Vector2 endPos = new Vector2(0f, convoHolder.anchoredPosition.y);

			
			// Turn off all conversations except for the one we want.
			for (int i = 0; i < activeConvoBuilders.Length; i++)
			{
				if (activeConvoBuilders[i].characterID == contact)
					activeConvoBuilders[i].gameObject.SetActive(true);
				else
					activeConvoBuilders[i].gameObject.SetActive(false);
			}


			// Slide the conversation holder into place.
			for (int i = 0; i < iterations; i++)
			{
				convoHolder.anchoredPosition = Vector2.Lerp(convoHolder.anchoredPosition, endPos, 0.25f);
				yield return new WaitForFixedUpdate();
			}


			// Make sure that the conversation holder reaches its final position.
			convoHolder.anchoredPosition = endPos;


			convoLoaded = true;
			if (OnAnimEnd != null)
				OnAnimEnd();


			yield return null;
		}


		public void UnloadConversation()
		{
			if (controller.AllowingInput)
				StartCoroutine(CoUnloadConvo());
		}


		IEnumerator CoUnloadConvo()
		{
			if (OnAnimStart != null)
				OnAnimStart();


			// Get the signpost text switching back to its default title.
			signpostText.Reset();


			// Calculate the iterations.
			int iterations = GetSlideIterations();


			// Find the end position for the transition.
			Vector2 endPos = new Vector2(convoHolder.sizeDelta.x, convoHolder.anchoredPosition.y);


			// Slide the conversation holder.
			for (int i = 0; i < iterations; i++)
			{
				convoHolder.anchoredPosition = Vector2.Lerp(convoHolder.anchoredPosition, endPos, 0.25f);
				yield return new WaitForFixedUpdate();
			}


			// Make sure the conversation holder reaches the final position.
			convoHolder.anchoredPosition = endPos;


			convoLoaded = false;
			if (OnAnimEnd != null)
				OnAnimEnd();


			yield return null;
		}
		#endregion


		#region Deploying text input area

		public void PressNewText()
		{
			if (controller.AllowingInput)
			{
				StopAllCoroutines();


				if (convoLoaded && !inputAreaDeployed)
				{
					StartCoroutine(CoToggleInputArea(true));
				}
				else
				{
					// StartCoroutine(CoShowMessageCreationScreen());
				}
			}
		}


		public void PressTextMessage(TextMessageButton button)
		{
			if (controller.AllowingInput)
			{
				if (inputAreaDeployed)
					StartCoroutine(CoToggleInputArea(false));
			}
		}


		IEnumerator CoToggleInputArea(bool shouldDeploy)
		{
			if (OnAnimStart != null)
				OnAnimStart();


			contactScrollView.gameObject.SetActive(false);


			Vector2 targetPos = shouldDeploy ? convoAreaVisiblePos : convoAreaHiddenPos;
			Vector2 targetSize = shouldDeploy ? convoHolderSizeWithKeyboard : convoHolderFullSize;


			int iterations = GetSlideIterations();


			for (int i = 0; i < iterations; i++)
			{
				convoRenderingArea.anchoredPosition = Vector2.Lerp(convoRenderingArea.anchoredPosition, targetPos, 0.25f);
				convoHolder.sizeDelta = Vector2.Lerp(convoHolder.sizeDelta, targetSize, 0.3f);
				yield return new WaitForFixedUpdate();
			}


			convoRenderingArea.anchoredPosition = targetPos;
			convoHolder.sizeDelta = targetSize;


			contactScrollView.gameObject.SetActive(true);


			inputAreaDeployed = shouldDeploy;
			if (OnAnimEnd != null)
				OnAnimEnd();
		}
		#endregion


		#region HotCorner events

		public void GoBack()
		{
			if (controller.AllowingInput)
			{
				if (convoLoaded)
				{
					if (inputAreaDeployed)
						StartCoroutine(CoToggleInputArea(false));
					else
						UnloadConversation();
				}
				
				// TODO: else, go back to the home screen.
				// Also check that we are not waiting for a response from the player.
			}
		}
		#endregion


		#region SaveFile operations

		public List<Conversation> GetConvosInProgress()
		{
			// Make a container for the conversations going on.
			List<Conversation> newConvos = new List<Conversation>();


			// Add each packaged conversation into the container.
			for (int i = 0; i < activeConvoBuilders.Length; i++)
			{
				newConvos.Add(activeConvoBuilders[i].ExportPackage());
			}


			return newConvos;
		}

		
		protected override void LoadSaveFile()
		{
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
			SaveManager.Current.saveFile.conversations = GetConvosInProgress();
		}
		#endregion


		#region Utilities

		int GetSlideIterations()
		{
			return (int)(slideDuration / Time.fixedDeltaTime);
		}
		#endregion
	}
}
