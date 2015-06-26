using UnityEngine;
using System.Collections;

namespace Phone
{
	public class HotCorner : MonoBehaviour
	{
		public enum Type { Back, Settings, Pause, Notifications };

		
		public delegate void HotCornerEvent();
		public static event HotCornerEvent OnBack;


		public Type type;

		
		public void Press()
		{
			switch (type)
			{
				case Type.Back:
					if (OnBack != null)
						OnBack();
					break;
				default:
					break;
			}
		}
	}
}
