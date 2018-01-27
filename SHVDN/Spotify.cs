using System;
using GTA;
using GTA.Native;
using SpotifyWebHelperAPI;
using System.IO;

namespace SpotifyRage
{
    public class Spotify : Script
    {
        public int RadioStation { get; private set; } = 0;

        // The communication service between SpotifyWebHelper.exe and SHVDN.
        private ISpotifyWebHelperCommunicationService CommunicationService;

        // The Scaleform used by the dashboard.
        private Scaleform DashboardScaleform;

        // Enable this to show the Dashboard on screen.
        private bool DebugDraw = false;

        // This stores the current configuration.
        private ScriptSettings Settings;

        /// <summary>
        /// The constructor for the Spotify class.
        /// </summary>
        public Spotify()
        {
            // Load the latest config.
            LoadConfig();

            // Initialize a connection to the Spotify Web Helper application.
            // TODO: Catch if the App is not running.
            CommunicationService = SpotifyWebHelperApi.Create();

            // Request Dashboard Scaleform Movie
            DashboardScaleform = new Scaleform("dashboard", true);


            // Update on every game tick.
            this.Tick += Update;

            
        }

        /// <summary>
        /// Updates the information. Runs every gametick.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update(object sender, EventArgs e)
        {
            // Check if the player is in a vehicle.
            if (Game.Player.Character.IsInVehicle())
            {
                // Check if the radio channel is set correctly.
                if (Function.Call<int>(Hash.GET_PLAYER_RADIO_STATION_INDEX) == RadioStation)
                {
                    // Get the latest Spotify status.
                    var status = CommunicationService.GetStatus();

                    // Disable radio in car.
                    Function.Call(Hash.SET_FRONTEND_RADIO_ACTIVE, false);
                    Function.Call(Hash.SET_VEHICLE_RADIO_LOUD, false);
                    Function.Call(Hash.SET_VEHICLE_RADIO_ENABLED, false);

                    // Push over the spotify information.
                    PushSpotifyInfo(status.Track.ArtistResource.Name, status.Track.TrackResource.Name, status.Playing);

                    // Draw the dashboard on the screen.
                    if (DebugDraw)
                        DashboardScaleform.Render2D();
                }

                // Check if vehicle radio is disabled or if it is set to be loud.
                if (!Function.Call<bool>(Hash._0x5F43D83FD6738741) || !Function.Call<bool>(Hash._0x032A116663A4D5AC, Game.Player.Character.CurrentVehicle))
                {
                    // Enable radio in car.
                    Function.Call(Hash.SET_FRONTEND_RADIO_ACTIVE, true);
                    Function.Call(Hash.SET_VEHICLE_RADIO_LOUD, true);
                    Function.Call(Hash.SET_VEHICLE_RADIO_ENABLED, true);
                }
            }
        }

        /// <summary>
        /// Loads the config from the hard drive.
        /// </summary>
        public void LoadConfig()
        {
            // Check if config file exists.
            if (!File.Exists(@"scripts\SpotifyRage.ini"))
            {
                // Write to SpotifyRage.ini if it can't be found.
                File.WriteAllLines(@"scripts\SpotifyRage.ini",
                    new string[] { "[Spotify]", "RadioStation=0", "DebugMode = false" });
            }

            // Load the config file.
            Settings = ScriptSettings.Load(@"scripts\SpotifyRage.ini");

            // Set all values.
            DebugDraw = Settings.GetValue<bool>("Spotify", "DebugMode", false);
            RadioStation = Settings.GetValue<int>("Spotify", "RadioStation", 0);

        }

        /// <summary>
        /// Pushes the given info over to the Dashboard scaleform for use ingame.
        /// </summary>
        /// <param name="artist">The artist of the song.</param>
        /// <param name="track">The song name itself.</param>
        /// <param name="playing">Whether the song is playing or not.</param>
        private void PushSpotifyInfo(string artist, string track, bool playing)
        {
            try
            {
                // Call function.
                DashboardScaleform.CallFunction("SET_RADIO",
                        "", playing ? "Spotify" : "Spotify - Paused",
                        artist, track);
            }
            catch (Exception exception)
            {
                UI.Notify(exception.ToString());
            }

        }
    }
}
