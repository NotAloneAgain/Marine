using Exiled.API.Features;
using Exiled.API.Features.Components;
using MEC;
using Mirror;
using SCPSLAudioApi;
using SCPSLAudioApi.AudioCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Marine.Misc.API
{
    public static class AudioExtensions
    {
        private static readonly HashSet<ReferenceHub> _dummies;
        private static bool _initialized;

        static AudioExtensions()
        {
            _dummies = new(100);
        }

        public static IEnumerator<float> _PlayAudio(this string audioFile, string tag)
        {
            if (!_initialized)
            {
                Startup.SetupDependencies();

                _initialized = true;
            }

            if (tag.HasAudio())
            {
                yield break;
            }

            var prefab = Object.Instantiate(NetworkManager.singleton.playerPrefab);

            var conn = new FakeAudioConn(_dummies.Count + 1);

            var hub = prefab.GetComponent<ReferenceHub>();

            _dummies.Add(hub);

            NetworkServer.AddPlayerForConnection(conn, prefab);

            hub.characterClassManager.InstanceMode = ClientInstanceMode.Unverified;

            while (hub.nicknameSync == null)
            {
                hub.nicknameSync = hub.GetComponent<NicknameSync>() ?? hub.gameObject.AddComponent<NicknameSync>();

                yield return Timing.WaitForSeconds(0.005f);
            }

            hub.nicknameSync.MyNick = tag;

            var audio = AudioPlayerBase.Get(hub);

            var path = Path.Combine(Path.Combine(Paths.Exiled, "Music"), audioFile);

            Log.Info($"Try playing {path}...");

            audio.LogDebug = false;
            audio.BroadcastChannel = VoiceChat.VoiceChatChannel.Intercom;
            audio.Volume = 32;
            audio.Loop = true;

            audio.CurrentPlay = path;
            audio.Play(-1);
        }

        public static void StopAudio()
        {
            foreach (var dummy in _dummies)
            {
                Timing.CallDelayed(0.1f, () => NetworkServer.Destroy(dummy.gameObject));
            }
        }

        public static bool HasAudio(this string tag) => _dummies.FirstOrDefault(dummy => dummy.nicknameSync.MyNick == tag) != null;
    }
}
