using System;
using System.Collections.Generic;
using UnityEngine;

namespace PetCarrier
{
    internal static class CratePacking
    {

        private sealed class PackReservation
        {
            internal long Sender;
            internal string RequestId;
            internal ZDOID PlayerId;
            internal float CreatedAt;
        }

        private sealed class PendingPack
        {
            internal Inventory Inventory;
            internal ItemDrop.ItemData Item;
            internal ZDOID CreatureId;
            internal float CreatedAt;
            internal bool Committed;
        }

        private const string RpcPack = "PetCarrier_Pack";
        private const string RpcPackAccepted = "PetCarrier_PackAccepted";
        private const string RpcPackCommit = "PetCarrier_PackCommit";
        private const string RpcPackResult = "PetCarrier_PackResult";
        private const float PackTimeout = 10f;
        private static readonly Dictionary<ZDOID, PackReservation> Reservations = new Dictionary<ZDOID, PackReservation>();
        private static readonly Dictionary<string, PendingPack> PendingPacks = new Dictionary<string, PendingPack>();

        internal static void RegisterCreatureRPC(Character creature)
        {
            ZNetView nview = creature.GetComponent<ZNetView>();
            if (!nview || !nview.IsValid())
                return;
            
            nview.Register<string, ZDOID>(RpcPack, (sender, requestId, playerId) => { RPC_Pack(creature, sender, requestId, playerId); });
            nview.Register<string>(RpcPackCommit, (sender, requestId) => { RPC_PackCommit(creature, sender, requestId); });
        }

        internal static void RegisterPlayerRPC(Player player)
        {
            ZNetView nview = player.GetComponent<ZNetView>();
            if (!nview || !nview.IsValid())
                return;
            
            nview.Register<string>(RpcPackAccepted, (sender, requestId) => { RPC_PackAccepted(player, sender, requestId); });
            nview.Register<string, bool>(RpcPackResult, (sender, requestId, success) => { RPC_PackResult(player, sender, requestId, success); });
        }

        private static void RPC_Pack(Character creature, long sender, string requestId, ZDOID playerId)
        {
            if (!creature || string.IsNullOrEmpty(requestId))
                return;

            ZNetView nview = creature.GetComponent<ZNetView>();
            if (!nview || !nview.IsValid() || !nview.IsOwner())
                return;

            ZDOID creatureId = nview.GetZDO().m_uid;

            CleanupExpired();
            if (Reservations.ContainsKey(creatureId))
                return;

            if (!CanPack(creature))
                return;

            GameObject playerObject = ZNetScene.instance.FindInstance(playerId);
            if (!playerObject || !playerObject.TryGetComponent<Player>(out var player))
                return;

            ZNetView playerNview = player.GetComponent<ZNetView>();
            if (!playerNview || !playerNview.IsValid())
                return;

            // Ensure the sender actually owns the specified player object.
            if (playerNview.GetZDO().GetOwner() != sender)
                return;

            Reservations.Add(creatureId, new PackReservation
            {
                Sender = sender,
                RequestId = requestId,
                PlayerId = playerId,
                CreatedAt = Time.realtimeSinceStartup
            });

            playerNview.InvokeRPC(sender, RpcPackAccepted, requestId);
        }

        private static void RPC_PackAccepted(Player player, long sender, string requestId)
        {
            if (!player || player != Player.m_localPlayer)
                return;

            if (!PendingPacks.TryGetValue(requestId, out var pending))
                return;

            GameObject creatureObject = ZNetScene.instance.FindInstance(pending.CreatureId);
            if (!creatureObject)
                return;

            ZNetView creatureNview = creatureObject.GetComponent<ZNetView>();
            if (!creatureNview || !creatureNview.IsValid())
                return;

            if (creatureNview.GetZDO().GetOwner() != sender)
                return;

            if (!pending.Inventory.ContainsItem(pending.Item))
            {
                PendingPacks.Remove(requestId);
                // item no longer exists
                return;
            }

            if (!pending.Inventory.RemoveOneItem(pending.Item))
            {
                PendingPacks.Remove(requestId);
                Plugin.Log.LogWarning("Failed to remove empty crate.");
                return;
            }

            pending.Committed = true;
            pending.CreatedAt = Time.realtimeSinceStartup;

            creatureNview.InvokeRPC(sender, RpcPackCommit, requestId);
        }

