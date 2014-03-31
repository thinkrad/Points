﻿using UnityEngine;
using System.Collections;

public class CommandRelay : MonoBehaviour {
	public int teamIndex;
	public int points;
	public KeyCode[] keys;
	bool isMyRelay;

	void OnNetworkInstantiate(NetworkMessageInfo info) {
		if (Network.isClient && info.sender == Network.player) {
			// hack to get around the fact that every client has every commandRelay,
			// ultimately they should only be sent between server and client,
			// but that can wait
			isMyRelay = true;
			//Destroy(gameObject);
		}
		if (GlobalEvents.commandRelayCreated != null) GlobalEvents.commandRelayCreated(this, info.sender);
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		stream.Serialize(ref teamIndex);
		stream.Serialize(ref points);
		if (stream.isReading) {
			StateRelay.Instance.UpdatePoints((byte)teamIndex, points);
		}
	}

	void Update() {
		if (Network.isClient && StateRelay.Instance.state == State.play/*&& isMyRelay*/) {
			int i = 0;
			foreach (var key in keys) {
				if (Input.GetKeyDown(key)) {
					points++;
				}
				i++;
			}
		}
	}

	[RPC] 
	void _ReadyToPlay() {
		if (Network.isServer) {
			Server.Instance.ClientIsReady();
		}
	}

	public void ReadyToPlay() {
		networkView.RPC("_ReadyToPlay", RPCMode.Server, null);
	}
}