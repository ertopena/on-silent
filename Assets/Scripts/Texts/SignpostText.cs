using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Texts
{
	public class SignpostText : MonoBehaviour
	{
		public Color baseColor;


		private Text _text;
		private float changeDuration = 0.4f;


		void Awake()
		{
			_text = GetComponent<Text>();
		}


		public void SwitchToCharacter(Character.ID ch)
		{
			StopAllCoroutines();
			StartCoroutine(CoSwitchText(GetCharacterNickname(ch)));
		}


		public void Reset()
		{
			StopAllCoroutines();
			StartCoroutine(CoSwitchText("Chats"));
		}


		IEnumerator CoSwitchText(string newText)
		{
			int iterations = GetHalfIterations();


			for (int i = 0; i < iterations; i++)
			{
				_text.color = Color.Lerp(baseColor, Color.clear, (float)(i + 1) / (float)iterations);
				yield return new WaitForFixedUpdate();
			}


			_text.text = newText;


			for (int i = 0; i < iterations; i++)
			{
				_text.color = Color.Lerp(Color.clear, baseColor, (float)(i + 1) / (float)iterations);
				yield return new WaitForFixedUpdate();
			}


			yield return null;
		}


		int GetHalfIterations()
		{
			return (int)(changeDuration / Time.fixedDeltaTime) / 2;
		}


		string GetCharacterNickname(Character.ID ch)
		{
			return CharacterManager.GetCharacter(ch).firstName == "" ? CharacterManager.GetCharacter(ch).alias : CharacterManager.GetCharacter(ch).firstName;
		}
	}
}
