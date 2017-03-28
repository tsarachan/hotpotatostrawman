namespace LightningRodBoss
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class LightningRodBehavior : EnemyBase {


		private LightningRodBossBehavior bossBehavior;


		private void Start(){
			bossBehavior = transform.parent.parent.GetComponent<LightningRodBossBehavior>();
		}


		//THIS IS VERY HACKY
		//THE TIME FOR A REFACTOR IS NIGH

		/// <summary>
		/// The lightning rod is never destroyed; rather, when hit, it sends a message to the boss to become
		/// vulnerable to attack
		/// </summary>
		public override void GetDestroyed(){
			bossBehavior.BecomeVulnerable();
		}
	}
}
