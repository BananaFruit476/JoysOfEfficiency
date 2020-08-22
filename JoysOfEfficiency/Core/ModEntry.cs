﻿using System.IO;
using JoysOfEfficiency.EventHandler;
using JoysOfEfficiency.Harmony;
using JoysOfEfficiency.Huds;
using JoysOfEfficiency.ModCheckers;
using JoysOfEfficiency.Utils;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace JoysOfEfficiency.Core
{
    using Player = Farmer;


    /// <summary>
    /// This class is a representation of the mod itself.
    /// </summary>
    internal class ModEntry : Mod
    {
        public static bool IsCoGOn { get; private set; }
        public static bool IsCaOn { get; private set; }
        private static Config Conf => InstanceHolder.Config;
        public static bool DebugMode { get; private set; }

        private static readonly Logger Logger = new Logger("Main");

        /// <summary>
        /// Called firstly when SMAPI finished loading of the mod.
        /// </summary>
        /// <param name="helper"></param>
        public override void Entry(IModHelper helper)
        {


            // Loads configuration from file.
            Config conf = Helper.ReadConfig<Config>();

            // Initialize Logger
            Logger.Init(Monitor);

            // Initialize InstanceHolder.
            InstanceHolder.Init(this, conf);



            // Register events.
            EventHolder.RegisterEvents(Helper.Events);


            // Registration commands.
            Helper.ConsoleCommands.Add("joedebug", "Debug command for JoE", OnDebugCommand);
            Helper.ConsoleCommands.Add("joerelcon", "Reloading config command for JoE", OnReloadConfigCommand);


            // Limit config values.
            ConfigLimitation.LimitConfigValues();

            // Check mod compatibilities.
            if(ModChecker.IsCoGLoaded(helper))
            {
                Logger.Log("CasksOnGround detected.");
                IsCoGOn = true;
            }

            if (ModChecker.IsCaLoaded(helper))
            {
                Logger.Log("CasksAnywhere detected.");
                IsCaOn = true;
            }

            // Do patching stuff
            if (!Conf.SafeMode)
            {
                HarmonyPatcher.DoPatching();
            }

            helper.WriteConfig(Conf);
            MineIcons.Init(helper);
        }

        private static void OnReloadConfigCommand(string name, string[] args)
        {
            // Loads configuration from file.
            InstanceHolder.LoadConfig();
            Logger.Log("Reloaded JoE's config.");
        }

        private static void OnDebugCommand(string name, string[] args)
        {
            DebugMode = !DebugMode;

        }

        public string GetFilePath(string fileName)
        {
            return Path.Combine(Helper.DirectoryPath, fileName);
        }
    }
}
