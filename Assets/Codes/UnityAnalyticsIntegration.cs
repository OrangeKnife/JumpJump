using UnityEngine;
using System.Collections;
using UnityEngine.Cloud.Analytics;

public class UnityAnalyticsIntegration : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
		const string projectId = "27425ee5-3c6e-4812-b220-8cf378c914d0";
		UnityAnalytics.StartSDK (projectId);
		
	}
}
