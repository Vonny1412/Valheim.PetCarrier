using UnityEngine;

namespace PetCarrier.Components
{
    internal sealed class CratePlacement : MonoBehaviour
    {
        private Player player;
        private ItemDrop.ItemData item;
        private GameObject marker;
        private Vector3 position;
        private bool validPosition;
        private int startedFrame;

        private static readonly int PlacementMask = LayerMask.GetMask(
            "Default",
            "static_solid",
            "Default_small",
            "piece",
            "piece_nonsolid",
            "terrain",
            "Water",
            "vehicle"
        );

        internal static void Begin(Player player, ItemDrop.ItemData item)
        {
            CratePlacement placement = player.GetComponent<CratePlacement>();

            if (!placement)
            {
                placement = player.gameObject.AddComponent<CratePlacement>();
            }

            if (placement.item == item)
            {
                return;
            }

            placement.StartPlacement(player, item);
        }

        private void StartPlacement(Player owner, ItemDrop.ItemData crate)
        {
            StopPlacement();

            player = owner;
            item = crate;
            startedFrame = Time.frameCount;

            if (player.m_placeMarker)
            {
                marker = Instantiate(player.m_placeMarker);
                marker.SetActive(false);
            }
            else
            {
                Plugin.Log.LogWarning("Placement marker prefab not found.");
            }
        }

        private void Update()
        {
            if (!player || item == null || !marker)
            {
                StopPlacement();
                return;
            }

            if (Player.m_localPlayer != player)
            {
                StopPlacement();
                return;
            }

            if (!player.IsItemEquiped(item) || !player.GetInventory().ContainsItem(item))
            {
                StopPlacement();
                return;
            }

            validPosition = false;

            if (GameCamera.instance)
            {
                Transform cameraTransform = GameCamera.instance.transform;
                Ray ray = new(cameraTransform.position, cameraTransform.forward);
                const float maxDistance = 10f;

                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, PlacementMask))
                {
                    position = hit.point;
                    validPosition = true;

                    marker.transform.position = position;

                    Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
                    marker.transform.rotation = Quaternion.LookRotation(hit.normal, forward);
                }
            }

            marker.SetActive(validPosition);

            if (Time.frameCount == startedFrame)
            {
                return;
            }
        }

        private void StopPlacement()
        {
            if (marker)
            {
                Destroy(marker);
                marker = null;
            }

            player = null;
            item = null;
            validPosition = false;
        }

        private void OnDestroy()
        {
            StopPlacement();
        }

        internal static bool IsPlacing(ItemDrop.ItemData item)
        {
            Player player = Player.m_localPlayer;
            if (!player || item == null)
            {
                return false;
            }
            CratePlacement placement = player.GetComponent<CratePlacement>();
            return placement && placement.item == item;
        }

        internal static bool IsPlacing()
        {
            Player player = Player.m_localPlayer;
            if (!player)
            {
                return false;
            }
            CratePlacement placement = player.GetComponent<CratePlacement>();
            return placement && placement.item != null;
        }

        internal static void TryPlace()
        {
            Player player = Player.m_localPlayer;
            if (!player)
            {
                return;
            }

            CratePlacement placement = player.GetComponent<CratePlacement>();
            if (!placement || placement.item == null || !placement.validPosition)
            {
                return;
            }

            if (CrateUnpacking.TryUnpack(player, placement.item, placement.position))
            {
                placement.StopPlacement();
            }
        }

        /*
        // deprecated
        private static bool TryGetLiquidPoint(Ray ray, float maxDistance, out Vector3 point)
        {
            point = default;

            const float step = 0.25f;
            Vector3 previousPoint = ray.origin;

            for (float distance = step; distance <= maxDistance; distance += step)
            {
                Vector3 currentPoint = ray.GetPoint(distance);

                float liquidLevel = Floating.GetLiquidLevel(currentPoint, 1f, LiquidType.All);

                if (liquidLevel > -10000f && previousPoint.y >= liquidLevel && currentPoint.y <= liquidLevel)
                {
                    float previousHeight = previousPoint.y - liquidLevel;
                    float currentHeight = currentPoint.y - liquidLevel;
                    float t = previousHeight / (previousHeight - currentHeight);

                    point = Vector3.Lerp(previousPoint, currentPoint, t);
                    point.y = Floating.GetLiquidLevel(point, 1f, LiquidType.All);

                    return true;
                }

                previousPoint = currentPoint;
            }

            return false;
        }
        */

    }
}
