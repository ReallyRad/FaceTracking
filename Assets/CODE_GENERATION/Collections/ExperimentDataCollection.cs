using UnityEngine;

namespace ScriptableObjectArchitecture
{
	[CreateAssetMenu(
	    fileName = "ExperimentDataCollection.asset",
	    menuName = SOArchitecture_Utility.COLLECTION_SUBMENU + "ExperimentData",
	    order = 120)]
	public class ExperimentDataCollection : Collection<ExperimentData>
	{
	}
}