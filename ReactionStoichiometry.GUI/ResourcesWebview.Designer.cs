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
        ///p.cre {
        ///    margin-left: auto;
        ///    margin-right: auto;
        ///    width: 80%;
        ///    /*font-family: monospace;*/
        ///    font-family: &apos;IBM Plex Sans&apos;;
        ///    font-size: larger;
        ///    text-align: center;
        ///    border: 1px solid gray;
        ///    padding: 10px;
        ///    word-wrap: break-word;
        ///}
        ///
        ///table.matrix {
        ///  margin-left: auto;
        ///  margin-right: auto;
        ///  border: 2px solid #FFFFFF;
        ///  text-align: center;
        ///  border-collapse: collapse;
        ///  font-family: monospace;
        ///  table-layout: fix [rest of string was truncated]&quot;;.
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
        ///	&lt;link href=&quot;https://fonts.googleapis.com/css2?family=IBM+Plex+Sans&amp;display=swap&quot; rel=&quot;stylesheet&quot;&gt;&lt;link rel=&quot;stylesheet&quot; type=&quot;text/css&quot; href=&quot;ReactionStoichiometry.JsonViewer.css&quot;&gt;
        ///    &lt;style type=&apos;text/css&apos;&gt;%cssContent%&lt;/style&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;script type=&apos;text/javascript&apos;&gt;
        ///        %jsContent%;
        ///        const json [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string htmlContent {
            get {
                return ResourceManager.GetString("htmlContent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to const INTERPUNCT = &quot;\u00B7&quot;;
        ///const EQUILIBRIUM = &apos;\u21CC&apos;;
        ///
        ///function MakeJsonReadable(Equation, identifier) {
        ///  Equation.Substances = Equation.Substances.map((substance) =&gt;
        ///    substance.replace(/(\d+(\.\d+)?)/g, &quot;&lt;sub&gt;$1&lt;/sub&gt;&quot;)
        ///  );
        ///
        ///  Equation.Labels = Equation.Labels.map((label) =&gt;
        ///    label.replace(/(\d+(\.\d+)?)/g, &quot;&lt;sub&gt;$1&lt;/sub&gt;&quot;)
        ///  );
        ///
        ///  if (Equation.RowsBasedSolution.AlgebraicExpressions) {
        ///    Equation.RowsBasedSolution.AlgebraicExpressions =
        ///      Equation.RowsBasedSolution.Algebrai [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string jsContent {
            get {
                return ResourceManager.GetString("jsContent", resourceCulture);
            }
        }
    }
}
