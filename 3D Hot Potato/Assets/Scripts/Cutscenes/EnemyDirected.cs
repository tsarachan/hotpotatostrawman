/*
 * 
 * This script moves an enemy in a "directed" way for cutscenes--it does what it's told, without any independent behavior.
 * 
 * The setup here assumes that the enemy will be able to complete one move before being told the next thing it needs to do.
 * It can't queue up moves to zig-zag or the like; it must receive the zig and finish it before being told to zag.
 * 
 */

namespace Cutscene
{
	using UnityEngine;
	using System.Collections;

	public class EnemyDirected : EnemyBase {

		private float timer = 0.0f;

		//variables controlling this enemy's movement
		private float startMoveTime = 0.0f;
		private float endMoveTime = 0.0f;
		private Vector3 startMovePos = Vector3.zero;
		private Vector3 endMovePos = Vector3.zero;
		private AnimationCurve moveCurve;



		private void Update(){
			timer += Time.deltaTime;

			if (timer >= startMoveTime && timer <= endMoveTime){
				transform.position = MoveTransform();
			}
		}

		/// <summary>
		/// Moves this enemy. The values used are set in MoveEnemy(), below.
		/// </summary>
		/// <returns>The enemy's new position.</returns>
		private Vector3 MoveTransform(){
			return Vector3.Lerp(startMovePos,
								endMovePos,
								moveCurve.Evaluate((timer - startMoveTime)/(endMoveTime - startMoveTime)));
		}


		/// <summary>
		/// CutsceneManager calls this to set this enemy in motion. All values MoveTransform() needs are set here.
		/// </summary>
		/// <param name="endPos">The enemy's final position.</param>
		/// <param name="moveTime">How long the enemy takes to move.</param>
		/// <param name="curve">An animation curve to fine-tune the move's speed.</param>
		public void MoveEnemy(Vector3 endPos, float moveTime, AnimationCurve curve){
			startMoveTime = Time.time;
			endMoveTime = Time.time + moveTime;
			startMovePos = transform.position;
			endMovePos = endPos;
			moveCurve = curve;
		}
	}
}
