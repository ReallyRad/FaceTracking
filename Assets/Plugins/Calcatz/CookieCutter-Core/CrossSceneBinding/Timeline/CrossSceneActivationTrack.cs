#if TIMELINE_AVAILABLE
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Calcatz.CookieCutter {
	/// <summary>
	/// Track that can be used to control the active state of a GameObject.
	/// </summary>
	[Serializable]
	[TrackClipType(typeof(CrossSceneActivationPlayableAsset))]
	//[TrackBindingType(typeof(GameObject))]
	[TrackColor(0.25f, 0.9f, 0.25f)]
	public class CrossSceneActivationTrack : TrackAsset {

		[SerializeField] private UnityEngine.Object m_binder;
		[SerializeField] private PostPlaybackState m_PostPlaybackState = PostPlaybackState.LeaveAsIs;


		private CrossSceneActivationMixerPlayable m_ActivationMixer;

		/// <summary>
		/// Specify what state to leave the GameObject in after the Timeline has finished playing.
		/// </summary>
		public enum PostPlaybackState {
			/// <summary>
			/// Set the GameObject to active.
			/// </summary>
			Active,

			/// <summary>
			/// Set the GameObject to Inactive.
			/// </summary>
			Inactive,

			/// <summary>
			/// Revert the GameObject to the state in was in before the Timeline was playing.
			/// </summary>
			Revert,

			/// <summary>
			/// Leave the GameObject in the state it was when the Timeline was stopped.
			/// </summary>
			LeaveAsIs
		}

		/// <summary>
		/// Specifies what state to leave the GameObject in after the Timeline has finished playing.
		/// </summary>
		public PostPlaybackState postPlaybackState {
			get { return m_PostPlaybackState; }
			set { m_PostPlaybackState = value; UpdateTrackMode(); }
		}

		/// <inheritdoc/>
		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
			var mixer = CrossSceneActivationMixerPlayable.Create(graph, inputCount);
			m_ActivationMixer = mixer.GetBehaviour();

			UpdateTrackMode();

			return mixer;
		}

		internal void UpdateTrackMode() {
			if (m_ActivationMixer != null) {
				m_ActivationMixer.postPlaybackState = m_PostPlaybackState;
				m_ActivationMixer.binder = m_binder;
			}
		}

		/// <inheritdoc/>
		public override void GatherProperties(PlayableDirector director, IPropertyCollector driver) {
			if (m_binder != null) {
				var gameObject = (CrossSceneBindingUtility.GetObject(m_binder) as Component).gameObject;
				director.SetGenericBinding(this, gameObject);
				if (gameObject != null) {
					driver.AddFromName(gameObject, "m_IsActive");
				}
			}
		}

		/// <inheritdoc/>
		protected override void OnCreateClip(TimelineClip clip) {
			clip.displayName = "Active";
			base.OnCreateClip(clip);
		}
	}
}
#endif