using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicNPCPaintings.Framework
{
    public class SavedDataManager
    {
        public Dictionary<string, string> FurnitureData;

        public Dictionary<string, string> TextureData;

        public SavedDataManager() 
        {
            FurnitureData = new Dictionary<string, string>();
            TextureData = new Dictionary<string, string>();
            ModEntry.modHelper.Events.GameLoop.SaveLoaded += SaveDataManager_OnSaveLoaded;
            ModEntry.modHelper.Events.GameLoop.Saving += SaveDataManager_OnSaving;
        }

        public void SaveDataManager_OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            FurnitureData.Clear();
            TextureData.Clear();

            if (!Context.IsMainPlayer)
                return;

            SavedDataManager manager = ModEntry.modHelper.Data.ReadSaveData<SavedDataManager>("DataManager");

            if (manager == null)
                return;

            ModEntry.instance.Monitor.Log("Found Saved Data");
            FurnitureData = manager.FurnitureData;
            TextureData = manager.TextureData;
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
            ModEntry.instance.Helper.Data.WriteSaveData<SavedDataManager>("DataManager", this);
        }
    }
}
