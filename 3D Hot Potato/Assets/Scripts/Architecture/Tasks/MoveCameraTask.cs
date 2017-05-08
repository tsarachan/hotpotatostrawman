namespace AttractMode
{
	using UnityEngine;
	using UnityEngine.UI;

	public class MoveCameraTask : Task {

		////////////////////////////////////////////////////////////////////////
		/// Fields that are different for each MoveCameraTask
		////////////////////////////////////////////////////////////////////////


		private Sprite cutscene; //the picture that will display
		private Vector3 start; //start location of camera
		private Vector3 end; //end location of camera
		private float duration; //how long this image stays on screen; also how long the camera moves


		////////////////////////////////////////////////////////////////////////
		/// Fields constant across MoveCameraTasks
		////////////////////////////////////////////////////////////////////////


		private Camera mainCam;
		private float moveTimer = 0.0f;
		private Image cutsceneImage;
		private const string CUTSCENE_IMAGE = "Cutscene image";


		////////////////////////////////////////////////////////////////////////
		/// Constructor
		////////////////////////////////////////////////////////////////////////


		public MoveCameraTask(Sprite cutscene, Vector3 start, Vector3 end, float duration){
			this.cutscene = cutscene;;
			this.start = start;
			this.end = end;
			this.duration = duration;
			cutsceneImage = GameObject.Find(CUTSCENE_IMAGE).GetComponent<Image>();
		}


		////////////////////////////////////////////////////////////////////////
		/// Lifetime functions
		////////////////////////////////////////////////////////////////////////


		protected override void Init(){
			cutsceneImage.sprite = cutscene;
			mainCam = Camera.main;
			mainCam.transform.position = start;
		}


		internal override void Update(){
			moveTimer += Time.deltaTime;

			mainCam.transform.position = Vector3.Lerp(start, end, moveTimer/duration);

			if (moveTimer >= duration){
				SetStatus(TaskStatus.Succeeded);
			}
		}


		protected override void Cleanup(){
			moveTimer = 0.0f;
		}
	}
}
