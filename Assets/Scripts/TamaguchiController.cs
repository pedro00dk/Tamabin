using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TamaguchiController : MonoBehaviour {

	TamabinController tamabinController;
	TamaguchiState state;

	bool spaceClear;
	bool tamabinClear;

	int trashAmount;
	const int tamabinFullWhenTrashAmount = 40;

	float timeWhenCurrentStateStarted;

	void Start() {
		tamabinController = FindObjectOfType<TamabinController>();
		state = TamaguchiState.NORMAL;
		spaceClear = false;
		tamabinClear = false;
		trashAmount = 0;
		timeWhenCurrentStateStarted = Time.time;
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
	}

	public enum TamaguchiState {
		NORMAL,
		SAD,
		HAPPY,
		THANK,
		FULL,
		CLEAR
	}
}
