using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimation : MonoBehaviour {

	// Components
	SpriteRenderer spriteRenderer;

	// Internal properties
	Sprite[] sprites;
	float framerate;
	bool loop;

	float startTime;
	int currentFrame;
	int executionCount;

	void Start() {
		sprites = null;
		framerate = -1;
		loop = false;
		spriteRenderer = GetComponent<SpriteRenderer>();
		startTime = Time.timeSinceLevelLoad;
		currentFrame = 0;
		executionCount = 0;
	}

	void Update() {
		if (sprites != null && sprites.Length > 0) {
			if (loop || executionCount == 0) {
				currentFrame = (int) (((Time.timeSinceLevelLoad - startTime) * framerate) % sprites.Length);
				spriteRenderer.sprite = sprites[currentFrame];
				if (currentFrame == sprites.Length - 1) {
					executionCount++;
				}
			}
		}
	}

	public void SetSpritesAndStartAnimation(Sprite[] sprites, float framerate, bool loop) {
		this.sprites = sprites;
		this.framerate = framerate;
		this.loop = loop;
		currentFrame = 0;
		executionCount = 0;
	}
}
