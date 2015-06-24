using UnityEngine;
using System.Collections;

namespace Phone
{
	public abstract class App : MonoBehaviour
	{
		public enum Type { Texts, Mail, Contacts, Camera, Settings, Social, Games, Calc, Calendar, Clock };


		public App.Type type { get; protected set; }
		public Sprite loadingScreen;
		public Color loadingColor = Color.white;


		public bool YieldSaveControl()
		{
			if (SaveManager.Current != null)
			{
				SaveManager.Current.SaveAppProgress = UpdateSaveFile;
				return true;
			}

			Debug.Log(gameObject.name + " failed to yield save control.");
			return false;
		}
		protected abstract void LoadSaveFile();
		protected abstract void UpdateSaveFile();
	}
}
