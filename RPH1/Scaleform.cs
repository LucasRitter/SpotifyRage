using Rage.Native;

namespace SpotifyRage
{
    class Scaleform
    {
        public int Handle { get; private set; }
        public string ScaleformID { get; private set; }

        public Scaleform(string scaleformId, bool forceLoad)
        {
            if (forceLoad)
                Load(scaleformId);
        }

        public Scaleform(int handle, bool forceLoad = false)
        {
            this.Handle = handle;
        }

        public bool Load(string scaleformId)
        {
            // Request a Scaleform movie identifier from the game.
            int handle = NativeFunction.Natives.RequestScaleformMovie<int>(scaleformId);

            // If a handle was not given, cancel the load process.
            if (handle == 0) return false;

            // Set local values.
            this.Handle = handle;
            this.ScaleformID = scaleformId;

            // Loading was successful, return true.
            return true;
        }

        public void Render2D()
        {
            // Draw a Scaleform movie in Fullscreen with 255 on all RGBA values.
            NativeFunction.Natives.x0DF606929C105BE1(this.Handle, 255, 255, 255, 255);
        }

        public void CallFunction (string function, params object[] arguments)
        {
            // Start calling the function.
            NativeFunction.Natives.xF6E48914C7A8694E(this.Handle, function);

            // Loop through all arguments.
            foreach (object argument in arguments)
            {
                // Do type checking

                // 32bit Integer
                if (argument.GetType() == typeof(int))
                {
                    // Call native GRAPHICS::_PUSH_SCALEFORM_MOVIE_METHOD_PARAMETER_INT
                    NativeFunction.Natives.xC3D0841A0CC546A6((int)argument);
                }

                else if (argument.GetType() == typeof(string))
                {
                    // Call native GRAPHICS::_PUSH_SCALEFORM_MOVIE_METHOD_PARAMETER_STRING
                    NativeFunction.Natives.xBA7148484BD90365((string)argument);
                }

            }
            
            // Pop the function over to the game.
            NativeFunction.Natives.xC6796A8FFA375E53();
        }

        public void CallFunctionArray (string function, params string[] arguments)
        {
            string[] argArray = new string[] { "", "", "", "", "" };

            for (int i = 0; i < 5; i++)
            {
                if (arguments.Length > i && arguments[i] != null)
                {
                    argArray[i] = arguments[i];
                }
            }

            // Call native GRAPHICS::_CALL_SCALEFORM_MOVIE_FUNCTION_STRING_PARAMS
            NativeFunction.Natives.x51BC1ED3CC44E8F7(this.Handle, function,
                argArray[0], argArray[1], argArray[2], argArray[3], argArray[4]);
            
            // Pop the function over to the game.
            NativeFunction.Natives.EndScaleformMovieMethod();
        }
    }
}
