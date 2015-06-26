using UnityEngine;
using System.Collections;

namespace Texts
{
	public class TextMessageButton : MonoBehaviour
	{
		public void Press()
		{
			TextsApp app = GetComponentInParent<TextsApp>();


			if (app != null)
				app.PressTextMessage(this);
		}
	}
}
