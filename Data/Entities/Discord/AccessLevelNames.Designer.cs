﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data.Entities.Discord {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class AccessLevelNames {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AccessLevelNames() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Data.Entities.Discord.AccessLevelNames", typeof(AccessLevelNames).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 😇 Админ.
        /// </summary>
        internal static string Admin {
            get {
                return ResourceManager.GetString("Admin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 🧑 Продвинутый.
        /// </summary>
        internal static string Advanced {
            get {
                return ResourceManager.GetString("Advanced", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 👶 Базовый.
        /// </summary>
        internal static string Default {
            get {
                return ResourceManager.GetString("Default", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 👷 Помощник.
        /// </summary>
        internal static string Helper {
            get {
                return ResourceManager.GetString("Helper", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ❓ Неизвестно.
        /// </summary>
        internal static string Unknown {
            get {
                return ResourceManager.GetString("Unknown", resourceCulture);
            }
        }
    }
}
