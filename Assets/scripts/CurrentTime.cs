using UnityEngine;
using System.Collections;

public class CurrentTime {

	private static readonly System.DateTime Jan1st1970 = new System.DateTime
		(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

	public static long currentTimeMillis() {
		return (long) (System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
	}
}
