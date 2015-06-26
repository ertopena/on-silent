using UnityEngine;
using System.Collections;

public interface ISeeable
{
	event Phone.Controller.GameEvent OnSeen;
}