        private static void RPC_PackCommit(Character creature, long sender, string requestId)
        {
            if (!creature)
                return;

            ZNetView nview = creature.GetComponent<ZNetView>();
            if (!nview || !nview.IsValid() || !nview.IsOwner())
                return;

            ZDOID creatureId = nview.GetZDO().m_uid;

            if (!Reservations.TryGetValue(creatureId, out var reservation))
                return;

            if (reservation.Sender != sender || reservation.RequestId != requestId)
                return;

            Reservations.Remove(creatureId);

            bool success = PackCreature(creature);
            if (!success)
                Plugin.Log.LogWarning($"Failed to pack creature: {creature.name}");

            GameObject playerObject = ZNetScene.instance.FindInstance(reservation.PlayerId);
            if (playerObject && playerObject.TryGetComponent<Player>(out var player))
            {
                ZNetView playerNview = player.GetComponent<ZNetView>();
                if (playerNview && playerNview.IsValid())
                    playerNview.InvokeRPC(reservation.Sender, RpcPackResult, requestId, success);
            }
        }

        private static void RPC_PackResult(Player player, long sender, string requestId, bool success)
        {
            if (!player || player != Player.m_localPlayer)
                return;

            if (!PendingPacks.TryGetValue(requestId, out var pending))
                return;

            if (!pending.Committed)
                return;

            GameObject creatureObject = ZNetScene.instance.FindInstance(pending.CreatureId);
            if (creatureObject)
            {
                ZNetView creatureNview = creatureObject.GetComponent<ZNetView>();
                if (creatureNview && creatureNview.IsValid() && creatureNview.GetZDO().GetOwner() != sender)
                    return;
            }

            PendingPacks.Remove(requestId);

            if (success)
            {
                Plugin.Log.LogInfo($"Creature packed successfully: {requestId}");
            }
            else
            {
                Plugin.Log.LogWarning($"Creature packing failed: {requestId}");
                player.Message(MessageHud.MessageType.Center, "Failed to pack creature.");

                GameObject prefab = ZNetScene.instance.GetPrefab(Plugin.Constants.EmptyPrefabName);
                if (!prefab || !pending.Inventory.AddItem(prefab, 1))
                {
                    Plugin.Log.LogError($"Failed to refund empty crate: {requestId}");
                }
            }
        }




        private static bool PackCreature(Character creature)
        {
            if (!creature || !ZNetScene.instance)
                return false;

            ZNetView creatureNview = creature.GetComponent<ZNetView>();
            if (!creatureNview || !creatureNview.IsValid() || !creatureNview.IsOwner())
                return false;

            if (!CanPack(creature))
                return false;

            GameObject cratePrefab = ZNetScene.instance.GetPrefab(Plugin.Constants.FilledPrefabName);
            if (!cratePrefab)
            {
                Plugin.Log.LogWarning("Filled crate prefab not found.");
                return false;
            }

            GameObject crate = null;

            try
            {
                ZDO creatureZDO = creatureNview.GetZDO();

                var creatureData = CrateZDO.GetData(creatureZDO);
                string serializedData = CrateZDO.Stringify(creatureData);
                string creatureName = creatureZDO.GetString(ZDOVars.s_tamedName, creature.m_name);
                var creatureLevel = creature.GetLevel();
                if (creatureLevel > 0)
                {
                    creatureName += $" {creatureLevel-1}★";
                }

                Vector3 spawnPosition = creature.transform.position + Vector3.up * 0.25f;
                Quaternion spawnRotation = Quaternion.Euler(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(-8f, 8f));
                crate = UnityEngine.Object.Instantiate(cratePrefab, spawnPosition, spawnRotation);

                ZNetView crateNview = crate.GetComponent<ZNetView>();
                if (!crateNview || !crateNview.IsValid() || !crateNview.IsOwner())
                {
                    Plugin.Log.LogWarning("Filled crate has no valid owned ZNetView.");
                    return false;
                }

                ZDO crateZDO = crateNview.GetZDO();
                crateZDO.Set(Plugin.ZDOVars.z_creaturePrefab, Utils.GetPrefabName(creature.name));
                crateZDO.Set(Plugin.ZDOVars.z_creatureData, serializedData);
                crateZDO.Set(Plugin.ZDOVars.z_creatureName, creatureName);
                CrateItemData.SetSource(crateZDO);

                ItemDrop crateItemDrop = crate.GetComponent<ItemDrop>();
                if (!crateItemDrop)
                {
                    Plugin.Log.LogWarning("Filled crate has no ItemDrop component.");
                    return false;
                }

                CrateItemData.TransferFromZDO(crateItemDrop.m_itemData, crateZDO);
                CrateItemData.SetCrateSharedData(crateItemDrop.m_itemData);

                // The filled crate is ready. Remove the creature.
                creatureNview.Destroy();

                // Keep the successfully created crate.
                crate = null;

                return true;
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to pack creature: {ex}");
                return false;
            }
            finally
            {
                if (crate)
                {
                    ZNetView nview = crate.GetComponent<ZNetView>();

                    if (nview && nview.IsValid() && nview.IsOwner())
                        nview.Destroy();
                    else
                        UnityEngine.Object.Destroy(crate);
                }
            }
        }

