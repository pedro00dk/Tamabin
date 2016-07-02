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
	}

	void Update() {
		tamabinController.TamabinCall();
		bool ateTrash = tamabinController.GetLastMessage() == TamabinController.EAT;
		bool tamabinClear = this.tamabinClear;
		this.tamabinClear = false;

		switch (state) {
		case TamaguchiState.NORMAL:
			if (timeWhenCurrentStateStarted + 60 < Time.time) {
				SetStateAndResetTime(TamaguchiState.SAD);
				trashAmount++;
			} else if (ateTrash) {
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
				SetStateAndResetTime(TamaguchiState.HAPPY);
				trashAmount++;
			} else if (spaceClear) {
				SetStateAndResetTime(TamaguchiState.CLEAR);
			} else if (tamabinClear) {
				SetStateAndResetTime(TamaguchiState.NORMAL);
			} else if (trashAmount >= tamabinFullWhenTrashAmount) {
				SetStateAndResetTime(TamaguchiState.FULL);
			}
		break;
		case TamaguchiState.HAPPY:
			if (timeWhenCurrentStateStarted + 60 < Time.time) {
				SetStateAndResetTime(TamaguchiState.NORMAL);
			} else if (ateTrash) {
				SetStateAndResetTime(TamaguchiState.THANK);
			} else if (spaceClear) {
				SetStateAndResetTime(TamaguchiState.CLEAR);
			} else if (tamabinClear) {
				SetStateAndResetTime(TamaguchiState.NORMAL);
			} else if (trashAmount >= tamabinFullWhenTrashAmount) {
				SetStateAndResetTime(TamaguchiState.FULL);
			}
		break;
		case TamaguchiState.THANK:
			if (timeWhenCurrentStateStarted + 8 < Time.time) {
				SetStateAndResetTime(TamaguchiState.HAPPY);
			} else if (ateTrash) {
				SetStateAndResetTime(TamaguchiState.THANK);
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
				tamaguchiAnimation.SetSpritesAndStartAnimation(normalStateSprites, 25, true);
			break;
			case TamaguchiState.SAD:
				tamaguchiAnimation.SetSpritesAndStartAnimation(sadStateSprites, 25, true);
			break;
			case TamaguchiState.HAPPY:
				tamaguchiAnimation.SetSpritesAndStartAnimation(happyStateSprites, 25, true);
			break;
			case TamaguchiState.THANK:
				tamaguchiAnimation.SetSpritesAndStartAnimation(thankStateSprites, 25, true);
			break;
			case TamaguchiState.FULL:
				tamaguchiAnimation.SetSpritesAndStartAnimation(fullClearStateSprites, 25, true);
			break;
			case TamaguchiState.CLEAR:
				tamaguchiAnimation.SetSpritesAndStartAnimation(fullClearStateSprites, 25, true);
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
