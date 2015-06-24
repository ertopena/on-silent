using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Texts
{
	[System.Serializable]
	public class Conversation
	{
		public Character.ID characterID;
		[XmlArray("msgBuilders")]
		[XmlArrayItem("msg")]
		public List<TextMessage> textMessages = new List<TextMessage>();
	}

 
	public class ConversationBuilder : MonoBehaviour
	{
		public Character.ID characterID;
		public GameObject msgScrollView;
		public GameObject ownerMsgPrefab, contactMsgPrefab;


		public void Build(Conversation conv)
		{
			characterID = conv.characterID;


			// Generate new msgBuilders one by one.
			for(int i = 0; i < conv.textMessages.Count; i++)
			{
				AddMessage(conv.textMessages[i], false);
			}


			ResizeScrollView();
		}


		public Conversation ExportPackage()
		{
			// Create a container for the list of text messages the player has seen.
			List<TextMessage> texts = new List<TextMessage>();


			// Get an array of TextMessageBuilders to start packing up.
			TextMessageBuilder[] msgBuilders = msgScrollView.GetComponentsInChildren<TextMessageBuilder>();


			// Add the TextMessage summary of each TextMessageBuilder to the list and sort it.
			for (int i = 0; i < msgBuilders.Length; i++)
			{
				texts.Add(msgBuilders[i].ExportPackage());
			}
			texts.Sort();


			// Package the msgBuilders into a Conversation.
			Conversation conv = new Conversation();
			conv.characterID = characterID;
			conv.textMessages = texts;


			return conv;
		}


		public TextMessage GetLastText()
		{
			TextMessage[] m = ExportPackage().textMessages.ToArray();
			return m[m.Length - 1];
		}


		void AddMessage(TextMessage msg, bool resizeScrollView)
		{
			// Instantiate the correct prefab and make it a child of the message scrollview.
			GameObject prefab = msg.characterID == Character.ID.Les ? ownerMsgPrefab : contactMsgPrefab;
			GameObject box = Instantiate(prefab, prefab.transform.position, Quaternion.identity) as GameObject;
			box.transform.SetParent(msgScrollView.transform, false);


			// Fill in the fields for the new text message.
			box.GetComponent<TextMessageBuilder>().Build(msg);


			if (resizeScrollView)
				ResizeScrollView();
		}
		void AddMessage(TextMessage msg)
		{
			// If no resize flag is included, resize just to be safe.
			AddMessage(msg, true);
		}


		void WipeMessages()
		{
			Transform[] msgChildren = msgScrollView.GetComponentsInChildren<Transform>();


			foreach (Transform t in msgChildren)
			{
				if (t != null)
					Destroy(t.gameObject);
			}
		}


		void ResizeScrollView()
		{
			// Create a container for the new height.
			float newY = 0;
			
			
			// Get an array of the text message boxes that are children of the msgScrollView.
			TextMessageBuilder[] msgBuilders = msgScrollView.GetComponentsInChildren<TextMessageBuilder>();


			// For each item in the array, add the height of the box.
			foreach(TextMessageBuilder tmb in msgBuilders)
			{
				newY += tmb.GetComponent<RectTransform>().sizeDelta.y;
			}


			// Add the spacing in between boxes and the top and bottom padding.
			VerticalLayoutGroup layout = msgScrollView.GetComponent<VerticalLayoutGroup>();
			if (msgBuilders.Length > 0)
				newY += layout.spacing * (msgBuilders.Length - 1);
			newY += layout.padding.top;
			newY += layout.padding.bottom;


			// Set scrollView's RectTransform to the desired size.
			msgScrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(msgScrollView.GetComponent<RectTransform>().sizeDelta.x, newY);
		}
	}
}
