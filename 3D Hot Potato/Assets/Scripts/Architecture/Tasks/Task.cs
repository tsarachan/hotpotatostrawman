using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task {

	//an enum with all the statuses a task can have
	//the normal sequence is Detacted -> Pending -> Working -> Success
	//Fail and Aborted are for special cases
	public enum TaskStatus { Detached, Pending, Working, Succeeded, Failed, Aborted };


	//each task has its own status
	public TaskStatus Status { get; private set; }


	//convenience properties for getting the task's status
	public bool IsDetached { get { return Status == TaskStatus.Detached; } }
	public bool IsAttached { get { return Status != TaskStatus.Detached; } }
	public bool IsPending { get { return Status == TaskStatus.Pending; } }
	public bool IsWorking { get { return Status == TaskStatus.Working; } }
	public bool IsSucceeded { get { return Status == TaskStatus.Succeeded; } }
	public bool IsFailed { get { return Status == TaskStatus.Failed; } }
	public bool IsAborted { get { return Status == TaskStatus.Aborted; } }
	public bool IsFinished {
		get { return (Status == TaskStatus.Aborted || 
					  Status == TaskStatus.Failed ||
					  Status == TaskStatus.Succeeded); } }



	/// <summary>
	/// Used the change the task's status.
	/// </summary>
	internal void SetStatus(TaskStatus status){
		//Debug.Log("SetStatus called; status == " + status);
		//if it's not actually a change, stop here
		if (Status == status){
			//Debug.Log("No change in status; returning");
			return;
		}


		Status = status;

		switch (status){

			//when the task begins working, initialize it
			//do not initialize sooner--that is more likely to result in stale data
			case TaskStatus.Working:
				Init();
				break;

			//however the task ends, it should act appropriately and then clean up
			case TaskStatus.Succeeded:
				//Debug.Log("Handling the success process");
				OnSuccess();
				Cleanup();
				break;
			case TaskStatus.Failed:
				OnFail();
				Cleanup();
				break;
			case TaskStatus.Aborted:
				OnAbort();
				Cleanup();
				break;

			//states relevant to the task manager--the task doesn't need to do do anything for these
			case TaskStatus.Detached:
			case TaskStatus.Pending:
				break;
			
			//error handling
			default:
				Debug.Log("Illegal status: " + status);
				break;
		}
	}


	/// <summary>
	/// Functions for the essential parts of a task's lifecycle.
	/// </summary>
	protected virtual void Init() { }

	internal virtual void Update() { }

	protected virtual void Cleanup() { }


	/// <summary>
	/// Functions tasks can override to define their behavior when these steps are reached.
	/// </summary>
	protected virtual void OnSuccess() { }

	protected virtual void OnFail() { }

	protected virtual void OnAbort() { }


	/// <summary>
	/// Call this to abort the task.
	/// </summary>
	public void Abort(){
		SetStatus(TaskStatus.Aborted);
	}


	/// <summary>
	/// System for assigning a task to follow the current one.
	/// 
	/// This property stores the following task.
	/// 
	/// Call the function below to assign the task.
	/// </summary>
	/// <value>The task that will follow the instant one.</value>
	public Task NextTask { get; private set; }


	public Task Then(Task task){
		Debug.Assert(!task.IsAttached);
		NextTask = task;
		return task;
	}
}
