﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ten kod został wygenerowany przez narzędzie.
//     Wersja wykonawcza:4.0.30319.42000
//
//     Zmiany w tym pliku mogą spowodować nieprawidłowe zachowanie i zostaną utracone, jeśli
//     kod zostanie ponownie wygenerowany.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RobKalDat {
    using System;
    
    
    /// <summary>
    ///   Klasa zasobu wymagająca zdefiniowania typu do wyszukiwania zlokalizowanych ciągów itd.
    /// </summary>
    // Ta klasa została automatycznie wygenerowana za pomocą klasy StronglyTypedResourceBuilder
    // przez narzędzie, takie jak ResGen lub Visual Studio.
    // Aby dodać lub usunąć składową, edytuj plik ResX, a następnie ponownie uruchom narzędzie ResGen
    // z opcją /str lub ponownie utwórz projekt VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class RobKalResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal RobKalResources() {
        }
        
        /// <summary>
        /// Zwraca buforowane wystąpienie ResourceManager używane przez tę klasę.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RobKalDat.RobKalResources", typeof(RobKalResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Przesłania właściwość CurrentUICulture bieżącego wątku dla wszystkich
        ///   przypadków przeszukiwania zasobów za pomocą tej klasy zasobów wymagającej zdefiniowania typu.
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
        ///   Wyszukuje zlokalizowany zasób typu System.Byte[].
        /// </summary>
        internal static byte[] Messprotokoll_template {
            get {
                object obj = ResourceManager.GetObject("Messprotokoll_template", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        /// Wyszukuje zlokalizowany ciąg podobny do ciągu &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;Project&gt;
        ///  &lt;Basic Company=&quot;&quot; Creater=&quot;&quot; Zone=&quot;&quot; Station=&quot;&quot; ReportPath=&quot;&quot; ReportName=&quot;&quot; LastModified=&quot;&quot; /&gt;
        ///  &lt;Settings XLim=&quot;10&quot; YLim=&quot;10&quot; ZLim=&quot;10&quot; RXLim=&quot;1&quot; RYLim=&quot;1&quot; RZLim=&quot;1&quot; ToleranceX=&quot;25&quot; ToleranceY=&quot;25&quot; ToleranceZ=&quot;25&quot; ToleranceRX=&quot;1.000000&quot; ToleranceRY=&quot;1.000000&quot; ToleranceRZ=&quot;1.000000&quot; CheckBaseToleranceX=&quot;1&quot; CheckBaseToleranceY=&quot;1&quot; CheckBaseToleranceZ=&quot;1&quot; CheckBaseToleranceRX=&quot;0.100000&quot; CheckBaseToleranceRY=&quot;0.100000&quot; CheckBaseToleranceRZ=&quot;0.100000&quot; CheckW [obcięto pozostałą część ciągu]&quot;;.
        /// </summary>
        internal static string templateBase {
            get {
                return ResourceManager.GetString("templateBase", resourceCulture);
            }
        }
        
        /// <summary>
        /// Wyszukuje zlokalizowany ciąg podobny do ciągu &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;configuration&gt;
        ///  &lt;KUKARoboter.SafeRobot.Parameters&gt;
        ///    &lt;encryptedData&gt;
        ///      &lt;SafetyParameters&gt;
        ///        &lt;RangeMonitoring&gt;
        ///        &lt;/RangeMonitoring&gt;
        ///        &lt;Tools&gt;
        ///        &lt;/Tools&gt;
        ///      &lt;/SafetyParameters&gt;
        ///    &lt;/encryptedData&gt;
        ///  &lt;/KUKARoboter.SafeRobot.Parameters&gt;
        ///&lt;/configuration&gt;.
        /// </summary>
        internal static string templateSafetyKuka {
            get {
                return ResourceManager.GetString("templateSafetyKuka", resourceCulture);
            }
        }
    }
}