using UnityEngine;
using System.Collections;

[System.Serializable]
public class Character
{
	public enum ID { Owner, Les, Jess, Rick, Bitch, Ellen, Mike, Burritos, KQAL, Office };


	public ID tag;
	public string firstName, lastName;
	public string alias;
	public Sprite picture;
	public long phoneNumber;
}


public class CharacterManager : MonoBehaviour {

	public static CharacterManager Current { get; private set; }


	public Character[] characters;


	void Awake()
	{
		if (Current == null)
		{
			Current = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else if (Current != this)
			Destroy(this.gameObject);
	}


	public static Character GetCharacter(Character.ID tag)
	{
		if (Current != null)
		{
			foreach (Character person in Current.characters)
			{
				if (person != null && person.tag == tag)
					return person;
			}
		}

		return null;
	}
}
