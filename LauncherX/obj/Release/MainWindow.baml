   M S B A M L   `   `   `  ���� D  @LauncherX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:LauncherX	LauncherX  D @FluentWPF, Version=0.8.0.6, Culture=neutral, PublicKeyToken=nullP6clr-namespace:SourceChord.FluentWPF;assembly=FluentWPFSourceChord.FluentWPF h dMicrosoft.Toolkit.Wpf.UI.XamlHost, Version=6.0.0.0, Culture=neutral, PublicKeyToken=4aff67a105548ee2�Zclr-namespace:Microsoft.Toolkit.Wpf.UI.XamlHost;assembly=Microsoft.Toolkit.Wpf.UI.XamlHost!Microsoft.Toolkit.Wpf.UI.XamlHost     LauncherX.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     )localclr-namespace:LauncherX     Efw6clr-namespace:SourceChord.FluentWPF;assembly=FluentWPF     oxamlHostZclr-namespace:Microsoft.Toolkit.Wpf.UI.XamlHost;assembly=Microsoft.Toolkit.Wpf.UI.XamlHost     -   )  #SourceChord.FluentWPF.AcrylicWindow   Name$  window�� � Title$* $LauncherX - Organize all your stuff!��$	��600��$	��830��$	��400��$	��650��   TintOpacity$
 0.75Q�   NoiseOpacity$
 0.01Q� � WindowStyle$
 None=�   AcrylicWindowStyle$
 None=�.��� -    �Name$ 	Container��$��0,0,0,0q�$��Stretch=�$��Stretch=�.+�� -   $
 grid��*  $SourceChord.FluentWPF.PointerTracker   Enabled . $��
0,90,0,0.5q�$��Stretch=�$��Stretch=�
�������.+�7  1Microsoft.Toolkit.Wpf.UI.XamlHost.WindowsXamlHost  -     Name$ OpenFileHost��	   InitialTypeName$%	 Windows.UI.Xaml.Controls.Button��$��	0,10,10,0q�$��Right=�$	��Top=�$��32��
 J� RenderTransformOrigin$
 0.5,0.5&�$	��105���� -    ��Name$ header��$
��Left=�$��	10,10,0,0q�$
��Wrap=�$%��My files, folders, and websites��$	��Top=�$
��Bold"�$��16(�  -   $ gridviewhost��$��10,47,10,55q�$'	 !Windows.UI.Xaml.Controls.GridView��$��Stretch=�$��Stretch=��� -    ��Name$ SettingsButton��$��Right=�$��40��$��0,0,10,5q�$��Bottom=�$��40��$��Segoe MDL2 Assets)�$	����$��19(���Z�   -   $ OpenFolderHost��$��Right=�$��32��$��
0,10,125,0q�$	��Top=�$	��110��$%	 Windows.UI.Xaml.Controls.Button��� -	   $ empty��$��Center=�$��55��$��0q�$��Center=�$	��350��.+��� -
    ��Name$ label1��$��There is nothing here.��$
��Left=�$	��Top=�$��16(�
�������$��40,0,0,0q��� -   $ label��$	����$��0q�$
��Left=�$��Center=�$��24(�$��Segoe MDL2 Assets)�
�������$��31���� -   $ label2��$@��:To add an item, click the buttons at the top right corner.��$
��Left=�$	��Top=�
�������$��	40,25,0,0q�  -   $ OpenWebsiteHost��$��Right=�$��32��$��
0,10,245,0q�$	��Top=�$	��120��$%	 Windows.UI.Xaml.Controls.Button��  -   $ 
SearchHost��$��32��$��
50,53,50,0q�$	��Top=�$-	 'Windows.UI.Xaml.Controls.AutoSuggestBox��� -   $ TitleBar��$��32��$	��Top=�$��Stretch=����	#00000000.+��� -   $ CloseButton��$	����$��0q�$��Center=�$��42��$��Segoe MDL2 Assets)�$��32��$��Right=�$��11(���Z� ��Z� �� -   $ MaximizeButton��$	����$��Right=�$��0,0,42,0q�$��Center=�$��42��$��Segoe MDL2 Assets)�$��32��$��11(���Z� ��Z� �� -   $ MinimiseButton��$	����$��Right=�$��0,0,84,0q�$��Center=�$��42��$��Segoe MDL2 Assets)�$��32��$��11(���Z� ��Z� �� -   $ TitleBarText��$
��Left=�$��30,0,0,0q�$
��Wrap=�$*��$LauncherX - Organize all your stuff!��$��Center=��� -    ��Name$ TitleBarIcon��$
��Left=�$��16��$��10,8,0,0q�$	��Top=�$��16��$��icon.ico��