global using AurieSharpInterop;
global using YYTKInterop;

namespace KeepModding
{

    namespace Data
    {

        /// <summary>
        /// Defines a contract for inspecting game objects and state.
        /// </summary>
        public interface IGameInspector
        {
            /// <summary>
            /// Inspects a global game object and prints its details.
            /// </summary>
            void InspectGlobalObject();
        }

    }
}

