using UnityEngine;

public class SwapEnabledTask : Task {


	////////////////////////////////////////////////////////////////////////
	/// Fields that are different for each SwapEnabledTask
	////////////////////////////////////////////////////////////////////////


	private GameObject obj1;
	private GameObject obj2;
	private Vector3 mainCameraPos;
	private float duration;


	////////////////////////////////////////////////////////////////////////
	/// Fields that are shared across EnableDisableTasks
	////////////////////////////////////////////////////////////////////////


	private float timer = 0.0f;


	////////////////////////////////////////////////////////////////////////
	/// Constructor
	////////////////////////////////////////////////////////////////////////


	public SwapEnabledTask(GameObject obj1, GameObject obj2, Vector3 mainCameraPos, float duration){
		this.obj1 = obj1;
		this.obj2 = obj2;
		this.mainCameraPos = mainCameraPos;
		this.duration = duration;
	}


	////////////////////////////////////////////////////////////////////////
	/// Lifecycle functions
	////////////////////////////////////////////////////////////////////////


	protected override void Init(){
		obj1.SetActive(true);
		obj2.SetActive(false);
		Camera.main.transform.position = mainCameraPos;
	}


	internal override void Update(){
		timer += Time.deltaTime;

		if (timer >= duration){
			obj1.SetActive(false);
			obj2.SetActive(true);
			SetStatus(TaskStatus.Succeeded);
		}
	}


	protected override void Cleanup(){
		timer = 0.0f;
	}
}
