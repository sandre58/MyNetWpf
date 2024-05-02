// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

[assembly: XmlnsPrefix("http://mynet.com/xaml/themes", "my")]
[assembly: XmlnsDefinition("http://mynet.com/xaml/themes", "MyNet.Wpf.LiveCharts")]
[assembly: XmlnsDefinition("http://mynet.com/xaml/themes", "MyNet.Wpf.LiveCharts.Converters")]
[assembly: XmlnsDefinition("http://mynet.com/xaml/themes", "MyNet.Wpf.LiveCharts.Views")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]
