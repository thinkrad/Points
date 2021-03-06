﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointCanvas : Singleton<PointCanvas> {
	Texture2D tex;
	public Color32[] colorMap;
	public List<ColorList> flashMap;
	StateRelay stateRelay;
	public float flashTime = 0.06f; 
	float time;
	int flashIndex = 0;

	void Start() {
		GlobalEvents.stateRelayCreated += () => {
			StateRelay.Instance.enterVictoryState += () => {
				ClearDisplay();
				renderer.enabled = false;
			};
			StateRelay.Instance.enterPlayState += () => renderer.enabled = true;
		};
		tex = new Texture2D(Mix.Instance.CanvasSize, Mix.Instance.CanvasSize);
		tex.filterMode = FilterMode.Point;
		renderer.material.mainTexture = tex;
		ClearDisplay();
		renderer.enabled = true;

		enabled = false;
		GlobalEvents.stateRelayCreated += () => enabled = true;
	}

	void Update() {
		if (StateRelay.Instance.state == State.play) {
			UpdateDisplay(StateRelay.Instance.canvas);
		}
	}
	
	public void UpdateDisplay(char[] image) {
		time += Time.deltaTime;
		if (time > flashTime) {
			flashIndex = (flashIndex + 1) % (flashMap[0].colors.Count - 1);
			time -= flashTime;
		}
		tex.SetPixels32(IndexedImageToColor(image));
		tex.Apply();
	}

	Color32[] IndexedImageToColor(char[] image) {
		var colorImage = new Color32[(int)Mathf.Pow(Mix.Instance.CanvasSize, 2)];
		int i = 0;
		foreach (var colorIndex in image) {
			colorImage[i] = flashMap[colorIndex].colors[flashIndex];
			i++;
		}
		return colorImage;
	}

	public void ClearDisplay() {
		var blackness = new Color32[(int)Mathf.Pow(Mix.Instance.CanvasSize, 2)];
		for (int i = 0; i < (int)Mathf.Pow(Mix.Instance.CanvasSize, 2); i++) {
			blackness[i] = Color.black;
		}
		tex.SetPixels32(blackness);
		tex.Apply();
	}
}

[System.Serializable]
public class ColorList {
	public List<Color32> colors;
}