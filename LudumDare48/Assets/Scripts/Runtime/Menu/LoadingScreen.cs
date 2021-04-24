using TMPro;
using UnityEngine;
using DG.Tweening;

namespace LD48
{
    ///<summary>The loading screen as an addressable instance class.</summary>
    public class LoadingScreen : MonoBehaviour
    {
        private static LoadingScreen monoInstance;

        [SerializeField]
        private RectTransform backgroundImage;

        [SerializeField]
        CanvasGroup itemGroup;
        [SerializeField]
        private RectTransform[] loadingBlobs;
        [SerializeField]
        float phaseOffset, blobTimeScale, blobMinSize, blobMaxSize;
        [SerializeField]
        private TextMeshProUGUI loadingText;

        // Start is called before the first frame update
        void Awake()
        {
            if(monoInstance)
                Destroy(gameObject);
            else
            {
                monoInstance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private System.Collections.IEnumerator Start() 
        {
            DOTween.Init(true, true, LogBehaviour.ErrorsOnly).SetCapacity(10, 50);
            this._Show();
            yield return new WaitForSeconds(3);
            this._Hide();
        }

        private void Update()
        {
            //Theres gotta be a better way of doing this lmao.
            string loadTxt;
            float rTime = Time.time * 3f % 3f;
            if(rTime <= 1f)
                loadTxt = "Loading.";
            else if(rTime <= 2f)
                loadTxt = "Loading..";
            else
                loadTxt = "Loading...";
            loadingText.text = loadTxt;

            float blobTime = Time.time * blobTimeScale;
            for(int i = 0; i < loadingBlobs.Length; i++)
            {
                float phase = (float)i * phaseOffset * Mathf.Deg2Rad;
                phase += blobTime;
                //phase *= Mathf.Deg2Rad;
                float blobSize = Mathf.Lerp(blobMinSize, blobMaxSize, Mathf.Sin(phase));
                loadingBlobs[i].localScale = new Vector3(blobSize, blobSize, 1);
            }
        }

        public static void Show() => monoInstance._Show();

        private void _Show()
        {
            Sequence showSequence = DOTween.Sequence();
            //Move the background into place.
            showSequence.Append(backgroundImage.DOAnchorPos(Vector2.zero, duration: 1.0f));
            //show the other items in the loading screen.
            showSequence.Append(itemGroup.DOFade(endValue: 1.0f, duration: 0.4f));
            showSequence.PlayForward();
        }

        public static void Hide() => monoInstance._Hide();
        
        private void _Hide()
        {
            Sequence hideSequence = DOTween.Sequence();
            //hide the other items in the loading screen.
            hideSequence.Append(itemGroup.DOFade(endValue: 0.0f, duration: 0.4f));
            //Move the background away from the screen
            hideSequence.Append(backgroundImage.DOAnchorPos(new Vector2(0, 1500), duration: 1.0f));
            hideSequence.PlayForward();
        }

        public static void Create()
        {
            if(monoInstance)
                return;
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScreen");
        }

    }
}