using UnityEngine;

namespace ScriptableObjectArchitecture
{
	[System.Serializable]
	[CreateAssetMenu(
	    fileName = "ExperimentDataGameEvent.asset",
	    menuName = SOArchitecture_Utility.GAME_EVENT + "ExperimentData",
	    order = 120)]
	public sealed class ExperimentDataGameEvent : GameEventBase<ExperimentData>
	{
	}
}