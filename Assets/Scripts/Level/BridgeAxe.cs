using System.Collections;
using System.Collections.Generic;
using Core.Managers;
using Core.NPC;
using Core.Player;
using Interfaces.Core.Managers;
using UnityEngine;

namespace Level
{
	public class BridgeAxe : MonoBehaviour {
		private ILevelManager _levelManager;
		private PlayerController _playerController;
		private Bowser _bowser;
		private List<GameObject> _bridgePieces = new List<GameObject> ();

		private float bridgePieceGravity = 8;
		private float waitBetweenCollapse = .2f;
		private bool _gotAxe;

		private void Start () {
			_levelManager = FindObjectOfType<LevelManager> ();
			_playerController = FindObjectOfType<PlayerController> ();
			_bowser = FindObjectOfType<Bowser> ();

			foreach (Transform child in transform.parent.Find("Bridge Pieces")) {
				_bridgePieces.Add (child.gameObject);
			}
		}


		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!other.CompareTag("Player") || _gotAxe) return;
			_gotAxe = true;
			_playerController.FreezeUserInput ();
			_levelManager.GetGameStateData.TimerPaused = true;

			if (_bowser) {  // bowser not yet defeated
				_bowser.active = false; 
				StartCoroutine (CollapseBridgeCo ());
			} else {
				_levelManager.GetLevelServices.MarioCompleteCastle ();
			}
			gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		}

		private IEnumerator CollapseBridgeCo() {
			foreach (GameObject bridgePiece in _bridgePieces) {
				bridgePiece.layer = LayerMask.NameToLayer ("Falling to Kill Plane");
				Rigidbody2D rgbd = bridgePiece.gameObject.GetComponent<Rigidbody2D> ();
				rgbd.bodyType = RigidbodyType2D.Dynamic;
				rgbd.gravityScale = bridgePieceGravity;
				_levelManager.GetSoundManager.SoundSource.PlayOneShot (_levelManager.GetSoundManager.BreakBlockSound);
				yield return new WaitForSeconds (waitBetweenCollapse);
			}
			_levelManager.GetLevelServices.MarioCompleteCastle ();
			Destroy (gameObject);
		}
	}
}
