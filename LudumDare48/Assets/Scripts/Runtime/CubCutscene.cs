using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
    public class CubCutscene : MonoBehaviour
    {
        [SerializeField]
        private Cinemachine.CinemachineVirtualCamera virtualCamera;
        [SerializeField]
        private float duration;
        [SerializeField]
        private Animator cubAnim;

        private PlayerController player;

        static readonly string cubAnimation = "Panicked_Cutscene";

        private void OnTriggerEnter2D(Collider2D other) 
        {
            player = other.GetComponent<PlayerController>();

            StartCoroutine(ShowCutscene());
        }
        IEnumerator ShowCutscene()
        {
            virtualCamera.enabled = true;
            player.SetPaused(true);

            yield return new WaitForSeconds(0.4f);

            cubAnim.Play(cubAnimation);

            yield return new WaitForSeconds(duration);

            virtualCamera.enabled = false;
            player.SetPaused(false);
            gameObject.SetActive(false);
        }
    }
}