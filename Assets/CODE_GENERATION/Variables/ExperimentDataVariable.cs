using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjectArchitecture
{
	[System.Serializable]
	public class ExperimentDataEvent : UnityEvent<ExperimentData> { }

	[CreateAssetMenu(
	    fileName = "ExperimentDataVariable.asset",
	    menuName = SOArchitecture_Utility.VARIABLE_SUBMENU + "ExperimentData",
	    order = 120)]
	public class ExperimentDataVariable : BaseVariable<ExperimentData, ExperimentDataEvent>
	{
	}
}