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
		public Vector3 cityStart;
		public Vector3 cityEnd;
		public float cityTime;
		private MoveCameraTask cityViewTask;


		[Header("Scene 2: Scientists")]
		public Sprite scientists;
		public Vector3 scientistStart;
		public Vector3 scientstsEnd;
		public float scientistsTime;
		private MoveCameraTask scientistsTask;


		[Header("Scene 3: Erased")]
		public Sprite erasedScientists;
		public Vector3 erasedStart;
		public Vector3 erasedEnd;
		public float erasedTime;
		private MoveCameraTask erasedTask;


		[Header("Scene 4: Voidweilder")]
		public Sprite voidwielder;
		public Vector3 voidwielderStart;
		public Vector3 voidwielderEnd;
		public float voidwielderTime;
		private MoveCameraTask voidwielderTask;


		[Header("Scene 5: Players")]
		public Sprite players;
		public Vector3 playersStart;
		public Vector3 playersEnd;
		public float playersTime;
		private MoveCameraTask playersTask;


		[Header("Scene 6: Lightrunners")]
		public Sprite lightrunners;
		public Vector3 lightrunnersStart;
		public Vector3 lightrunnersEnd;
		public float lightrunnersTime;
		private MoveCameraTask lightrunnersTask;


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


			cityViewTask = new MoveCameraTask(city, cityStart, cityEnd, cityTime);
			scientistsTask = new MoveCameraTask(scientists, scientistStart, scientstsEnd, scientistsTime);
			erasedTask = new MoveCameraTask(erasedScientists, erasedStart, erasedEnd, erasedTime);
			voidwielderTask = new MoveCameraTask(voidwielder, voidwielderStart, voidwielderEnd, voidwielderTime);
			playersTask = new MoveCameraTask(players, playersStart, playersEnd, playersTime);
			lightrunnersTask = new MoveCameraTask(lightrunners, lightrunnersStart, lightrunnersEnd, lightrunnersTime);


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
