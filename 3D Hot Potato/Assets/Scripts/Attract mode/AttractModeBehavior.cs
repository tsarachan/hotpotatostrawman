namespace AttractMode
{
	using Rewired;
	using UnityEngine;
	using UnityEngine.SceneManagement;

	public class AttractModeBehavior : MonoBehaviour {


		////////////////////////////////////////////////////////////////////////
		/// Task manager that drives the attract mode
		////////////////////////////////////////////////////////////////////////


		private TaskManager taskManager;


		////////////////////////////////////////////////////////////////////////
		/// Information for the tasks that drive the attract mode,
		/// broken down by cutscene
		////////////////////////////////////////////////////////////////////////


		[Header("Scene 1: City view")]
		public Sprite city;
		public Vector2 cityStart;
		public Vector2 cityEnd;
		public float cityTime;
		private MoveImageTask cityViewTask;


		[Header("Scene 2: Scientists")]
		public Sprite scientists;
		public Vector2 scientistStart;
		public Vector2 scientstsEnd;
		public float scientistsTime;
		private MoveImageTask scientistsTask;


		[Header("Scene 3: Erased")]
		public Sprite erasedScientists;
		public Vector2 erasedStart;
		public Vector2 erasedEnd;
		public float erasedTime;
		private MoveImageTask erasedTask;


		[Header("Scene 4: Voidweilder")]
		public Sprite voidwielder;
		public Vector2 voidwielderStart;
		public Vector2 voidwielderEnd;
		public float voidwielderTime;
		private MoveImageTask voidwielderTask;


		[Header("Scene 5: Players")]
		public Sprite players;
		public Vector2 playersStart;
		public Vector2 playersEnd;
		public float playersTime;
		private MoveImageTask playersTask;


		[Header("Scene 6: Lightrunners")]
		public Sprite lightrunners;
		public Vector2 lightrunnersStart;
		public Vector2 lightrunnersEnd;
		public float lightrunnersTime;
		private MoveImageTask lightrunnersTask;


		[Header("Scene 7: Title screen")]
		public float titleSceneDuration = 5.0f;
		public Vector3 mainCameraPos = new Vector3(0.0f, 33.63f, -24.0f);
		private const string TITLE_SCENE_PREFAB = "Title scene as prefab";
		private const string CUTSCENE_CANVAS_OBJ = "Cutscene canvas";
		private SwapEnabledTask swapEnabledTask;


		////////////////////////////////////////////////////////////////////////
		/// Next scene to load
		////////////////////////////////////////////////////////////////////////


		private const string NEXT_SCENE = "Level 1";


		////////////////////////////////////////////////////////////////////////
		/// Setup
		////////////////////////////////////////////////////////////////////////


		//create and order tasks
		private void Start(){
			taskManager = new TaskManager();


			cityViewTask = new MoveImageTask(city, cityStart, cityEnd, cityTime);
			scientistsTask = new MoveImageTask(scientists, scientistStart, scientstsEnd, scientistsTime);
			erasedTask = new MoveImageTask(erasedScientists, erasedStart, erasedEnd, erasedTime);
			voidwielderTask = new MoveImageTask(voidwielder, voidwielderStart, voidwielderEnd, voidwielderTime);
			playersTask = new MoveImageTask(players, playersStart, playersEnd, playersTime);
			lightrunnersTask = new MoveImageTask(lightrunners, lightrunnersStart, lightrunnersEnd, lightrunnersTime);


			GameObject titleScene = GameObject.Find(TITLE_SCENE_PREFAB);
			GameObject cutsceneCanvas = GameObject.Find(CUTSCENE_CANVAS_OBJ);
			swapEnabledTask = new SwapEnabledTask(titleScene, cutsceneCanvas, mainCameraPos, titleSceneDuration);
			titleScene.SetActive(false);

			cityViewTask.Then(scientistsTask);
			scientistsTask.Then(erasedTask);
			erasedTask.Then(voidwielderTask);
			voidwielderTask.Then(playersTask);
			playersTask.Then(lightrunnersTask);
			lightrunnersTask.Then(swapEnabledTask);
			swapEnabledTask.Then(cityViewTask);


			taskManager.AddTask(cityViewTask);
		}


		////////////////////////////////////////////////////////////////////////
		/// Update loop
		////////////////////////////////////////////////////////////////////////


		private void Update(){
			taskManager.Tick();

			if (ReInput.controllers.GetAnyButtonDown()){
				SceneManager.LoadScene(NEXT_SCENE);
			}
		}
	}
}
