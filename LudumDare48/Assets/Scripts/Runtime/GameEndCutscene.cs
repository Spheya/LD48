using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace LD48
{
    public class GameEndCutscene : MonoBehaviour
    {
        [SerializeField]
        private AudioSource bgmSource;
        [SerializeField]
        private GameObject virtualPlayerCamera;
        [SerializeField]
        private string menuScene = "MainMenu";

        private void Start()
        {
            //call it to be safe.
            LoadingScreen.Create();
        }

        private void OnTriggerEnter2D(Collider2D other) 
        {
            bgmSource.DOFade(0, 5).OnComplete(() => StartCoroutine(LoadingScreen.LoadScene(menuScene)));
            virtualPlayerCamera.SetActive(false);
        }
    }
}