﻿#pragma checksum "..\..\..\View\FlatFormV2.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "76054DA466D6D5A0942D7AE9958979BD919C23DAC86208569BF7CEA578AB3A9C"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using FontAwesome5;
using FontAwesome5.Converters;
using RealtorObjects.View;
using RealtorObjects.View.Converters;
using RealtorObjects.ViewModel;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace RealtorObjects.View {
    
    
    /// <summary>
    /// FlatFormV2
    /// </summary>
    public partial class FlatFormV2 : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 223 "..\..\..\View\FlatFormV2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid First;
        
        #line default
        #line hidden
        
        
        #line 303 "..\..\..\View\FlatFormV2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid Second;
        
        #line default
        #line hidden
        
        
        #line 414 "..\..\..\View\FlatFormV2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid Third;
        
        #line default
        #line hidden
        
        
        #line 572 "..\..\..\View\FlatFormV2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid Fourth;
        
        #line default
        #line hidden
        
        
        #line 860 "..\..\..\View\FlatFormV2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid Fifth;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/RealtorObjects;component/view/flatformv2.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\View\FlatFormV2.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\..\View\FlatFormV2.xaml"
            ((RealtorObjects.View.FlatFormV2)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Window_MouseDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.First = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            
            #line 267 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.AlphabeticalOnly);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 282 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.PhoneNumbers);
            
            #line default
            #line hidden
            return;
            case 5:
            this.Second = ((System.Windows.Controls.Grid)(target));
            return;
            case 6:
            
            #line 328 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.ComboBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.RussianOnly);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 342 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.ComboBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.RussianOnly);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 365 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.NumericOnly);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 378 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.NumericOnly);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 392 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.ComboBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.RussianOnly);
            
            #line default
            #line hidden
            return;
            case 11:
            this.Third = ((System.Windows.Controls.Grid)(target));
            return;
            case 12:
            
            #line 440 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.ComboBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.RussianOnly);
            
            #line default
            #line hidden
            return;
            case 13:
            
            #line 454 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.ComboBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.RussianOnly);
            
            #line default
            #line hidden
            return;
            case 14:
            
            #line 469 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.NumericOnly);
            
            #line default
            #line hidden
            return;
            case 15:
            
            #line 483 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.ComboBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.RussianOnly);
            
            #line default
            #line hidden
            return;
            case 16:
            
            #line 497 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.NumericOnly);
            
            #line default
            #line hidden
            return;
            case 17:
            
            #line 511 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.NumericOnly);
            
            #line default
            #line hidden
            return;
            case 18:
            
            #line 525 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.NumericOnly);
            
            #line default
            #line hidden
            return;
            case 19:
            
            #line 539 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.NumericOnly);
            
            #line default
            #line hidden
            return;
            case 20:
            
            #line 553 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.NumericOnly);
            
            #line default
            #line hidden
            return;
            case 21:
            this.Fourth = ((System.Windows.Controls.Grid)(target));
            return;
            case 22:
            
            #line 619 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.ComboBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.RussianOnly);
            
            #line default
            #line hidden
            return;
            case 23:
            
            #line 628 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.ComboBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.RussianOnly);
            
            #line default
            #line hidden
            return;
            case 24:
            
            #line 637 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.NumericOnly);
            
            #line default
            #line hidden
            return;
            case 25:
            
            #line 646 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.ComboBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.RussianOnly);
            
            #line default
            #line hidden
            return;
            case 26:
            
            #line 655 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.ComboBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.RussianOnly);
            
            #line default
            #line hidden
            return;
            case 27:
            
            #line 689 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.ComboBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.RussianOnly);
            
            #line default
            #line hidden
            return;
            case 28:
            
            #line 698 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.ComboBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.RussianOnly);
            
            #line default
            #line hidden
            return;
            case 29:
            
            #line 707 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.ComboBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.RussianOnly);
            
            #line default
            #line hidden
            return;
            case 30:
            
            #line 716 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.ComboBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.RussianOnly);
            
            #line default
            #line hidden
            return;
            case 31:
            
            #line 725 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.ComboBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.RussianOnly);
            
            #line default
            #line hidden
            return;
            case 32:
            
            #line 766 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.AlphabeticalOnly);
            
            #line default
            #line hidden
            return;
            case 33:
            
            #line 785 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.AnyLetter);
            
            #line default
            #line hidden
            return;
            case 34:
            
            #line 850 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.AnyLetter);
            
            #line default
            #line hidden
            return;
            case 35:
            this.Fifth = ((System.Windows.Controls.Grid)(target));
            return;
            case 36:
            
            #line 893 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.NumericOnly);
            
            #line default
            #line hidden
            return;
            case 37:
            
            #line 909 "..\..\..\View\FlatFormV2.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.NumericOnly);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

