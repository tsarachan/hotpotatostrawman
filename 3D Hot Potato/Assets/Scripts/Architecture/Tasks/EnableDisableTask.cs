﻿using UnityEngine;

public class EnableDisableTask : Task {


	////////////////////////////////////////////////////////////////////////
	/// Fields that are different for each EnableDisableTask
	////////////////////////////////////////////////////////////////////////


	private GameObject obj;
	private float duration;


	////////////////////////////////////////////////////////////////////////
	/// Fields that are shared across EnableDisableTasks
	////////////////////////////////////////////////////////////////////////


	private float timer = 0.0f;


	////////////////////////////////////////////////////////////////////////
	/// Constructor
	////////////////////////////////////////////////////////////////////////


	public EnableDisableTask(GameObject obj, float duration){
		this.obj = obj;
		this.duration = duration;
	}


	////////////////////////////////////////////////////////////////////////
	/// Lifecycle functions
	////////////////////////////////////////////////////////////////////////


	protected override void Init(){
		obj.SetActive(true);
	}


	internal override void Update(){
		timer += Time.deltaTime;

		if (timer >= duration){
			obj.SetActive(false);
			SetStatus(TaskStatus.Succeeded);
		}
	}


	protected override void Cleanup(){
		timer = 0.0f;
	}
}