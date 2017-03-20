using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Completed
{
	using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;					//Allows us to use UI.

	public class GameManager : MonoBehaviour
	{		
		public float levelStartDelay = 0.005f;						//Time to wait before starting level, in seconds.
		public float resumeDelay = 0.005f;						//Time to wait before resuming in seconds
		public float turnDelay = 0.005f;							//Delay between each Player turn.
		public int playerFoodPoints = 100;						//Starting value for Player food points.
		public int highscore = 0;
		public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
		[HideInInspector] public bool playersTurn = true;		//Boolean to check if it's players turn, hidden in inspector but public.
		public bool gameIsOver = false;
		public bool gameIsPaused = false;
		public int level = 1;	

		private Text highScoreText;
		private Text currentText;
		private Text pauseHighScoreText;
		private Text levelText;									//Text to display current level number.
		private Text continueText;
		private Text dayText;									//Text to display current level number.
		private GameObject levelImage;							//Image to block out level as levels are being set up, background for levelText.
		private GameObject pauseImage;
		private BoardManager boardScript;						//Store a reference to our BoardManager which will set up the level.
		//private int level = 1;									//Current level number, expressed in game as "Day 1".
		private List<Enemy> enemies;							//List of all Enemy units, used to issue them move commands.
		private bool enemiesMoving;								//Boolean to check if enemies are moving.
		private bool doingSetup = true;							//Boolean to check if we're setting up board, prevent Player from moving during setup.

		#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
		private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
		#endif

		//Awake is always called before any Start functions
		void Awake()
		{
			//Check if instance already exists
			if (instance == null)

				//if not, set instance to this
				instance = this;

			//If instance already exists and it's not this:
			else if (instance != this)

				//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
				Destroy(gameObject);	

			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);

			//Assign enemies to a new List of Enemy objects.
			enemies = new List<Enemy>();

			//Get a component reference to the attached BoardManager script
			boardScript = GetComponent<BoardManager>();

			if (!PlayerPrefs.HasKey ("level")) { 
				PlayerPrefs.SetInt ("level", 1);
			} else {
				level = PlayerPrefs.GetInt("level");
			}

			if (!PlayerPrefs.HasKey ("food")) { 
				PlayerPrefs.SetInt ("food", 100);
			} else {
				playerFoodPoints = PlayerPrefs.GetInt("food");
			}

			//Call the InitGame function to initialize the first level 
			InitGame();
		}

		//this is called only once, and the paramter tell it to be called only after the scene was loaded
		//(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static public void CallbackInitialization()
		{
			//register the callback to be called everytime the scene is loaded
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		//This is called each time a scene is loaded.
		static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
		{
			instance.level++;
			instance.InitGame();

		}


		//Initializes the game for each level.
		void InitGame()
		{
			//While doingSetup is true the player can't move, prevent player from moving while title card is up.
			doingSetup = true;

			//Get a reference to our image LevelImage by finding it by name.
			levelImage = GameObject.Find("LevelImage");

			//Get a reference to our image LevelImage by finding it by name.
			pauseImage = GameObject.Find("PauseImage");

			//Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
			levelText = GameObject.Find("LevelText").GetComponent<Text>();
			continueText = GameObject.Find("ContinueText").GetComponent<Text>();
			highScoreText = GameObject.Find("HighScore").GetComponent<Text>();
			pauseHighScoreText = GameObject.Find("PauseHighScore").GetComponent<Text>();
			currentText = GameObject.Find("CurrentScore").GetComponent<Text>();
			currentText.text = "Current: Night " + level;
			//Highscore
			if (!PlayerPrefs.HasKey ("highscore")) { 
				PlayerPrefs.SetInt ("highscore", 0);
				highScoreText.text = "Highscore: 0 nights survived";
			} else {
				highscore = PlayerPrefs.GetInt("highscore");
				highScoreText.text = "Highscore: " + highscore + " nights survived";
			}

			//Set the text of levelText to the string "Day" and append the current level number.
			if (level == 1) {
				levelText.text = "How long can this last?";
				continueText.text = "Swipe to move around. Don't forget to eat.\nTap to get started.";
			} else if (level == 2) {
				levelText.text = "Beware the creatures\nof the night.";
			} else if (level == 3) {
				levelText.text = "Each night they gather\nin larger numbers.";	
			} else if (level == 4) {
				levelText.text = "Or with fiercer force.";	
			} else if (level == 5) {
				levelText.text = "As long as you eat,\nyou can survive.";	
			} else if (level == 6) {
				levelText.text = "Loneliness can be your friend,\njust not tonight.";	
			} else if (level == 7) {
				levelText.text = "I feel this may\nonly get worse.";	
			} else if (level == 8) {
				levelText.text = "Remember who you are.\nRemember where you're from.";	
			} else if (level == 9) {
				levelText.text = "Surely there's an end.";	
			} else if (level == 10) {
				levelText.text = level + " nights wasn't so bad!";	
			} else if (level == 11) {
				levelText.text = "Right now I'm having amnesia\nand deja vu\nat the same time.";	
			} else if (level == 12) {
				levelText.text = "Risk must have reward";	
			} else if (level == 13) {
				levelText.text = "Or at least enjoyment";	
			} else if (level == 14) {
				levelText.text = "It was then\nthat he realised\nhe was golden.";	
			} else if (level == 15) {
				levelText.text = "What happens if you get\nscared half to death\ntwice?";	
			} else if (level == 16) {
				levelText.text = "We all need to learn\nhow to slow things down.";	
			} else if (level == 17) {
				levelText.text = "You can't have everything.\nWhere would you put it?";	
			} else if (level == 18) {
				levelText.text = "What happens if you get\nscared half to death\ntwice";	
			} else if (level == 20) {
				levelText.text = level + " nights.\nWoah!";	
			} else if (level == 21) {
				levelText.text = "At this point\nI'm starting to wonder how\nthis even happened.";	
			} else if (level == 22) {
				levelText.text = "I remember hearing something.";	
			} else if (level == 23) {
				levelText.text = "About a man,\nwho had a gift.";	
			} else if (level == 24) {
				levelText.text = "He was witty.";	
			} else if (level == 25) {
				levelText.text = "And occasionally,\ncrude.";	
			} else if (level == 26) {
				levelText.text = "I'm sure he'll pop up\neventually.";	
			} else if (level == 27) {
				levelText.text = "For now though,\njust survive.";	
			} else if (level == 28) {
				levelText.text = "It is crucial\nfor the task at hand.";	
			} else if (level == 29) {
				levelText.text = "That you make the effort\nto go all the way";	
			} else if (level == 30) {
				levelText.text = "Don't give up.\nYou can prevail!";	
			} else {
				levelText.text = "Night " + level;
			}

			//Set levelImage to active blocking player's view of the game board during setup.
			levelImage.SetActive(true);

			//Set pauseImage to inactive.
			pauseImage.SetActive(false);

			//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
			//Invoke("HideLevelImage", levelStartDelay);

			//Clear any Enemy objects in our List to prepare for next level.
			enemies.Clear();

			//Call the SetupScene function of the BoardManager script, pass it current level number.
			boardScript.SetupScene(level);

		}


		//Hides black image used between levels
		void HideLevelImage()
		{
			//Disable the levelImage gameObject.
			levelImage.SetActive(false);

			//Disable the levelImage gameObject.
			pauseImage.SetActive(false);

			//Set doingSetup to false allowing player to move again.
			doingSetup = false;
		}

		//Shows pause screen
		public void ShowPause()
		{
			//Enable the levelImage gameObject.
			pauseImage.SetActive(true);
		}

		//Shows pause screen
		public void Unpause()
		{
			Invoke("UnpauseTrue", levelStartDelay);
		}

		public void UnpauseTrue (){
			//Disable the pauseImage gameObject.
			gameIsPaused = false;
			doingSetup = false;
			pauseImage.SetActive(false);
		}

		//Update is called every frame.
		void Update()
		{			
			//Check if game over and allow restart
			if(gameIsOver){
				//enabled = false;
				if (Input.GetMouseButton (0)){
					SceneManager.LoadScene("Main");
				//enabled = false;
					level = 0;
					gameIsOver = false;
				}
			}

			//Tap to start or continue game 
			if(doingSetup && !gameIsPaused){
				if (Input.GetMouseButton (0)){					
					Invoke("HideLevelImage", levelStartDelay);
				}
			}

			//Pause game
			if (!gameIsOver && !gameIsPaused) {
				if (Input.GetKeyDown(KeyCode.Escape)){
					gameIsPaused = true;
					doingSetup = true;
					pauseImage.SetActive(true);
					pauseHighScoreText.text = "Highscore: " + highscore + " nights survived";
				}
			}

			//Pause menu
			if(gameIsPaused){
				//Unpause
				if (Input.GetKeyDown(KeyCode.Escape)){
					Unpause ();
				}
			}

			//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
			if(playersTurn || enemiesMoving || doingSetup)

				//If any of these are true, return and do not start MoveEnemies.
				return;

			//Start moving enemies.
			StartCoroutine (MoveEnemies ());
		}

		//Call this to add the passed in Enemy to the List of Enemy objects.
		public void AddEnemyToList(Enemy script)
		{
			//Add Enemy to List enemies.
			enemies.Add(script);
		}


		//GameOver is called when the player reaches 0 food points
		public void GameOver()
		{		
			//Set levelText to display number of levels passed and game over message
			levelText.text = "You died,\nafter " + level + " nights.";
			continueText.text = "Tap to restart";

			//Enable black background image gameObject.
			levelImage.SetActive(true);

			if(level > PlayerPrefs.GetInt ("highscore")){
				PlayerPrefs.SetInt ("highscore", level);
			}

			PlayerPrefs.SetInt ("level", 1);
			PlayerPrefs.SetInt ("food", 100);
			PlayerPrefs.Save();
			//level = 1;
			gameIsOver = true;
			//Disable this GameManager.
			//enabled = false;
		}

		//Coroutine to move enemies in sequence.
		IEnumerator MoveEnemies()
		{
			//While enemiesMoving is true player is unable to move.
			enemiesMoving = true;

			//Wait for turnDelay seconds, defaults to .1 (100 ms).
			yield return new WaitForSeconds(turnDelay);

			//If there are no enemies spawned (IE in first level):
			if (enemies.Count == 0) 
			{
				//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
				yield return new WaitForSeconds(turnDelay);
			}

			//Loop through List of Enemy objects.
			for (int i = 0; i < enemies.Count; i++)
			{
				//Call the MoveEnemy function of Enemy at index i in the enemies List.
				enemies[i].MoveEnemy ();

				//Wait for Enemy's moveTime before moving next Enemy, 
				yield return new WaitForSeconds(enemies[i].moveTime);
			}
			//Once Enemies are done moving, set playersTurn to true so player can move.
			playersTurn = true;

			//Enemies are done moving, set enemiesMoving to false.
			enemiesMoving = false;
		}
	}
}

