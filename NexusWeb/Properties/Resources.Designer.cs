﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NexusWeb.Properties {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NexusWeb.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Disable.
        /// </summary>
        internal static string DisableText {
            get {
                return ResourceManager.GetString("DisableText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enable.
        /// </summary>
        internal static string EnableText {
            get {
                return ResourceManager.GetString("EnableText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://fireeagle.yahooapis.com/oauth/request_token.
        /// </summary>
        internal static string FireEagle_UnauthorizedRequestToken {
            get {
                return ResourceManager.GetString("FireEagle_UnauthorizedRequestToken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to http\:\/\/www.google.com/latitude/apps/badge/api\?user=([0-9-]*).
        /// </summary>
        internal static string GoogleLatitudeIdentiferParseRegex {
            get {
                return ResourceManager.GetString("GoogleLatitudeIdentiferParseRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Location sharing is currently disabled. Your contacts will not be able to view where you are..
        /// </summary>
        internal static string LocationDisabledWarning {
            get {
                return ResourceManager.GetString("LocationDisabledWarning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Location sharing is currently disabled..
        /// </summary>
        internal static string LocationStatusDisabled {
            get {
                return ResourceManager.GetString("LocationStatusDisabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Location sharing is currently enabled..
        /// </summary>
        internal static string LocationStatusEnabled {
            get {
                return ResourceManager.GetString("LocationStatusEnabled", resourceCulture);
            }
        }
    }
}
