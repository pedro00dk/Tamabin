using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class SpriteAnimation : MonoBehaviour {

	public AudioSource delayedPlayer;
	public AudioSource instantPlayer;

	// Components
	SpriteRenderer spriteRenderer;

	// Internal properties
	Sprite[] sprites;
	float framerate;
	bool loop;

	float startTime;
	int currentFrame;
	bool firstLoop;

	Queue<SpriteInfo> spriteQueue;

	void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();

		sprites = null;
		framerate = -1;
		loop = false;

		startTime = Time.timeSinceLevelLoad;
		currentFrame = 0;
		firstLoop = true;
		spriteQueue = new Queue<SpriteInfo>();
	}

	void Update() {
		if ((sprites == null || sprites.Length == 0) && spriteQueue.Count > 0) {
			SpriteInfo spriteInfo = spriteQueue.Dequeue();
			SetSpritesAndStartAnimation(spriteInfo.sprites, spriteInfo.framerate, spriteInfo.loop, spriteInfo.clip, spriteInfo.delay);
		}
		if (sprites != null && sprites.Length > 0) {
			if (loop || firstLoop) {
				currentFrame = (int) ((Time.timeSinceLevelLoad - startTime) * framerate);
				int currentSpriteFrame = currentFrame % sprites.Length;
				spriteRenderer.sprite = sprites[currentSpriteFrame];
				if (currentFrame >= sprites.Length - 1) {
					firstLoop = false;
					if (spriteQueue.Count > 0) {
						SpriteInfo spriteInfo = spriteQueue.Dequeue();
						SetSpritesAndStartAnimation(spriteInfo.sprites, spriteInfo.framerate, spriteInfo.loop, spriteInfo.clip, spriteInfo.delay);
					}
				}
			}
		}
	}

	public void SetSpritesAndStartAnimation(Sprite[] sprites, float framerate, bool loop, AudioClip clip, float clipDelay) {
		this.sprites = sprites;
		this.framerate = framerate;
		this.loop = loop;
		startTime = Time.timeSinceLevelLoad;
		currentFrame = 0;
		firstLoop = true;
		if (clip != null) {
			delayedPlayer.clip = clip;
			delayedPlayer.PlayDelayed(clipDelay);
		}
	}

	public void EnqueueSpritesAndWaitLoopAnimation(Sprite[] sprites, float framerate, bool loop, AudioClip clip, float clipDelay) {
		spriteQueue.Enqueue(new SpriteInfo(sprites, framerate, loop, clip, clipDelay));
	}

	class SpriteInfo {
		public readonly Sprite[] sprites;
		public readonly float framerate;
		public readonly bool loop;
		public readonly AudioClip clip;
		public readonly float delay;

		public SpriteInfo(Sprite[] sprites, float framerate, bool loop, AudioClip clip, float delay) {
			this.sprites = sprites;
			this.framerate = framerate;
			this.loop = loop;
			this.clip = clip;
			this.delay = delay;
		}
	}

	public void PlayInstantCip(AudioClip clip) {
		instantPlayer.clip = clip;
		instantPlayer.Play();
	}
}
