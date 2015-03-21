using UnityEngine;
using System.Collections;

public class PracticeBehavior : MonoBehaviour {
	int currState = 0;
	Animator anim;
	private int runHash = Animator.StringToHash("run");
	private int idleHash = Animator.StringToHash("idle");
	private int runAttackSingleShot = 
		Animator.StringToHash("run, attack single shot");
	private int standAttackSingleShot =
		Animator.StringToHash("stand, attack single shot");
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		//anim.SetFloat ("Speed", 2);
		//anim.SetTrigger ("Fire");
	}
	
	// Update is called once per frame
	void Update () {
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo (0);
		int newState = stateInfo.shortNameHash;
		if (currState != stateInfo.shortNameHash) {
			currState = stateInfo.shortNameHash;
			if (stateInfo.shortNameHash == idleHash) {
				Debug.Log ("is idle");
			} else if (stateInfo.shortNameHash == runHash) {
				Debug.Log ("is running");
			} else if (stateInfo.shortNameHash == runAttackSingleShot) {
				Debug.Log ("is running and single shot");
			} else if (newState == standAttackSingleShot) {
				Debug.Log ("is stand, attack single shot");
			}
		}
	}
}
