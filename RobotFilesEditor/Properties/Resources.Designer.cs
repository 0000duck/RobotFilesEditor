﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ten kod został wygenerowany przez narzędzie.
//     Wersja wykonawcza:4.0.30319.42000
//
//     Zmiany w tym pliku mogą spowodować nieprawidłowe zachowanie i zostaną utracone, jeśli
//     kod zostanie ponownie wygenerowany.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RobotFilesEditor.Properties {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        /// Zwraca buforowane wystąpienie ResourceManager używane przez tę klasę.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RobotFilesEditor.Properties.Resources", typeof(Resources).Assembly);
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
        /// Wyszukuje zlokalizowany ciąg podobny do ciągu {DATE_AND_TIME}
        ///
        ///Version:  V8.36P/06/None
        ///Build ID: V8.3607        2/8/2017
        ///
        ///1A05B-2500-H510
        ///.
        /// </summary>
        internal static string BACKDATE {
            get {
                return ResourceManager.GetString("BACKDATE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Wyszukuje zlokalizowany zasób typu System.Drawing.Icon podobny do zasobu (Ikona).
        /// </summary>
        internal static System.Drawing.Icon Harvester {
            get {
                object obj = ResourceManager.GetObject("Harvester", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        /// Wyszukuje zlokalizowany ciąg podobny do ciągu &amp;ACCESS RV1
        ///&amp;REL 12
        ///&amp;PARAM DISKPATH = KRC:\R1\BMW_Program
        ///DEFDAT  {PATHNAME}
        ///
        ///;FOLD EXTERNAL DECLARATIONS;%{PE}%MKUKATPBASIS,%CEXT,%VCOMMON,%P
        ///;FOLD BASISTECH EXT;%{PE}%MKUKATPBASIS,%CEXT,%VEXT,%P
        ///EXT  BAS (BAS_COMMAND  :IN,REAL  :IN )
        ///DECL INT SUCCESS
        ///;ENDFOLD (BASISTECH EXT)
        ///;FOLD USER EXT;%{E}%MKUKATPUSER,%CEXT,%VEXT,%P
        ///;Make your modifications here
        ///
        ///;ENDFOLD (USER EXT)
        ///;ENDFOLD (EXTERNAL DECLARATIONS)
        ///
        ///
        ///;================================================
        ///; Positions (if any)
        ///;========== [obcięto pozostałą część ciągu]&quot;;.
        /// </summary>
        internal static string KUKA_DAT_TEMPLATE {
            get {
                return ResourceManager.GetString("KUKA_DAT_TEMPLATE", resourceCulture);
            }
        }
        
        /// <summary>
        /// Wyszukuje zlokalizowany ciąg podobny do ciągu &amp;ACCESS RV1
        ///&amp;REL 12
        ///&amp;PARAM DISKPATH = KRC:\R1\BMW_Program
        ///DEF  {PATHNAME}	( )
        ///;###### do not delete this line ######
        ///;FOLD INI 
        ///  ;FOLD USER INI
        ///    ;Make your modifications here
        ///			IF PLC_CHK_INIT() THEN
        ///				GLOBAL INTERRUPT DECL 3 WHEN $STOPMESS==TRUE DO IR_STOPM ( )
        ///				INTERRUPT ON 3 
        ///				BAS (#INITMOV,0 )
        ///				;FOLD APPLICATION_INI
        ///				APPLICATION_INI ( )
        ///				;ENDFOLD (APPLICATION_INI)
        ///			ENDIF	
        ///  ;ENDFOLD (USER INI)
        ///;ENDFOLD (INI)
        ///
        ///{PATH_SRC_CONTENT}
        ///
        ///END.
        /// </summary>
        internal static string KUKA_SRC_TEMPLATE {
            get {
                return ResourceManager.GetString("KUKA_SRC_TEMPLATE", resourceCulture);
            }
        }
        
        /// <summary>
        /// Wyszukuje zlokalizowany ciąg podobny do ciągu /PROG  PROG251
        ////ATTR
        ///OWNER		= MNEDITOR;
        ///COMMENT		= &quot;A01 0.0.1&quot;;
        ///PROG_SIZE	= 211;
        ///CREATE		= DATE 00-01-07  TIME 00:22:46;
        ///MODIFIED	= DATE 16-02-09  TIME 15:53:38;
        ///FILE_NAME	= ;
        ///VERSION		= 0;
        ///LINE_COUNT	= 1;
        ///MEMORY_SIZE	= 587;
        ///PROTECT		= READ_WRITE;
        ///TCD:  STACK_SIZE	= 0,
        ///      TASK_PRIORITY	= 50,
        ///      TIME_SLICE	= 0,
        ///      BUSY_LAMP_OFF	= 0,
        ///      ABORT_REQUEST	= 0,
        ///      PAUSE_REQUEST	= 0;
        ///DEFAULT_GROUP	= 1,*,*,*,*;
        ///CONTROL_CODE	= 00000000 00000000;
        ////APPL
        ///  HANDLING : TRUE ; 
        ///
        ///CORNER_ [obcięto pozostałą część ciągu]&quot;;.
        /// </summary>
        internal static string PROG251 {
            get {
                return ResourceManager.GetString("PROG251", resourceCulture);
            }
        }
        
        /// <summary>
        /// Wyszukuje zlokalizowany ciąg podobny do ciągu /PROG  PROG{ProgNum}
        ////ATTR
        ///OWNER		= MNEDITOR;
        ///COMMENT		= &quot;&quot;;
        ///PROG_SIZE	= 0;
        ///CREATE		= DATE {Date}  TIME {Time};
        ///MODIFIED	= DATE {Date}  TIME {Time};
        ///FILE_NAME	= ;
        ///VERSION		= 0;
        ///LINE_COUNT	= 0;
        ///MEMORY_SIZE	= 0;
        ///PROTECT		= READ_WRITE;
        ///TCD:  STACK_SIZE	= 0,
        ///      TASK_PRIORITY	= 50,
        ///      TIME_SLICE	= 0,
        ///      BUSY_LAMP_OFF	= 0,
        ///      ABORT_REQUEST	= 0,
        ///      PAUSE_REQUEST	= 0;
        ///DEFAULT_GROUP	= 1,*,*,*,*;
        ///CONTROL_CODE	= 00000000 00000000;
        ////APPL
        ///
        ///CORNER_SPEED_HEADER;
        ///  ENABLE_CORNER_SPEED  [obcięto pozostałą część ciągu]&quot;;.
        /// </summary>
        internal static string ProgHeaderFanucDE {
            get {
                return ResourceManager.GetString("ProgHeaderFanucDE", resourceCulture);
            }
        }
        
        /// <summary>
        /// Wyszukuje zlokalizowany ciąg podobny do ciągu /PROG  PROG{ProgNum}
        ////ATTR
        ///OWNER		= MNEDITOR;
        ///COMMENT		= &quot;&quot;;
        ///PROG_SIZE	= 0;
        ///CREATE		= DATE {Date}  TIME {Time};
        ///MODIFIED	= DATE {Date}  TIME {Time};
        ///FILE_NAME	= ;
        ///VERSION		= 0;
        ///LINE_COUNT	= 0;
        ///MEMORY_SIZE	= 0;
        ///PROTECT		= READ_WRITE;
        ///TCD:  STACK_SIZE	= 0,
        ///      TASK_PRIORITY	= 50,
        ///      TIME_SLICE	= 0,
        ///      BUSY_LAMP_OFF	= 0,
        ///      ABORT_REQUEST	= 0,
        ///      PAUSE_REQUEST	= 0;
        ///DEFAULT_GROUP	= 1,*,*,*,*;
        ///CONTROL_CODE	= 00000000 00000000;
        ////APPL
        ///
        ///CORNER_SPEED_HEADER;
        ///  ENABLE_CORNER_SPEED  [obcięto pozostałą część ciągu]&quot;;.
        /// </summary>
        internal static string ProgHeaderFanucEN {
            get {
                return ResourceManager.GetString("ProgHeaderFanucEN", resourceCulture);
            }
        }
        
        /// <summary>
        /// Wyszukuje zlokalizowany ciąg podobny do ciągu &amp;ACCESS RVO
        ///&amp;REL 5
        ///DEFDAT  UserVariables Public
        ///
        ///;***************************************************
        ///;*
        ///;*  Default .dat Template for Kuka-Krc-Bmw-User
        ///;*
        ///;* In this file the user can define additional
        ///;* needed variables.
        ///;*
        ///;* Rules like naming, language etc. must be
        ///;* according to the standard concept.
        ///;*
        ///;***************************************************
        ///
        ///;================================================
        ///;* Variable name               : MyTypNum
        ///;*
        ///;* Description of the function : [obcięto pozostałą część ciągu]&quot;;.
        /// </summary>
        internal static string UserVariables {
            get {
                return ResourceManager.GetString("UserVariables", resourceCulture);
            }
        }
        
        /// <summary>
        /// Wyszukuje zlokalizowany ciąg podobny do ciągu &amp;ACCESS RVO8
        ///&amp;REL 8
        ///DEF Utils( )
        ///END
        ///
        ///GLOBAL DEFFCT BOOL CHK_AXIS_POS (INPOS :IN)
        ///E6AXIS INPOS
        ///
        ///;***********************************************************
        ///;* Programm           : CHK_AXIS_POS
        ///;* Description        : CHK_AXIS_POS
        ///;* Robot              : KUKA KRC4
        ///;* Company            : AIUT 
        ///;* Programmer         : {NAME}
        ///;* Date               : {DATE}
        ///;* Change history     : V1.0 i.O.
        ///;***********************************************************
        ///
        ///IF ( ABS($axis_act.A1 - INPOS.A1)&lt;0.1) TH [obcięto pozostałą część ciągu]&quot;;.
        /// </summary>
        internal static string utils {
            get {
                return ResourceManager.GetString("utils", resourceCulture);
            }
        }
    }
}
