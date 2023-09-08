using Exiled.API.Features;
using MEC;
using Mirror;
using SCPSLAudioApi;
using SCPSLAudioApi.AudioCore;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Marine.Misc.API
{
    public static class AudioExtensions
    {
        private static ReferenceHub _dummy;
        private static bool _initialized;

        public static IEnumerator<float> _PlayAudio(this string audioFile)
        {
            if (!_initialized)
            {
                Startup.SetupDependencies();

                _initialized = true;
            }

            if (HasAudio())
            {
                yield break;
            }

            var prefab = Object.Instantiate(NetworkManager.singleton.playerPrefab);

            var conn = new FakeAudioConn(0);

            var hub = prefab.GetComponent<ReferenceHub>();

            _dummy = hub;

            NetworkServer.AddPlayerForConnection(conn, prefab);

            hub.characterClassManager.InstanceMode = ClientInstanceMode.Unverified;

            while (hub.nicknameSync == null)
            {
                hub.nicknameSync = hub.GetComponent<NicknameSync>() ?? hub.gameObject.AddComponent<NicknameSync>();

                yield return Timing.WaitForSeconds(0.005f);
            }

            hub.nicknameSync.MyNick = "Музончик? Он самый!";

            var audio = AudioPlayerBase.Get(hub);

            var path = Path.Combine(Path.Combine(Paths.Exiled, "Music"), audioFile);

            Log.Info($"Try playing {path}...");

            audio.LogDebug = false;
            audio.BroadcastChannel = VoiceChat.VoiceChatChannel.Intercom;
            audio.Volume = 3;
            audio.Loop = true;

            audio.CurrentPlay = path;
            audio.Play(-1);
        }

        public static void StopAudio()
        {
            Timing.CallDelayed(0.5f, () => NetworkServer.Destroy(_dummy.gameObject));

            _dummy = null;
        }

        public static bool HasAudio() => _dummy != null;
    }
}
