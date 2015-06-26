using UnityEngine;
using System.Collections;

namespace Fungus
{
	[EventHandlerInfo("Progression",
			"Image seen",
			"The block will execute when an ISeeable fires off an 'OnSeen' event.")]
	[AddComponentMenu("")]
	public class ItemSeen : EventHandler
	{
		[Tooltip("ISeeable to expect an event from.")]
		public ISeeable seeableObj;


		protected virtual void OnEnable()
		{
			seeableObj.OnSeen += DoEvent;
		}


		protected virtual void OnDisable()
		{
			seeableObj.OnSeen -= DoEvent;
		}


		private void DoEvent()
		{
			ExecuteBlock();
		}
	}
}
