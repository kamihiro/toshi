using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class FlashcardManager : MonoBehaviour
{
    public RawImage cardImage;
    public Button nextButton;
    public AudioSource audioSource;

    private int currentIndex = -1;

    void Start()
    {
        nextButton.onClick.AddListener(() => StartCoroutine(ShowRandomCard()));
        StartCoroutine(ShowRandomCard());
    }

    IEnumerator ShowRandomCard()
    {
        int newIndex;
        do
        {
            newIndex = Random.Range(1, 41); // 1〜40
        } while (newIndex == currentIndex);

        currentIndex = newIndex;

        string num = newIndex.ToString();
        string imagePath = Path.Combine(Application.streamingAssetsPath, $"Images/f{num}.png");
        string audioPath = Path.Combine(Application.streamingAssetsPath, $"Audio/phrase_{num}.mp3");

        // Load image
        using (UnityWebRequest imgRequest = UnityWebRequestTexture.GetTexture(imagePath))
        {
            yield return imgRequest.SendWebRequest();
            if (!imgRequest.result.Equals(UnityWebRequest.Result.Success))
                Debug.LogError($"画像読み込み失敗: {imgRequest.error}");
            else
                cardImage.texture = DownloadHandlerTexture.GetContent(imgRequest);
        }

        // Load and play audio
        using (UnityWebRequest audioRequest = UnityWebRequestMultimedia.GetAudioClip(audioPath, AudioType.MPEG))
        {
            yield return audioRequest.SendWebRequest();
            if (!audioRequest.result.Equals(UnityWebRequest.Result.Success))
                Debug.LogError($"音声読み込み失敗: {audioRequest.error}");
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(audioRequest);
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
}
