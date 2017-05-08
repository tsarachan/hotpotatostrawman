using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager{

	private readonly List<Task> tasks = new List<Task>();


	public void AddTask(Task task){
		Debug.Assert(task != null);

		Debug.Assert(!task.IsAttached);

		tasks.Add(task);
		task.SetStatus(Task.TaskStatus.Pending);
	}


	public void Tick(){
		for (int i = tasks.Count - 1; i >= 0; --i){
			Task task = tasks[i];

			//switch any tasks just added to working status, so that they initialize themselves
			if (task.IsPending){
				task.SetStatus(Task.TaskStatus.Working);
			}

			//if the task is done, change the game state accordingly
			if (task.IsFinished){
				HandleCompletion(task, i);
			}

			//the task isn't done yet, so it should do at least one update loop
			else{
				task.Update();
				if (task.IsFinished){
					HandleCompletion(task, i);
				}
			}
		}
	}


	/// <summary>
	/// Use this function to establish the appropriate state when a task is finished. It tells the task manager
	/// to manage the next task, and then detaches the current task so that the manager knows it's no longer being
	/// addressed and can be cleaned up.
	/// </summary>
	/// <param name="task">The task that is completed.</param>
	/// <param name="taskIndex">The index of the completed task in the manager's list of tasks.</param>
	private void HandleCompletion(Task task, int taskIndex){
		if (task.NextTask != null && task.IsSucceeded){
			AddTask(task.NextTask);
		}

		tasks.RemoveAt(taskIndex);
		task.SetStatus(Task.TaskStatus.Detached);
	}
}
