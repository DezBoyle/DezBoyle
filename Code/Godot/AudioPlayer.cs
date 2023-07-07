using Godot;
using System;

/// <summary>
/// An Audio Player script that adds more functionality onto Godot's AudioStreamPlayer3D
/// Features : Multiple Sounds at a time
/// 		   A "PlayOneShot" method which is similar to Unity
/// 		   Random Sounds
/// </summary>
public partial class AudioPlayer : Node3D
{
	private AudioStreamPlayer3D currentAudio;

	public override void _EnterTree()
	{
		NewAudio();
	}

	public void PlayRandom(Godot.Collections.Array<AudioStream> streams, float volumeDb)
	{
		PlayOneShot(streams[Utility.RandomRange(0, streams.Count)], volumeDb);
	}

  	public void PlayOneShot(AudioStream stream, float volumeDb) { PlayOneShot(stream, volumeDb, 1f); }
	public void PlayOneShot(AudioStream stream, float volumeDb, float pitch)
	{
		currentAudio.Stream = stream;
		currentAudio.Play();
  		currentAudio3D.VolumeDb = volumeDb;
		currentAudio3D.MaxDb = volumeDb;
		currentAudio.PitchScale = pitch;
		currentAudio.Finished += currentAudio.QueueFree;
		NewAudio();
	}

	private void NewAudio()
	{
		currentAudio = new AudioStreamPlayer3D();
		AddChild(currentAudio);
	}
}
