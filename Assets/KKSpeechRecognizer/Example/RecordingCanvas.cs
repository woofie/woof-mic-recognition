using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using KKSpeech;

public class RecordingCanvas : MonoBehaviour {

	public Button startRecordingButton;
	public Text resultText;
	public string[] blocks;
	public int score1, score2;
	public string[] commands = {
		"sit",
		"stay",
		"walk",
		"good",
		"bad",
		"dead", // play dead
		"speak"
	};
	public string command;

	void Start() {
		if (SpeechRecognizer.ExistsOnDevice()) {
			SpeechRecognizerListener listener = GameObject.FindObjectOfType<SpeechRecognizerListener>();

			/* Add a bunch of listeners for all events from speech recognition. */
			listener.onAuthorizationStatusFetched.AddListener(OnAuthorizationStatusFetched);
			listener.onAvailabilityChanged.AddListener(OnAvailabilityChange);
			listener.onErrorDuringRecording.AddListener(OnError);
			listener.onErrorOnStartRecording.AddListener(OnError);
			listener.onFinalResults.AddListener(OnFinalResult);
			listener.onPartialResults.AddListener(OnPartialResult);
			listener.onEndOfSpeech.AddListener(OnEndOfSpeech);
			startRecordingButton.enabled = false;

			/* As for permission to record. */
			SpeechRecognizer.RequestAccess();
		} else {
			resultText.text = "Sorry, but this device doesn't support speech recognition";
			startRecordingButton.enabled = false;
		}
	}

	public void OnFinalResult(string result) {
		resultText.text = result;
		score1 = 0;
		score2 = 0;

		command = "";

		/* Analyse the words the user says for the activation phrase. This bypasses NLP. */
		blocks = resultText.text.Split (' ');

		/* Only test the first ten words. */
		for (int i=0; i < ((blocks.Length > 10) ? 10 : blocks.Length); i++) {
			if (blocks [i] == "dog" || blocks [i] == "dawg" || blocks[i] == "doggy" || blocks[i] == "doggie" || blocks[i] == "Dogg") {
				score1++;
			} else if (blocks [i] == "hey" || blocks[i] == "hi" || blocks[i] == "hello" || blocks[i] == "good" || blocks[i] == "bad") {
				score2++;
			}
		}

		/* Score must be at least 2 for it to recognise command mode. */
		if (score1 >= 1 && score2 >= 1) {
			for (int i = 0; i < (blocks.Length); i++) {
				for (int j = 0; j < (commands.Length); j++) {
					if (blocks [i] == commands [j]) {
						command = commands [j];
						break;
					}
				}
			}
		}

		if (command != "") {
			resultText.text = "COMMAND RECOGNISED: " + command;
		} else {
			resultText.text = "NO COMMAND, YOU: " + resultText.text;
		}
	}

	public void OnPartialResult(string result) {
		resultText.text = result;
	}

	public void OnAvailabilityChange(bool available) {
		startRecordingButton.enabled = available;
		if (!available) {
			resultText.text = "Speech Recognition not available";
		} else {
			resultText.text = "Say something :-)";
		}
	}

	public void OnAuthorizationStatusFetched(AuthorizationStatus status) {
		switch (status) {
		case AuthorizationStatus.Authorized:
			startRecordingButton.enabled = true;
			break;
		default:
			startRecordingButton.enabled = false;
			resultText.text = "Cannot use Speech Recognition, authorization status is " + status;
			break;
		}
	}

	public void OnEndOfSpeech() {
		startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
	}

	public void OnError(string error) {
		Debug.LogError(error);
		resultText.text = "Something went wrong... Try again! \n [" + error + "]";
		startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
	}

	public void OnStartRecordingPressed() {
		if (SpeechRecognizer.IsRecording()) {
			SpeechRecognizer.StopIfRecording();
			startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
		} else {
			SpeechRecognizer.StartRecording(true);
			startRecordingButton.GetComponentInChildren<Text>().text = "Stop Recording";
			resultText.text = "Say something :-)";
		}
	}
}
