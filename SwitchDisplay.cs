﻿using NAudio.CoreAudioApi;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace SwitchDisplay
{
    public class SwitchDisplay : Plugin
    {
        
        private static readonly ILogger logger = LogManager.GetLogger();

        private SwitchDisplaySettings settings { get; set; }

        private IPlayniteAPI api { get; set; }

        public override Guid Id { get; } = Guid.Parse("75b4d2cc-8308-4c34-8aeb-4dd9a012586d");

        private PolicyConfigClient _policyConfigClient;

        public MMDeviceEnumerator AudioEnumerator { get; set; }

        public DisplayHandler Handler { get; set; }

        public SwitchDisplay(IPlayniteAPI api) : base(api)
        {
            settings = new SwitchDisplaySettings(this);
            this.api = api;
            Handler = new DisplayHandler();
            AudioEnumerator = new MMDeviceEnumerator();
            _policyConfigClient = new PolicyConfigClient();
        }

        public override IEnumerable<ExtensionFunction> GetFunctions()
        {
            return new List<ExtensionFunction>
            {
                
            };
        }

        public override void OnGameInstalled(Game game)
        {
            // Add code to be executed when game is finished installing.
        }

        public override void OnGameStarted(Game game)
        {
            // Add code to be executed when game is started running.
        }

        public override void OnGameStarting(Game game)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameStopped(Game game, long elapsedSeconds)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameUninstalled(Game game)
        {
            // Add code to be executed when game is uninstalled.
        }

        public override void OnApplicationStarted()
        {
            if(api.ApplicationInfo.Mode == ApplicationMode.Fullscreen)
            {

                logger.Info("OnApplicationStarted:: FullscreenDisplay:: " + settings.FullscreenDisplay);
                //Display
                if (settings.SwitchDisplays && !String.IsNullOrEmpty(settings.FullscreenDisplay))
                {
                    if (!Handler.SwitchPrimaryDisplay(settings.FullscreenDisplay))
                    {
                        logger.Error(String.Format("Error setting primary display: {0}", settings.FullscreenDisplay));
                    }
                    
                }

                //Audio
                if (settings.SwitchAudio && settings.FullScreenAudioDeviceList.Count > 0)
                {
                    //search available device
                    foreach(KeyValuePair<string, string> device in settings.FullScreenAudioDeviceList)
                    {
                        string id = "";
                        if (settings.EnumerateAudioDevices.ContainsKey(device.Key))
                        {
                            id = device.Key;
                        } else if (settings.EnumerateAudioDevices.ContainsValue(device.Value))
                        {
                            id = settings.EnumerateAudioDevices.First(p => p.Value == device.Value).Value;
                        }
                        if(id.Length > 0)
                        {
                            _policyConfigClient.SetDefaultEndpoint(id, Role.Multimedia);
                            break;
                        }

                    }
                    
                }
            }

        }

        public override void OnApplicationStopped()
        {
            if (api.ApplicationInfo.Mode == ApplicationMode.Fullscreen)
            {
                //Display
                if (settings.SwitchDisplays && !String.IsNullOrEmpty(settings.DefaultDisplay))
                {
                    if (!Handler.SwitchPrimaryDisplay(settings.DefaultDisplay))
                    {
                        logger.Error(String.Format("Error setting primary display: {0}", settings.DefaultDisplay));
                    }

                }
                //Audio
                if (settings.SwitchAudio && !String.IsNullOrEmpty(settings.DefaultAudioDevice))
                {
                    _policyConfigClient.SetDefaultEndpoint(settings.DefaultAudioDevice, Role.Multimedia);
                }
            }
        }

        public override void OnLibraryUpdated()
        {
            // Add code to be executed when library is updated.
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new SwitchDisplaySettingsView(settings);
        }

    }
}