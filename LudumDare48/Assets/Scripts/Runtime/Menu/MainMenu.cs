using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace LD48
{
    ///<summary>This class contains most, if not all of the functionality for the main menu.</summary>
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private string gameScene = "Gaming";

        [SerializeField]
        private Button[] uiButtons;

        private void Start() 
        {
            LoadingScreen.Create(); 
        }

        public void PlayGame()
        {
            //disable buttons, then load the scene.
            for(int i = 0; i < uiButtons.Length; i++)
                uiButtons[i].interactable = false;
            SceneManager.LoadScene(gameScene);
        }

        //just a smooth way to get into the game.
        private IEnumerator LoadGame()
        {
            //TODO: fade out the menu music during the transition
            LoadingScreen.Show();
            var operation = SceneManager.LoadSceneAsync(gameScene);
            operation.allowSceneActivation = false;
            //when the scene cant auto activate, progress is halted at 0.9, so check for that.
            yield return new WaitUntil(() => operation.progress >= 0.9f);
            LoadingScreen.Hide();
            operation.allowSceneActivation = true;
        }

        //lol just quit.
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}