global using AurieSharpInterop;
global using YYTKInterop;
using System.Runtime.CompilerServices;

namespace ManagedMod
{
    namespace Generic
    {

        /// <summary>
        /// Defines a contract for inspecting game objects and state.
        /// </summary>
        public interface IGameInspector
        {


            void Initialize(AurieManagedModule IModule);
            void finish(AurieManagedModule IModule);
            /// <summary>
            /// Inspects a global game object and prints its details.
            /// </summary>
            abstract static void InspectGlobalObject();
            /// <summary>
            /// Inspects the currently running room and prints its details.
            /// </summary>
            abstract static void InspectRunningRoom();
        }

        public interface IInputManager
        {
            /// <summary>
            /// Sets a binding for a player, overwriting the binding that was already there.
            /// </summary>. 
            /// 
            /// <description>
            /// If `forGamepad`
            /// is specified, the binding will be set for gamepad devices, otherwise the binding will be set
            /// for the keyboard and mouse device (`INPUT_KBM`). The alternate index parameter can be used
            /// to set multiple parallel bindings for one verb.
            /// 
            /// This function, unlike `InputBindingSet()`, will try to automatically resolve conflicts. This
            /// function is effective for simple control schemes but may fail in more complex situations; in
            /// these cases, you’ll need to handle conflict resolution yourself.
            /// </description>

            /// <reference>
            /// https://github.com/offalynne/Input/blob/master/scripts/InputBindingSetSafe/InputBindingSetSafe.gml
            /// </reference>
            AurieStatus AddPostInputHook(AurieManagedModule IModule, string Input, AfterScriptCallbackHandler Callback);
            void InputEvent(CodeExecutionContext context);

            // AurieStatus InputBindingSetSafe(bool forGamepad, Delegate yourFunction, GameVariable binding, short alternateA = 0, short playerIndex = 0);
            /// <param name="verb">The name of the action to bind (e.g., "jump").</param>
            /// <param name="forGamepad">Whether this binding is for a gamepad.</param>
            /// <param name="binding">The binding to set (e.g., a key, a mouse button).</param>
            /// <param name="alternateIndex">The alternate binding index (0 or 1).</param>
            /// <param name="playerIndex">The local player index.</param>
            /// <returns>A status indicating success or failure.</returns>
        }

    }

    namespace Core
    {
        public interface IPlayground
        {
            void Initialize(AurieManagedModule IModule);
        }
    }
}

// namespace YYTK
// {
//     public interface IYYTKInterface
//     {
//         AurieStatus Create();
//         void Destroy();
//         short[] QueryVersion();
//         int GetNamedRoutineIndex(string FunctionName);
//         nint GetNamedRoutinePointer(string FunctionName);

//     }

// }