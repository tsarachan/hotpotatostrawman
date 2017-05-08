namespace AttractMode
{
	using TMPro;
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
		private string caption; //the text, if any, that displays while this image is on-screen


		////////////////////////////////////////////////////////////////////////
		/// Fields constant across MoveImageTasks
		////////////////////////////////////////////////////////////////////////


		private RectTransform rect;
		private float moveTimer = 0.0f;
		private Image cutsceneImage;
		private const string CUTSCENE_IMAGE = "Cutscene image";
		private GameObject captionOrganizer;
		private const string CAPTION_ORGANIZER = "Cutscene caption";
		private TextMeshProUGUI captionText;
		private const string CAPTION_OBJ = "Caption";
		private float typeDelay = 0.0f;
		private float typeTimer = 0.0f;
		private int numCharsDisplayed = 0;


		////////////////////////////////////////////////////////////////////////
		/// Constructor
		////////////////////////////////////////////////////////////////////////


		public MoveImageTask(Sprite cutscene, Vector2 start, Vector2 end, float duration, string text){
			this.cutscene = cutscene;;
			this.start = start;
			this.end = end;
			this.duration = duration;
			this.caption = text;
			cutsceneImage = GameObject.Find(CUTSCENE_IMAGE).GetComponent<Image>();
			rect = cutsceneImage.rectTransform;

			captionOrganizer = GameObject.Find(CAPTION_ORGANIZER);
			captionText = GameObject.Find(CAPTION_OBJ).GetComponent<TextMeshProUGUI>();
		}


		////////////////////////////////////////////////////////////////////////
		/// Lifetime functions
		////////////////////////////////////////////////////////////////////////


		protected override void Init(){
			cutsceneImage.sprite = cutscene;
			rect.anchoredPosition = start;
			if (captionText.text == ""){
				captionOrganizer.SetActive(false);
			} else {
				captionOrganizer.SetActive(true);
				captionText.text = caption;
			}
		}


		internal override void Update(){
			rect.anchoredPosition = MoveImage();

			if (numCharsDisplayed < caption.Length){
				captionText.text = TypeText();
			}

			if (moveTimer >= duration){
				SetStatus(TaskStatus.Succeeded);
			}
		}


		private Vector2 MoveImage(){
			moveTimer += Time.deltaTime;

			return Vector2.Lerp(start, end, moveTimer/duration);
		}


		private string TypeText(){
			typeTimer += Time.deltaTime;

			char[] captionAsArray = caption.ToCharArray();
			string temp = "";

			if (typeTimer >= typeDelay){
				numCharsDisplayed++;

				for (int i = 0; i < numCharsDisplayed; i++){
					temp += captionAsArray[i];
				}
			}

			return temp;
		}


		protected override void Cleanup(){
			moveTimer = 0.0f;
			typeTimer = 0.0f;
			captionOrganizer.SetActive(false);
		}
	}
}