        internal static void TryPack(Player player, Inventory inventory, ItemDrop.ItemData item, Character creature)
        {
            if (!player || !creature || item == null || !ZNetScene.instance)
                return;

            inventory ??= player.GetInventory();

            if (!inventory.ContainsItem(item))
                return;

            CleanupExpired();
            if (PendingPacks.Count > 0)
                return;

            if (!CanPack(creature, player))
                return;

            ZNetView creatureNview = creature.GetComponent<ZNetView>();
            ZNetView playerNview = player.GetComponent<ZNetView>();

            if (!creatureNview || !creatureNview.IsValid() || !playerNview || !playerNview.IsValid() || !playerNview.IsOwner())
                return;

            string requestId = Guid.NewGuid().ToString("N");

            PendingPacks.Add(requestId, new PendingPack
            {
                Inventory = inventory,
                Item = item,
                CreatureId = creatureNview.GetZDO().m_uid,
                CreatedAt = Time.realtimeSinceStartup
            });

            creatureNview.InvokeRPC(RpcPack, requestId, playerNview.GetZDO().m_uid);
        }




        private static bool CanPack(Character creature, Player player = null)
        {
            if (!creature)
            {
                return false;
            }

            ZNetView nview = creature.GetComponent<ZNetView>();
            if (!nview || !nview.IsValid())
            {
                return false;
            }

            if (!creature.IsTamed())
            {
                player?.Message(MessageHud.MessageType.Center, Plugin.Constants.TranslationKey_MessageNotTamed);
                return false;
            }

            if (creature.TryGetComponent<BaseAI>(out var baseAI))
            {
                if (baseAI.IsAlerted())
                {
                    player?.Message(MessageHud.MessageType.Center, Plugin.Constants.TranslationKey_MessageIsAlerted);
                    return false;
                }

                if (baseAI.IsAggravated())
                {
                    player?.Message(MessageHud.MessageType.Center, Plugin.Constants.TranslationKey_MessageIsAggravated);
                    return false;
                }
            }

            if (nview.GetZDO().GetLong(ZDOVars.s_pregnant, 0L) != 0)
            {
                player?.Message(MessageHud.MessageType.Center, Plugin.Constants.TranslationKey_MessageIsPregnant);
                return false;
            }

            return true;
        }

        internal static void Clear()
        {
            Reservations.Clear();
            PendingPacks.Clear();
        }

        internal static void CleanupExpired()
        {
            float now = Time.realtimeSinceStartup;

            List<string> expiredRequests = new List<string>();

            foreach (var entry in PendingPacks)
            {
                if (!entry.Value.Committed && now - entry.Value.CreatedAt > PackTimeout)
                    expiredRequests.Add(entry.Key);
            }

            foreach (string requestId in expiredRequests)
                PendingPacks.Remove(requestId);

            List<ZDOID> expiredReservations = new List<ZDOID>();

            foreach (var entry in Reservations)
            {
                if (now - entry.Value.CreatedAt > PackTimeout)
                    expiredReservations.Add(entry.Key);
            }

            foreach (ZDOID creatureId in expiredReservations)
                Reservations.Remove(creatureId);

            // TODO: Handle committed pack requests that never receive RpcPackResult.
            //       Prevent stale PendingPacks entries from blocking future pack attempts.
            //       Do not automatically refund the crate, as packing may have succeeded.
        }

    }
}
