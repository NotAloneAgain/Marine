using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Mirror;
using System.Linq;
using UnityEngine;

namespace Marine.Misc.Handlers
{
    internal sealed class WarheadHandlers
    {
        #region Initialize
        private static readonly string[] _optionalObjects;
        private static readonly string[] _additionalObjects;

        static WarheadHandlers()
        {
            _optionalObjects = new[]
            {
                "Canvas", "Point light", "TMP SubMeshUI [CHINESE Material + fallback1 Atlas]", "Text (TMP)", "Stator",
                "Frame", "CameraPosition", "Cylinder", "All", "Reflection Probe", "szklo", "Label (1)", "collider",
                "Lockdown", "camera", "obracadlo", "cctvmodel", "MotorStator", "rotor", "Side Colliders", "newmodel(Clone)",
                "GFX", "Cube", "cctv1", "Cylinder.001", "Cylinder.002", "Lens", "Lamp", "Clutter", "079 Camera", "Quad",
                "Point light (1)", "head", "ElevatorPanel", "RID_C", "RID", "Collider_Glass", "Lock_Down", "Lock_Up",
                "MovablePart", "RailingCollider", "Scp079Speaker", "Point light (2)", "Cube (1)", "Second Digit",
                "First Digit", "Reverb Zone", "Lighting", "Lamp (2)", "Lamp (1)", "pilka", "body", "Lights", "monitor",
                "Quad (1)", "BTPar", "Monitor", "Gas", "Labels", "Cube (2)", "Plane", "Text", "longstand", "Spawnpoints",
                "default", "cctventrance", "Point light (3)", "TyreBurnoutSmoke (3)", "Background", "Body", "Desk", "Seat",
                "Cube (3)", "Keyboard", "EV_Lamp", "Plane (2)", "Obj", "TyreBurnoutSmoke", "RailingColliders", "Collider",
                ";)", "Safe Teleport Position #1", "Label", "AntijumperCells", "PrisonChamber", "TyreBurnoutSmoke (2)",
                "PrisonLight (1)"
            };

            _additionalObjects = new[]
            {
                "SCP-914 Controller",
                "DecontaminationManager",
                "Tesla Gate Controller",
                "LightControllerObject",
                "FlickerableLightController"
            };
        }
        #endregion
        #region Handler
        public void OnDetonated()
        {
            var door = Door.Get(DoorType.NukeSurface);

            door.Lock(900, DoorLockType.Warhead);
            door.IsOpen = true;

            foreach (var item in Pickup.List)
            {
                if (item.Room != null && item.Room.Zone == ZoneType.Surface)
                {
                    continue;
                }

                item.Destroy();
            }

            foreach (var ragdoll in Ragdoll.List)
            {
                if (ragdoll.Room == null || ragdoll.Room.Zone == ZoneType.Surface)
                {
                    continue;
                }

                ragdoll.Destroy();
            }

            OptimizeEverything();
        }
        #endregion
        #region Optimization
        private static void OptimizeEverything()
        {
            foreach (GameObject gameObject in Object.FindObjectsOfType<GameObject>().Distinct())
            {
                if (gameObject == null || !gameObject.activeSelf)
                {
                    continue;
                }

                if (Player.Get(gameObject) != null)
                {
                    continue;
                }

                if (IsGameObjectCanBeInactive(gameObject))
                {
                    gameObject.SetActive(false);

                    var identity = gameObject.GetComponent<NetworkIdentity>();

                    if (identity == null)
                    {
                        continue;
                    }

                    identity.enabled = false;

                    Object.Destroy(identity);

                    continue;
                }

                if (gameObject.tag != "Untagged")
                    continue;

                if (_additionalObjects.Contains(gameObject.name))
                {
                    gameObject.SetActive(false);

                    var identity = gameObject.GetComponent<NetworkIdentity>();

                    if (identity == null)
                    {
                        continue;
                    }

                    identity.enabled = false;

                    Object.Destroy(identity);
                }
            }
        }

        private static bool IsGameObjectCanBeInactive(GameObject gameObject)
        {
            if (gameObject.tag is "GeneratorSpawn" or "Finish" or "AnticheatIgnore" or "LiftTarget" || gameObject.tag.Contains("Door") || gameObject.tag.Contains("Button"))
            {
                return true;
            }

            return _optionalObjects.Contains(gameObject.name) || gameObject.name.Contains("Button") || gameObject.name.Contains("Door") || gameObject.name.Contains("Intercom");
        }

        [System.Obsolete]
        private static bool IsGameObjectCanBeDestroyed(GameObject gameObject)
        {
            var position = gameObject.transform.position;

            return position.y is > 1050 or < -50 && (position.y is > 2 or < -2 || position.x is > 10 or < -10 || position.z is > 10 or < -10);
        }
        #endregion
    }
}
