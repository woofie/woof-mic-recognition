using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KKSpeech;

/* Attach this to a button. */

public class RecordAudio : MonoBehaviour {
	private Text instruction;
	private Button recordingButton;

	SpeechRecognizerListener listener;
	

	void Start() {
		/* And get and set up the UI elements. */
		instruction = GetComponentsInChildren<Text>()[0];

		Button recordingButton = GetComponent<Button>();
		recordingButton.onClick.AddListener(this.recordMic);

		/* Create the SpeechRecognitionListener. */
		listener = GameObject.FindObjectOfType<SpeechRecognizerListener>();
		listener.onEndOfSpeech.AddListener(this.speechEnd);
	}

	void Update() {
		instruction.text = SpeechRecognizer.IsRecording() ? "Stop." : "Record!";
	}

	void recordMic() {
		if (SpeechRecognizer.IsRecording()) {
			SpeechRecognizer.StopIfRecording();
		} else {
			SpeechRecognizer.StartRecording(true);
		}
	}

	void speechEnd() {
		Debug.Log("Recognition complete.");
	}
}