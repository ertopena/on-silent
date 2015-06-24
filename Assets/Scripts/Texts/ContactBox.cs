using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Texts
{
	public class ContactBox : MonoBehaviour
	{
		public Character.ID characterID;
		public Text aliasText, latestMessageText;
		public Image profilePic;


		public void Build(ConversationBuilder cb)
		{
			characterID = cb.characterID;
			SetAliasText();
			SetLastMessageText(cb);
			SetProfilePic();
		}


		public void LoadConversation()
		{
			GetComponentInParent<TextsApp>().LoadConversation(characterID);
		}


		void SetAliasText()
		{
			if (aliasText != null)
			{
				Character contact = CharacterManager.GetCharacter(characterID);
				string alias;


				if (contact.firstName == "" && contact.lastName == "")
					alias = contact.alias;
				else
					alias = contact.firstName + " " + contact.lastName;


				aliasText.text = alias;
			}
		}


		void SetLastMessageText(ConversationBuilder cb)
		{
			if (latestMessageText != null)
				latestMessageText.text = cb.GetLastText().content;
		}


		void SetProfilePic()
		{
			if (profilePic != null)
			{
				profilePic.sprite = CharacterManager.GetCharacter(characterID).picture;
			}
		}
	}
}
