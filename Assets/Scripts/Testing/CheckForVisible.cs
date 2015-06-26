using UnityEngine;
using System.Collections;

namespace Testing
{
	/*
	 * Testing this script on UI elements revealed that the OnBecameVisible event does not fire when
	 * UI elements enter the screen. DO NOT USE.
	 */ 
	
	
	public class CheckForVisible : MonoBehaviour, ISeeable
	{
		public event Phone.Controller.GameEvent OnSeen;
		public bool hasBeenSeen = false;


		void OnBecameVisible()
		{
			hasBeenSeen = true;
			
			if (OnSeen != null)
				OnSeen();
		}
	}
}
