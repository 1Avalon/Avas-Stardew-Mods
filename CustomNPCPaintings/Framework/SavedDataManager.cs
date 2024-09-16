using CustomNPCPaintings.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DynamicNPCPaintings.Framework
{
    public class SavedDataManager
    {
        public Dictionary<string, string> FurnitureData;

        public Dictionary<string, string> TextureData; //key = uniqueName value = file

        public Dictionary<string, NetworkPictureData> PictureData;

        public string SaveFolderName = "";
        private SavedDataManager(bool init = false) 
        {
            if (!init)
                return;

            PictureData = new Dictionary<string, NetworkPictureData>();
            FurnitureData = new Dictionary<string, string>();
            TextureData = new Dictionary<string, string>();
            ModEntry.modHelper.Events.GameLoop.SaveLoaded += SaveDataManager_OnSaveLoaded;
            ModEntry.modHelper.Events.GameLoop.Saving += SaveDataManager_OnSaving;
            ModEntry.modHelper.Events.Multiplayer.PeerConnected += OnPeerConnected;
            ModEntry.modHelper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
        }


        public SavedDataManager(Dictionary<string, string> furnitureData, Dictionary<string, string> textureData, Dictionary<string, NetworkPictureData> pictureData, string saveFolderName)
        {
            FurnitureData = furnitureData;
            TextureData = textureData;
            PictureData = pictureData;
            SaveFolderName = saveFolderName;
        }

        public static SavedDataManager Create()
        {
            return new SavedDataManager(true);
        }
        private void Log(string message)
        {
            ModEntry.instance.Monitor.Log(message);
        }

        public void OnPeerConnected(object sender, PeerConnectedEventArgs e)
        {
            SaveFolderName = Constants.SaveFolderName;
            ModEntry.modHelper.Multiplayer.SendMessage<SavedDataManager>(this, "DataManager", new string[] { ModEntry.instance.ModManifest.UniqueID}, new long[] {e.Peer.PlayerID});
            Log($"Farmhand {e.Peer.PlayerID} connected. Sending Data Manager....");
        }
        public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != ModEntry.instance.ModManifest.UniqueID)
                return;

            if (Context.IsMainPlayer)
            {
                if (e.Type == "CustomPaintingData")
                {
                    Log(Constants.SaveFolderName);
                    NetworkPictureData data = e.ReadAs<NetworkPictureData>();
                    string uniqueId = TextureHelper.ExportToPainting(data, false);
                    Log($"Received painting data from {e.FromPlayerID}. Sending Data Manager with {PictureData.Count} custom paintings");
                    ModEntry.modHelper.Multiplayer.SendMessage<SavedDataManager>(this, "DataManager", new string[] { ModEntry.instance.ModManifest.UniqueID });
                    ModEntry.instance.Helper.Multiplayer.SendMessage<string>(uniqueId, "UniquePaintingId", new string[] {ModEntry.instance.ModManifest.UniqueID});
                }
            }
            else
            {
                if (e.Type == "DataManager")
                {
                    ModEntry.dataManager = e.ReadAs<SavedDataManager>();
                    Log($"Received DataManager... Found {PictureData.Count} custom paintings...s");
                }


                else if (e.Type == "UniquePaintingId")
                {
                    Log("Received UniquePaintingId from host");
                    Log(Constants.SaveFolderName);
                    string uniqueId = e.ReadAs<string>();
                    Furniture furniture = new Furniture(uniqueId, Vector2.Zero);
                    List<Item> items = new List<Item>()
                    {
                        furniture
                    };
                    Game1.activeClickableMenu = new ItemGrabMenu(items);
                }
            }
            ModEntry.instance.Helper.GameContent.InvalidateCache("Data/Furniture");
        }
        public void SaveDataManager_OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            FurnitureData.Clear();
            TextureData.Clear();
            PictureData.Clear();

            if (!Context.IsMainPlayer)
                return;

            SavedDataManager manager = ModEntry.modHelper.Data.ReadSaveData<SavedDataManager>("DataManager");

            if (manager == null)
                return;

            ModEntry.instance.Monitor.Log("Found Saved Data");
            FurnitureData = manager.FurnitureData;
            TextureData = manager.TextureData;
            PictureData = manager.PictureData == null ? new Dictionary<string, NetworkPictureData>() : manager.PictureData; //thats for all the people who already made a save

            ModEntry.instance.Helper.GameContent.InvalidateCache("Data/Furniture");
        }

        public void SaveDataManager_OnSaving(object sender, SavingEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                Save();
                ModEntry.instance.Monitor.Log("Successfully saved data");
            }
        }

        public void Save()
        {
            if (Context.IsMultiplayer)
                ModEntry.modHelper.Multiplayer.SendMessage<SavedDataManager>(this, "DataManager", new string[] { ModEntry.instance.ModManifest.UniqueID });

            ModEntry.instance.Helper.Data.WriteSaveData<SavedDataManager>("DataManager", this);
        }
    }
}
