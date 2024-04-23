using UnityEngine;

namespace ScriptableObjectArchitecture
{
	[System.Serializable]
	public sealed class ExperimentDataReference : BaseReference<ExperimentData, ExperimentDataVariable>
	{
	    public ExperimentDataReference() : base() { }
	    public ExperimentDataReference(ExperimentData value) : base(value) { }
	}
}