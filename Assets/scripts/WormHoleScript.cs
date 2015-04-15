using UnityEngine;
using System.Collections;

public class WormHoleScript : MonoBehaviour, InstanceObjectScript, UIPlacer {
	//TODO: change to WormHole type if possible
	public InstanceObject instanceObject { get; set; }

	public Vector3 getUIScreenPosition() {
		WormHole w = (WormHole)instanceObject;
		Vector3 worldOffset = new Vector3 (0, 0, 0);
		if (w.relativeCoords.y > 0) {
			worldOffset = new Vector3 (0, this.gameObject.transform.localScale.x, 0);
		} else if (w.relativeCoords.y < 0) {
			worldOffset = new Vector3 (0, -this.gameObject.transform.localScale.x, 0);
		}
		Vector3 textPosition = Camera.main.WorldToScreenPoint (transform.position + worldOffset);
		//SetActive(!Globals.isInLocalView);
		return textPosition;
	}

	public Vector2 getPivot() {
		WormHole w = (WormHole)instanceObject;
		Vector2 retVal = new Vector2 ();
		if (w.relativeCoords.x < 0) {
			retVal.x = 1;;
		} else {
			retVal.x = 0;
		}
		if (w.relativeCoords.y > 0)
			retVal.y = 0;
		else
			retVal.y = 1;
		return retVal;
	}

	public void onAttackConfirmed(int numUnits) {
		StartCoroutine (coOnAttackConfirmed (numUnits));
	}

	public IEnumerator coOnAttackConfirmed(int numUnits) {
		WormHole w = (WormHole)gameObject.GetComponent<InstanceObjectScript> ().instanceObject;
		Base b = w.b;
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("username", "kmw8sf");
		wwwform.AddField ("baseId", b.baseId);
		wwwform.AddField ("wormholeId", w.wormholeId);
		wwwform.AddField ("numUnits", numUnits);
		WWW request = new WWW ("localhost:8080/myapp/world/attack", wwwform);
		yield return request;
		Attack attack = LitJson.JsonMapper.ToObject<Attack> (request.text);
		if (attack.attackerBaseId != null) {
			AttackHandler.instance.addAttack(attack);
		}
	}
}
