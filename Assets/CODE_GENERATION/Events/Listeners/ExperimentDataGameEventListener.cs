using UnityEngine;

namespace ScriptableObjectArchitecture
{
	[AddComponentMenu(SOArchitecture_Utility.EVENT_LISTENER_SUBMENU + "ExperimentData")]
	public sealed class ExperimentDataGameEventListener : BaseGameEventListener<ExperimentData, ExperimentDataGameEvent, ExperimentDataUnityEvent>
	{
	}
}