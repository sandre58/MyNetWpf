// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Markup;

[assembly: XmlnsPrefix("http://mynet.com/xaml/themes", "my")]
[assembly: XmlnsDefinition("http://mynet.com/xaml/themes", "MyNet.Wpf.Presentation.Controls")]
[assembly: XmlnsDefinition("http://mynet.com/xaml/themes", "MyNet.Wpf.Presentation.Converters")]
[assembly: XmlnsDefinition("http://mynet.com/xaml/themes", "MyNet.Wpf.Presentation.Parameters")]
[assembly: XmlnsDefinition("http://mynet.com/xaml/themes", "MyNet.Wpf.Presentation.Selectors")]
[assembly: XmlnsDefinition("http://mynet.com/xaml/themes", "MyNet.Wpf.Presentation.Views.Edition")]
[assembly: XmlnsDefinition("http://mynet.com/xaml/themes", "MyNet.Wpf.Presentation.Views.Export")]
[assembly: XmlnsDefinition("http://mynet.com/xaml/themes", "MyNet.Wpf.Presentation.Views.FileHistory")]
[assembly: XmlnsDefinition("http://mynet.com/xaml/themes", "MyNet.Wpf.Presentation.Views.List")]
[assembly: XmlnsDefinition("http://mynet.com/xaml/themes", "MyNet.Wpf.Presentation.Views.Shell")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]
