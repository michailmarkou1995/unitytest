using System;
using Abilities.Pickups;
using Core.Player;
using Interfaces.Abilities.PickUps;
using Interfaces.Abilities.Player;
using Interfaces.Core.Managers;
using Interfaces.Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using Interfaces.UI;

namespace Core.Managers
{
    [RequireComponent(typeof(IGameStateManager))]
    [RequireComponent(typeof(ILoadLevelSceneHandle))]
    [RequireComponent(typeof(IHUD))]
    [RequireComponent(typeof(IPlayerPickUpAbilities))]
    [RequireComponent(typeof(IPlayerAbilities))]
    public class LevelManager : LevelManagerBase, ILevelManager
    {
        #region GettersAndSetters

        // Exposed API's Calls External
        public ISoundManagerExtras GetSoundManager => _soundManager;
        public ILoadLevelSceneHandle GetLoadLevelSceneHandler => _loadLevelSceneHandler;
        public IGameStateManager GetGameStateManager => _gameStateManager;
        public IHUD GetHUD => _hud;
        public IPlayerPickUpAbilities GetPlayerPickUpAbilities => _playerPickUpAbilities;
        public IPlayerAbilities GetPlayerAbilities => _playerAbilities;
        public PlayerController GetPlayerController => _playerController;
        public ILevelServices GetLevelServices => _levelServices;
        public IGameStateData GetGameStateData => _gameStateData;
        public ISetTimerHUD GetSetTimerHUD => _setTimerHUD;

        #endregion

        private IGameStateManager _gameStateManager;
        private ISoundManagerExtras _soundManager;
        private PlayerController _playerController; //TODO IPlayerController
        private ILoadLevelSceneHandle _loadLevelSceneHandler;
        private IHUD _hud;
        private IPlayerPickUpAbilities _playerPickUpAbilities;
        private IPlayerAbilities _playerAbilities;
        private ILevelServices _levelServices;
        private IGameStateData _gameStateData;
        private ISetTimerHUD _setTimerHUD;

        private void Awake()
        {
            _soundManager = FindObjectOfType<SoundManager>();
            _loadLevelSceneHandler = GetComponent<ILoadLevelSceneHandle>();
            _gameStateManager = FindObjectOfType<GameStateManager>();
            _hud = GetComponent<IHUD>();
            _playerPickUpAbilities = GetComponent<IPlayerPickUpAbilities>();
            _playerAbilities = GetComponent<IPlayerAbilities>();
            _levelServices = GetComponent<ILevelServices>();
            _gameStateData = GetComponent<IGameStateData>();
            _setTimerHUD = GetComponent<ISetTimerHUD>();

            Time.timeScale = 1;
        }

        private void Start()
        {
            RetrieveGameState();

            Debug.Log(this.name + " Start: current scene is " + SceneManager.GetActiveScene().name);
        }

        private void OnEnable()
        {
            Coin.OnCoinCollected += _playerPickUpAbilities.AddCoin;
            Starman.OnStarmanCollected += _playerAbilities.MarioInvincibleStarman;
            OneUpMushroom.OnOneUpCollected += _playerPickUpAbilities.AddLife;
            PowerupObject.OnPowerUpCollected += _playerAbilities.MarioPowerUp;
        }

        private void OnDisable()
        {
            Coin.OnCoinCollected -= _playerPickUpAbilities.AddCoin;
            Starman.OnStarmanCollected -= _playerAbilities.MarioInvincibleStarman;
            OneUpMushroom.OnOneUpCollected -= _playerPickUpAbilities.AddLife;
            PowerupObject.OnPowerUpCollected -= _playerAbilities.MarioPowerUp;
        }

        public void RetrieveGameState()
        {
            _gameStateData.Lives = _gameStateManager.Lives;
            _gameStateData.PlayerSize = _gameStateManager.PlayerSize;
            _gameStateData.HurryUp = _gameStateManager.HurryUp;
            _hud.Coins = _gameStateManager.Coins;
            _hud.Scores = _gameStateManager.Scores;
            _gameStateData.TimeLeft = _gameStateManager.TimeLeft;

            _playerController = FindObjectOfType<PlayerController>();
            PlayerAnimator.PlayerAnimatorComponent = _playerController.gameObject.GetComponent<Animator>();
            _playerAbilities.PlayerRigidbody2D = _playerController.gameObject.GetComponent<Rigidbody2D>();
            _playerController.UpdateSize();

            GetSoundManager.GetSoundVolume();

            _hud.SetHUD();
            _soundManager.GetSoundLevelHandle.ChangeMusic(_gameStateData.HurryUp
                ? GetSoundManager.LevelMusicHurry
                : GetSoundManager.LevelMusic);
        }

        private void Update()
        {
            _setTimerHUD.TimerHUD();

            _setTimerHUD.TimerHUDMusic();

            _setTimerHUD.TimeUpCounter();

            _gameStateData.GetPauseUnPauseGame.GamePauseCheck();
        }
    }
}