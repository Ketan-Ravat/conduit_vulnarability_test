﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Jarvis.Service.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ResourceMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ResourceMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Jarvis.Service.Resources.ResourceMessages", typeof(ResourceMessages).Assembly);
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
        ///   Looks up a localized string similar to Asset is not exist.
        /// </summary>
        internal static string AssetNotFound {
            get {
                return ResourceManager.GetString("AssetNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error In Create Issue.
        /// </summary>
        internal static string Errorincreateissue {
            get {
                return ResourceManager.GetString("Errorincreateissue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error in Transaction History.
        /// </summary>
        internal static string ErrorinTranHistory {
            get {
                return ResourceManager.GetString("ErrorinTranHistory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error in update inspection status.
        /// </summary>
        internal static string Errorinupdateinspectionstatus {
            get {
                return ResourceManager.GetString("Errorinupdateinspectionstatus", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inspection is not exist.
        /// </summary>
        internal static string InspectionNotFound {
            get {
                return ResourceManager.GetString("InspectionNotFound", resourceCulture);
            }
        }
    }
}
