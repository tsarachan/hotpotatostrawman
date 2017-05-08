namespace AttractMode
{
	using UnityEngine;
	using UnityEngine.UI;

	public class MoveImageTask : Task {

		////////////////////////////////////////////////////////////////////////
		/// Fields that are different for each MoveImageTask
		////////////////////////////////////////////////////////////////////////


		private Sprite cutscene; //the picture that will display
		private Vector2 start; //start location of image
		private Vector2 end; //end location of image
		private float duration; //how long this image stays on screen; also how long the camera moves


		////////////////////////////////////////////////////////////////////////
		/// Fields constant across MoveImageTasks
		////////////////////////////////////////////////////////////////////////


		private RectTransform rect;
		private float moveTimer = 0.0f;
		private Image cutsceneImage;
		private const string CUTSCENE_IMAGE = "Cutscene image";


		////////////////////////////////////////////////////////////////////////
		/// Constructor
		////////////////////////////////////////////////////////////////////////


		public MoveImageTask(Sprite cutscene, Vector2 start, Vector2 end, float duration){
			this.cutscene = cutscene;;
			this.start = start;
			this.end = end;
			this.duration = duration;
			cutsceneImage = GameObject.Find(CUTSCENE_IMAGE).GetComponent<Image>();
			rect = cutsceneImage.rectTransform;
		}


		////////////////////////////////////////////////////////////////////////
		/// Lifetime functions
		////////////////////////////////////////////////////////////////////////


		protected override void Init(){
			cutsceneImage.sprite = cutscene;
			rect.anchoredPosition = start;
		}


		internal override void Update(){
			moveTimer += Time.deltaTime;

			rect.anchoredPosition = Vector2.Lerp(start, end, moveTimer/duration);

			if (moveTimer >= duration){
				SetStatus(TaskStatus.Succeeded);
			}
		}


		protected override void Cleanup(){
			moveTimer = 0.0f;
		}
	}
}
