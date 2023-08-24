﻿using RoR2;
using RoR2.Artifacts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace csProj.Artifacts.EvilSystem
{
    class PackSpawner
    {
		public static void randomPack()
        {
            System.Random r = new System.Random();
			Xoroshiro128Plus xoro = new Xoroshiro128Plus((uint)new System.Random().Next(-int.MaxValue, int.MaxValue));

			switch (r.Next(0, 11))
            {
				case (0): // acrid pack
					spawnPack2(xoro, "CrocoMonsterMaster", 20, "The Doggo Squad");
					break;
				case (1): // huntress pack
					spawnPack2(xoro, "HuntressMonsterMaster", 20, "Huntress Horde");
					break;
				case (2): // commando pack
					spawnPack2(xoro, "CommandoMonsterMaster", 20, "Commando Crusade");
					break;
				case (3): // Mage pack
					spawnPack2(xoro, "MageMonsterMaster", 20, "Masterful Mages");
					break;
				case (4): // Toolbot pack
					spawnPack2(xoro, "ToolbotMonsterMaster", 20, "THE TERMINATORS");
					break;
				case (5): // Railgunner
					spawnPack2(xoro, "RailgunnerMonsterMaster", 20, "Faze Squad");
					break;
				case (6): // Bandit
					spawnPack2(xoro, "BanditMonsterMaster", 20, "Highway Robbers");
					break;
				case (7): // Engi
					spawnPack2(xoro, "BanditMonsterMaster", 20, "Geek Gang");
					break;
				case (8): // TreeBot
					spawnPack2(xoro, "TreebotMonsterMaster", 20, "Spoder Swarm");
					break;
				case (9): // Captain
					spawnPack2(xoro, "CaptainMonsterMaster", 20, "NASA");
					break;
				case (10): // crab
					spawnPack2(xoro, "VoidSurvivorMonsterMaster", 20, "Void Avatars");
					break;
			}
		}

		public static void spawnPack2(Xoroshiro128Plus rng, String masterTargetString, int quantity, String packName)
        {
			for (int i = CharacterMaster.readOnlyInstancesList.Count - 1; i >= 0; i--)
			{
				CharacterMaster characterMaster = CharacterMaster.readOnlyInstancesList[i];
				if (characterMaster.teamIndex == TeamIndex.Player && characterMaster.playerCharacterMasterController)
				{
					for (int x = 0; x < quantity; x++)
					{
						CreateDoppelgangerStub(characterMaster, rng, masterTargetString, packName);
					}
				}
			}
		}


		public static void CreateDoppelgangerStub(CharacterMaster srcCharacterMaster, Xoroshiro128Plus rng, String masterTargetString, String packName)
        {
			DoppelgangerSpawnCard spawnCard = DoppelgangerSpawnCard.FromMaster(srcCharacterMaster);
			if (!spawnCard)
			{
				return;
			}
			Transform spawnOnTarget;
			DirectorCore.MonsterSpawnDistance input;
			if (TeleporterInteraction.instance)
			{
				spawnOnTarget = TeleporterInteraction.instance.transform;
				input = DirectorCore.MonsterSpawnDistance.Close;
			}
			else
			{
				spawnOnTarget = srcCharacterMaster.GetBody().coreTransform;
				input = DirectorCore.MonsterSpawnDistance.Far;
			}
			DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule
			{
				spawnOnTarget = spawnOnTarget,
				placementMode = DirectorPlacementRule.PlacementMode.NearestNode
			};

			// CHANGE SPAWNCARD PARAMETERS TO MATCH NEEDED PARAMETERS
			CharacterMaster[] masters = UnityEngine.Resources.FindObjectsOfTypeAll<RoR2.CharacterMaster>();
			CharacterMaster targetMaster = null;
			foreach (CharacterMaster m in masters)
			{
				Debug.Log(m.name);
				if (m.name.Equals(masterTargetString))
				{
					targetMaster = m;
				}
			}

			// CHANGE SPAWNCARD PARAMETERS TO MATCH NEEDED PARAMETERS
			Debug.Log("targetMaster = " + targetMaster);
			spawnCard.srcCharacterMaster = targetMaster;
			spawnCard.prefab = MasterCatalog.GetMasterPrefab(targetMaster.masterIndex);
			spawnCard.name = packName;

			DirectorCore.GetMonsterSpawnDistance(input, out directorPlacementRule.minDistance, out directorPlacementRule.maxDistance);
			DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnCard, directorPlacementRule, rng);
			directorSpawnRequest.teamIndexOverride = new TeamIndex?(TeamIndex.Monster);
			directorSpawnRequest.ignoreTeamMemberLimit = true;
			CombatSquad combatSquad = null;
			DirectorSpawnRequest directorSpawnRequest2 = directorSpawnRequest;
			directorSpawnRequest2.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest2.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult result)
			{
				if (!combatSquad)
				{
					combatSquad = UnityEngine.Object.Instantiate<GameObject>(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/Encounters/ShadowCloneEncounter")).GetComponent<CombatSquad>();
				}
				combatSquad.AddMember(result.spawnedInstance.GetComponent<CharacterMaster>());
			}));
			DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
			if (combatSquad)
			{
				NetworkServer.Spawn(combatSquad.gameObject);
			}
			UnityEngine.Object.Destroy(spawnCard);
		}






        public static void spawnPack()
        {
			
			Debug.Log("Prefabs/CharacterBodies/CommandoBody? " + Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody"));
			//SpawnCard card = SpawnCard.CreateInstance<SpawnCard>();
			MasterCopySpawnCard card = MasterCopySpawnCard.FromMaster(LocalUserManager.GetFirstLocalUser().currentNetworkUser.GetCurrentBody().master, false, false, null);
			card.prefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody");
			card.directorCreditCost = 0;
			card.name = "Void Warrior";

            RoR2.DirectorCore.spawnedObjects.Capacity = 99999;
			RoR2.SceneDirector.cardSelector.Capacity = 99999;

			/*
			CharacterMaster[] masters = UnityEngine.Resources.FindObjectsOfTypeAll<RoR2.CharacterMaster>();
			CharacterMaster targetMaster = null;
			foreach (CharacterMaster m in masters)
			{
				Debug.Log(m.name);
				if (m.name.Equals("CommandoMonsterMaster"))
				{
					targetMaster = m;
				}
			}
			*/

			Transform spawnOnTarget;
			DirectorCore.MonsterSpawnDistance input;
			if (TeleporterInteraction.instance)
			{
				spawnOnTarget = TeleporterInteraction.instance.transform;
				input = DirectorCore.MonsterSpawnDistance.Close;
			}
			else
			{
				spawnOnTarget = LocalUserManager.GetFirstLocalUser().currentNetworkUser.GetCurrentBody().coreTransform;
				input = DirectorCore.MonsterSpawnDistance.Far;
			}
			DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule
			{
				spawnOnTarget = spawnOnTarget,
				placementMode = DirectorPlacementRule.PlacementMode.NearestNode
			};

			DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(card, directorPlacementRule, new Xoroshiro128Plus((uint)new System.Random().Next(-int.MaxValue, int.MaxValue)));
            directorSpawnRequest.ignoreTeamMemberLimit = true;
			directorSpawnRequest.teamIndexOverride = TeamIndex.Monster;
            Debug.Log(directorSpawnRequest.teamIndexOverride.ToString());
			DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
		}
    }
}
