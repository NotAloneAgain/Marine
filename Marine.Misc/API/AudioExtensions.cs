using Exiled.API.Features;
using MEC;
using Mirror;
using SCPSLAudioApi;
using SCPSLAudioApi.AudioCore;
using System.IO;
using UnityEngine;

namespace Marine.Misc.API
{
    public static class AudioExtensions
    {
        private static ReferenceHub _dummy;
        private static bool _initialized;

        public static void PlayAudio(this string audioFile)
        {
            if (HasAudio())
            {
                return;
            }

            if (!_initialized)
            {
                Startup.SetupDependencies();

                _initialized = true;
            }

            GameObject prefab = Object.Instantiate(NetworkManager.singleton.playerPrefab);

            var conn = new FakeAudioConn(0);

            ReferenceHub hub = prefab.GetComponent<ReferenceHub>();

            _dummy = hub;

            _ = NetworkServer.AddPlayerForConnection(conn, prefab);

            hub.characterClassManager.InstanceMode = ClientInstanceMode.Unverified;

            hub.nicknameSync = hub.GetComponent<NicknameSync>() ?? hub.gameObject.AddComponent<NicknameSync>();

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
            if (!HasAudio())
            {
                return;
            }

            _ = Timing.CallDelayed(0.5f, delegate ()
            {
                NetworkServer.Destroy(_dummy.gameObject);

                _dummy = null;
            });
        }

        public static bool HasAudio()
        {
            return _dummy != null;
        }
    }
}
