using UnityEngine;
using System.Collections;

public class TamaguchiPlayer : MonoBehaviour {

	public MovieTexture normal;
	public MovieTexture sad;
	public MovieTexture happy;
	public MovieTexture thank;
	public MovieTexture fullAndClear;

	TamabinController tamaguchiController;

	void Start() {
		tamaguchiController = GetComponent<TamaguchiController>();
	}

	void Update() {
		
	}
}
