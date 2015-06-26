using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace Texts
{
	/*
	 * TextMessageBuilder is a Monobehaviour attached to each box representing a text message in the Texts app. It
	 * contains references used to fill in the various fields when the Texts controller is populating each conversation
	 * when the game loads.
	 * 
	 * The Xml-serializable TextMessage class is a container for the information to build each Text Message box
	 * in each conversation within the Texts app.
	 * 
	 * The TextMessage objects will be taken from the Conversations portion of the Xml-formatted save file. See the
	 * SaveManager and ConversationBuilder classes for more information.
	 */
 
	[System.Serializable]
	public class TextMessage : System.IComparable<TextMessage>
	{
		public Character.ID characterID;			// To quickly find the profile pic of the sender from CharacterManager.Current.
		public string content;
		public Phone.Timestamp timestamp;


		// Compares the text messages by date.
		public int CompareTo(TextMessage other)
		{
			if (other == null)
				return 1;
			
			return this.timestamp.CompareTo(other.timestamp);
		}
	}

	
	public class TextMessageBuilder : MonoBehaviour
	{
		public Character.ID characterID;
		public Text msgContentText;
		public Image profileImage;
		public Text timestampText;
		public Phone.Timestamp timestamp;


		public void Build(TextMessage msg)
		{
			characterID = msg.characterID;
			msgContentText.text = msg.content;
			profileImage.sprite = CharacterManager.GetCharacter(msg.characterID).picture;
			timestamp = msg.timestamp;
			timestampText.text = GetTimestampString();
		}


		public TextMessage ExportPackage()
		{
			TextMessage msg = new TextMessage();

			msg.characterID = characterID;
			msg.content = msgContentText.text;
			msg.timestamp = timestamp;

			return msg;
		}


		string GetTimestampString(Phone.Timestamp ts)
		{
			return ts.month.ToString() + @"/" + ts.day.ToString("00") + System.Environment.NewLine
				+ ts.hours.ToString() + @":" + ts.minutes.ToString("00");
		}
		string GetTimestampString(TextMessage msg)
		{
			return GetTimestampString(msg.timestamp);
		}
		string GetTimestampString()
		{
			return GetTimestampString(timestamp);
		}
	}
}
