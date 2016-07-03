using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TamaguchiController : MonoBehaviour {

	public Sprite[] normalStateSprites;
	public Sprite[] sadStateSprites;
	public Sprite[] happyStateSprites;
	public Sprite[] thankStateSprites;
	public Sprite[] fullClearStateSprites;

	public AudioClip onNormalClip;
	public AudioClip onSadClip;
	public AudioClip onHappyClip;
	public AudioClip onThankClip;
	public AudioClip onFullClearClip;

	public AudioClip onEatClip;

	TamabinController tamabinController;
	SpriteAnimation tamaguchiAnimation;

	TamaguchiState state;

	bool spaceClear;
	bool tamabinClear;

	int trashAmount;
	const int tamabinFullWhenTrashAmount = 40;

	float timeWhenCurrentStateStarted;

	Stack<TamaguchiState> stateStack;

	void Start() {
		tamabinController = FindObjectOfType<TamabinController>();
		tamaguchiAnimation = FindObjectOfType<SpriteAnimation>();
		stateStack = new Stack<TamaguchiState>();
		SetStateAndResetTime(TamaguchiState.NORMAL);
		spaceClear = false;
		tamabinClear = false;
		trashAmount = 0;

		StartCoroutine(SpriteAnimatorController());

		lastTimeEat = Time.timeSinceLevelLoad;
	}

	float lastTimeEat;

	void Update() {
		tamabinController.TamabinCall();
		bool ateTrash = tamabinController.GetLastMessage() == TamabinController.EAT;

		if (lastTimeEat + 2 < Time.timeSinceLevelLoad) {

			bool tamabinClear = this.tamabinClear;
			this.tamabinClear = false;

			switch (state) {
			case TamaguchiState.NORMAL:
				if (timeWhenCurrentStateStarted + 30 < Time.time) {
					SetStateAndResetTime(TamaguchiState.SAD);
				} else if (ateTrash) {
					tamaguchiAnimation.PlayInstantCip(onEatClip);
					trashAmount++;
					lastTimeEat = Time.timeSinceLevelLoad;
					SetStateAndResetTime(TamaguchiState.HAPPY);
				} else if (spaceClear) {
					SetStateAndResetTime(TamaguchiState.CLEAR);
				} else if (tamabinClear) {
					// Stay on this state
				} else if (trashAmount >= tamabinFullWhenTrashAmount) {
					SetStateAndResetTime(TamaguchiState.FULL);
				}
			break;
			case TamaguchiState.SAD:
			// Time not important on this state
				if (ateTrash) {
					tamaguchiAnimation.PlayInstantCip(onEatClip);
					SetStateAndResetTime(TamaguchiState.HAPPY);
					trashAmount++;
					lastTimeEat = Time.timeSinceLevelLoad;
				} else if (spaceClear) {
					SetStateAndResetTime(TamaguchiState.CLEAR);
				} else if (tamabinClear) {
					SetStateAndResetTime(TamaguchiState.NORMAL);
				} else if (trashAmount >= tamabinFullWhenTrashAmount) {
					SetStateAndResetTime(TamaguchiState.FULL);
				}
			break;
			case TamaguchiState.HAPPY:
				if (timeWhenCurrentStateStarted + 30 < Time.time) {
					SetStateAndResetTime(TamaguchiState.NORMAL);
				} else if (ateTrash) {
					tamaguchiAnimation.PlayInstantCip(onEatClip);
					SetStateAndResetTime(TamaguchiState.THANK);
					lastTimeEat = Time.timeSinceLevelLoad;
				} else if (spaceClear) {
					SetStateAndResetTime(TamaguchiState.CLEAR);
				} else if (tamabinClear) {
					SetStateAndResetTime(TamaguchiState.NORMAL);
				} else if (trashAmount >= tamabinFullWhenTrashAmount) {
					SetStateAndResetTime(TamaguchiState.FULL);
				}
			break;
			case TamaguchiState.THANK:
				if (timeWhenCurrentStateStarted + 6 < Time.time) {
					SetStateAndResetTime(TamaguchiState.HAPPY);
				} else if (ateTrash) {
					tamaguchiAnimation.PlayInstantCip(onEatClip);
					SetStateAndResetTime(TamaguchiState.THANK);
					lastTimeEat = Time.timeSinceLevelLoad;
				} else if (spaceClear) {
					SetStateAndResetTime(TamaguchiState.CLEAR);
				} else if (tamabinClear) {
					SetStateAndResetTime(TamaguchiState.NORMAL);
				} else if (trashAmount >= tamabinFullWhenTrashAmount) {
					SetStateAndResetTime(TamaguchiState.FULL);
				}
			break;
			case TamaguchiState.FULL:
				if (tamabinClear) {
					SetStateAndResetTime(TamaguchiState.NORMAL);
				}
			break;
			case TamaguchiState.CLEAR:
				if (!spaceClear) {
					SetStateAndResetTime(TamaguchiState.NORMAL);
				}
			break;
			}
		}
	}

	public void OnSpaceClear() {
		spaceClear = true;
	}

	public void OnSpaceDirty() {
		spaceClear = false;
	}

	public void ChangeSpaceType() {
		spaceClear = !spaceClear;
	}

	public void OnTamabinClear() {
		tamabinClear = true;
		trashAmount = 0;
	}

	void SetStateAndResetTime(TamaguchiState state) {
		this.state = state;
		timeWhenCurrentStateStarted = Time.time;
		stateStack.Push(state);
	}

	public TamaguchiState PopNextState() {
		return stateStack.Count != 0 ? stateStack.Pop() : TamaguchiState.REPEAT;
	}

	IEnumerator SpriteAnimatorController() {
		while (true) {
			TamaguchiState currentState = PopNextState();
			switch (currentState) {
			case TamaguchiState.NORMAL:
				tamaguchiAnimation.EnqueueSpritesAndWaitLoopAnimation(normalStateSprites, 25, true, onNormalClip, 2);
			break;
			case TamaguchiState.SAD:
				tamaguchiAnimation.EnqueueSpritesAndWaitLoopAnimation(sadStateSprites, 25, true, onSadClip, 2);
			break;
			case TamaguchiState.HAPPY:
				tamaguchiAnimation.EnqueueSpritesAndWaitLoopAnimation(happyStateSprites, 25, true, onHappyClip, 2);
			break;
			case TamaguchiState.THANK:
				tamaguchiAnimation.EnqueueSpritesAndWaitLoopAnimation(thankStateSprites, 25, true, onThankClip, 2);
			break;
			case TamaguchiState.FULL:
				tamaguchiAnimation.EnqueueSpritesAndWaitLoopAnimation(fullClearStateSprites, 25, true, onFullClearClip, 2);
			break;
			case TamaguchiState.CLEAR:
				tamaguchiAnimation.EnqueueSpritesAndWaitLoopAnimation(fullClearStateSprites, 25, true, onFullClearClip, 2);
			break;
			case TamaguchiState.REPEAT:
			break;
			}
			yield return null;
		}
	}

	public enum TamaguchiState {
		NORMAL,
		SAD,
		HAPPY,
		THANK,
		FULL,
		CLEAR,
		REPEAT
	}
}
