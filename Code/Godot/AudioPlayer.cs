using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// An Audio Player script that adds more functionality onto Godot's AudioStreamPlayer3D
/// Features : Multiple Sounds at a time
/// 		   A "PlayOneShot" method which is similar to Unity
/// 		   Random Sounds
/// </summary>
[GlobalClass]
public partial class AudioPlayer : Node3D
{
	[Export] public AudioStream ClipToPlayOnAwake = null;
	[Export] private float clipToPlayOnAwakeVolume = 0f;
	[Export] private float clipToPlayOnAwakePitchMin = 1f;
	[Export] private float clipToPlayOnAwakePitchMax = 1f;
	[Export] private bool randomStartPos = false;

	[Export] public float MaxDistance = 30f;
	[Export] public bool Is3D { get; set; } = true;
    public AudioStreamPlayer3D PlayOnAwakeAudioSource { get; private set; }

	private AudioStreamPlayer3D currentAudio3D; //sadly, AudioStreamPlayer3D doesnt inherit from AudioStreamPlayer :(
	private AudioStreamPlayer currentAudio;

	public AudioPlayer() {} //Godot requires an empty constructor
	public AudioPlayer(bool is3D)
	{
		Is3D = is3D;
	}

	public const float DefaultMaxDistance = 30;

	public override void _Ready()
	{
		NewAudio();

		if (ClipToPlayOnAwake != null)
		{
			PlayOneShot(ClipToPlayOnAwake, clipToPlayOnAwakeVolume, Utility.RandomRange(clipToPlayOnAwakePitchMin, clipToPlayOnAwakePitchMax),
						randomStartPos ? Utility.RandomRange(0f, (float)ClipToPlayOnAwake.GetLength()) : 0f);
			PlayOnAwakeAudioSource = currentAudio3D; //assuming its 3D because you cant change Is3D in the inspector
		}
	}


	private int lastRandomIndex = -1;
	public void PlayOneShotRandom(Godot.Collections.Array<AudioStream> streams, float volumeDb, float pitch = 1f)
	{
		int random = Utility.RandomRange(0, streams.Count);
		if(random == lastRandomIndex)
		{
			random = (random + 1) % streams.Count; //add 1 or wrap around to 0
		}
		lastRandomIndex = random;
		PlayOneShot(streams[random], volumeDb, pitch);
	}
	public void PlayOneShotRandom(AudioStream[] streams, float volumeDb, float pitch = 1f)
	{
		int random = Utility.RandomRange(0, streams.Length);
		if(random == lastRandomIndex)
		{
			random = (random + 1) % streams.Length; //add 1 or wrap around to 0
		}
		lastRandomIndex = random;
		PlayOneShot(streams[random], volumeDb, pitch);
	}

    public void PlayOneShot(AudioStream stream, float volumeDb) { PlayOneShot(stream, volumeDb, 1f); }
	public void PlayOneShot(AudioStream stream, float volumeDb, float pitch, float startPos = 0f)
	{
		if(Is3D)
		{
            currentAudio3D.Stream = stream;
            currentAudio3D.Seek(startPos);
            currentAudio3D.Play();
            currentAudio3D.VolumeDb = volumeDb;
			currentAudio3D.MaxDb = volumeDb;
            currentAudio3D.PitchScale = pitch;
            currentAudio3D.Finished += currentAudio3D.QueueFree;
            NewAudio();
		}
		else
		{
			currentAudio.Stream = stream;
            currentAudio.Seek(startPos);
            currentAudio.Play();
            currentAudio.VolumeDb = volumeDb;
            currentAudio.PitchScale = pitch;
            currentAudio.Finished += currentAudio.QueueFree;
            NewAudio();
		}
		
	}

	public void StopPlaying()
	{
		foreach (Node child in GetChildren())
		{
			if(child == currentAudio || child == currentAudio3D)
			{ return; }
			
			AudioStreamPlayer audio = child as AudioStreamPlayer;
			AudioStreamPlayer3D audio3D = child as AudioStreamPlayer3D;
			audio?.QueueFree();
			audio3D?.QueueFree();
		}
	}

	private int id = 0;
	private void NewAudio()
	{
		if (Is3D)
		{
			currentAudio3D = new AudioStreamPlayer3D();
			currentAudio3D.AttenuationFilterCutoffHz = 20500;
			currentAudio3D.MaxDistance = MaxDistance;
			AddChild(currentAudio3D);
			currentAudio3D.Position = Vector3.Zero;
			currentAudio3D.Name = $"AudioStreamPlayer3D_{id}";
		}
		else
		{
			currentAudio = new AudioStreamPlayer();
			AddChild(currentAudio);
			currentAudio.Name = $"AudioStreamPlayer_{id}";
		}

		id++;
		if(id >= int.MaxValue)
		{ id = 0;}
	}
}
