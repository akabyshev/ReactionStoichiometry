﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ReactionStoichiometry.GUI {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ResourcesWebview {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ResourcesWebview() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ReactionStoichiometry.GUI.ResourcesWebview", typeof(ResourcesWebview).Assembly);
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
        ///   Looks up a localized string similar to body {
        ///  font-family: Arial, sans-serif;
        ///}
        ///
        ///.cre p {
        ///    width: 100%;
        ///    font-family: monospace;
        ///}
        ///
        ///table {
        ///  table-layout: fixed;
        ///  width: auto;
        ///  font-family: monospace;
        ///}
        ///
        ///th {
        ///  width: auto; 
        ///  white-space: nowrap;
        ///  text-align: center;
        ///  font-weight: normal;
        ///}
        ///
        ///.vertical-headers th {
        ///    transform: rotate(180deg);
        ///    writing-mode: vertical-lr;
        ///    vertical-align: top;
        ///  }
        ///
        ///thead,
        ///tbody {
        ///  vertical-align: middle;
        ///  text-align: right;
        ///}.
        /// </summary>
        internal static string cssContent {
            get {
                return ResourceManager.GetString("cssContent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///&lt;html lang=&quot;en&quot;&gt;
        ///&lt;head&gt;
        ///	&lt;meta charset=&quot;UTF-8&quot;&gt;
        ///    &lt;meta name=&quot;viewport&quot; content=&quot;width=device-width, initial-scale=1.0&quot;&gt;
        ///    &lt;title&gt;ReactionStoichiometry.JsonViewer&lt;/title&gt;
        ///&lt;style type=&apos;text/css&apos;&gt;
        ///                    %cssContent%
        ///                &lt;/style&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;script type=&apos;text/javascript&apos;&gt;
        ///                    %jsContent%;
        ///const jsonobject = %jsonContent%;
        ///MakeJsonReadable(jsonobject, &quot;Solution&quot;);
        ///                &lt;/script&gt;
        ///&lt;/body&gt;
        ///&lt;/html&gt;.
        /// </summary>
        internal static string htmlContent {
            get {
                return ResourceManager.GetString("htmlContent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function MakeJsonReadable(record, identifier) {
        ///  const recordDiv = document.createElement(&quot;div&quot;);
        ///
        ///  record.Substances = record.Substances.map((substance) =&gt;
        ///    substance.replace(/(\d+(\.\d+)?)/g, &quot;&lt;sub&gt;$1&lt;/sub&gt;&quot;)
        ///  );
        ///
        ///  record.Labels = record.Labels.map(
        ///    (label) =&gt; &quot;&lt;i&gt;&quot; + label.replace(/(\d+(\.\d+)?)/g, &quot;&lt;sub&gt;$1&lt;/sub&gt;&quot;) + &quot;&lt;/i&gt;&quot;
        ///  );
        ///
        ///  if (record.GeneralizedSolution.AlgebraicExpressions) {
        ///    record.GeneralizedSolution.AlgebraicExpressions =
        ///      record.GeneralizedSolution.Algebraic [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string jsContent {
            get {
                return ResourceManager.GetString("jsContent", resourceCulture);
            }
        }
    }
}
