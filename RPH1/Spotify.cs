using System;
using Rage;
using Rage.Native;
using SpotifyWebHelperAPI;

namespace SpotifyRage
{
    public sealed class EntryPoint
    {
        public static int RadioStation { get; private set; } = 0;

        private static ISpotifyWebHelperCommunicationService CommunicationService;
        private static Scaleform DashboardScaleform;

        private static bool DebugDraw = false;

        public static void Main()
        {
            Game.Console.Print($"SpotifyRage is now starting..");

            // Initialize a connection to the Spotify Web Helper application.
            // TODO: Catch if the App is not running.
            Game.Console.Print("Connecting to the Spotify Web Helper application.");
            CommunicationService = SpotifyWebHelperApi.Create();

            // Request Dashboard Scaleform Movie
            DashboardScaleform = new Scaleform("dashboard", true);


            // Update on every game tick.
            Game.FrameRender += Update;

            while (true)
                GameFiber.Yield();
        }

        private static void Update(object sender, GraphicsEventArgs e)
        {
            
            if (Game.LocalPlayer.Character.IsInAnyVehicle(false))
            {
                if (NativeFunction.Natives.GetPlayerRadioStationIndex<int>() == RadioStation)
                {
                    var status = CommunicationService.GetStatus();

                    // Disable radio in car.
                    NativeFunction.Natives.SetFrontendRadioActive(false);
                    NativeFunction.Natives.SetVehicleRadioLoud(Game.LocalPlayer.Character.CurrentVehicle, false);
                    NativeFunction.Natives.SetVehicleRadioEnabled(Game.LocalPlayer.Character.CurrentVehicle, false);

                    PushSpotifyInfo(status.Track.ArtistResource.Name, status.Track.TrackResource.Name, status.Playing);

                    if (DebugDraw)
                        DashboardScaleform.Render2D();
                }

                // Check if vehicle radio is disabled or if it is set to be loud.
                if (!NativeFunction.Natives.x5F43D83FD6738741<bool>() || !NativeFunction.Natives.x032A116663A4D5AC<bool>(Game.LocalPlayer.Character.CurrentVehicle))
                {
                    NativeFunction.Natives.SetFrontendRadioActive(true);
                    NativeFunction.Natives.SetVehicleRadioLoud(Game.LocalPlayer.Character.CurrentVehicle, true);
                    NativeFunction.Natives.SetVehicleRadioEnabled(Game.LocalPlayer.Character.CurrentVehicle, true);
                }
            }
        }
#if DEBUG
        [Rage.Attributes.ConsoleCommand()]
        private static void Command_GetSpotifyInfo()
        {
            var status = CommunicationService.GetStatus();
            Game.Console.Print($"Artist: {status.Track.ArtistResource.Name}, Track: {status.Track.TrackResource.Name}, Is Playing: {status.Playing}");
        }

        [Rage.Attributes.ConsoleCommand()]
        private static void Command_SetTestInfo()
        {
            var status = CommunicationService.GetStatus();
            Game.Console.Print($"Artist: {status.Track.ArtistResource.Name}, Track: {status.Track.TrackResource.Name}, Is Playing: {status.Playing}");
        }

        [Rage.Attributes.ConsoleCommand()]
        private static void Command_GetDashboard()
        {
            Game.Console.Print(DashboardScaleform.Handle.ToString());
        }

        [Rage.Attributes.ConsoleCommand()]
        private static void Command_DrawDebug(bool enabled)
        {
            DebugDraw = enabled;
        }

#endif

        private static void PushSpotifyInfo(string artist, string track, bool playing)
        {
            try
            {
                DashboardScaleform.CallFunction("SET_RADIO",
                        "", playing ? "Spotify" : "Spotify - Paused",
                        artist, track);
            }
            catch (Exception exception)
            {
                Game.Console.Print("Spotify fucked up, " + exception.ToString());
            }
            
        }
    }
}
